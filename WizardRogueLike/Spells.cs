using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;

namespace WizardRogueLike
{
    public enum element
    {
        fire = 0,
        toxic,
        ice,
        electro,
        water,
        wind,
        nature,
        ground,
        dark,
        none

    }
    public abstract class Spell
    {
        public float cooldown = 0.5f;
        public Vector2 position { get; set; }
        public Vector2 direction { get; set; }
        public Texture2D spellTexture { get; set; }
        public float speed { get; set; }

        public bool isEnemy { get; set; }

        public element myElement;

        bool isDOT = false;

        public float damage = 0;

        List<GameObject> enemiesHit = new List<GameObject>();

        public Spell(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            cooldown = 0.5f;
        }

        public virtual void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/red_hand");
        }

        public virtual bool Update(Game1 game, GameTime gameTime)
        {
            position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (position.X > game.areaSize.X || position.Y > game.areaSize.Y || position.X < 0 || position.Y < 0)
            {
                return true;
            }
            return false;
        }
        public virtual void Draw(Game1 game, SpriteBatch _spriteBatch, GameTime gameTime)
        {
            if (spellTexture == null) return;
            _spriteBatch.Draw(spellTexture, position, Color.White);
        }

        public void Hit(Game1 game, GameTime gameTime, GameObject enemyhit)
        {
            bool hasToxic = false;
            bool hasSlow = false;
            bool hasWet = false;
            bool hasParalisis = false;
            StatusEffect removed = null;
            StatusEffect enemyPoison = null;
            StatusEffect enemySlow = null;
            StatusEffect enemyWet = null;
            StatusEffect enemyParalisis = null;
            foreach (StatusEffect effect in enemyhit.effects)
            {
                if (effect.hasReacted) continue;
                if (effect.GetType() == typeof(Poisoned)) 
                {
                    hasToxic = true;
                    enemyPoison = effect;
                }
                if (effect.GetType() == typeof(Slow)) 
                {
                    hasSlow = true;
                    if (myElement == element.fire || myElement == element.electro) removed = effect;
                    enemySlow = effect;
                }
                if (effect.GetType() == typeof(Wet))
                {
                    hasWet = true;
                    if (myElement == element.electro) removed = effect;
                    enemyWet = effect;
                }
                if (effect.GetType() == typeof(Paralized))
                {
                    hasParalisis = true;
                    if (myElement == element.fire) removed = effect;
                    enemyParalisis = effect;
                }
            }

            enemyhit.effects.Remove(removed);

            if(myElement == element.fire && hasToxic && !enemiesHit.Contains(enemyhit))
            {
                enemiesHit.Add(enemyhit);
                FireBlast newSpellCast = new FireBlast(position, direction, 300, false);
                newSpellCast.Instantiate(game);

                game.spellsToSpawn.Add(newSpellCast);
                enemyPoison.hasReacted = true;
            }
            if(myElement == element.fire && hasSlow && !enemiesHit.Contains(enemyhit))
            {
                enemiesHit.Add(enemyhit);
                enemyhit.health -= 2;
                enemySlow.hasReacted = true;
                WaterAOE newSpellCast = new WaterAOE(enemyhit.position, direction, 300, false, 2, 5);
                newSpellCast.Instantiate(game);

                game.spellsToSpawn.Add(newSpellCast);
            }
            if(myElement == element.electro && hasSlow && !enemiesHit.Contains(enemyhit))
            {
                enemiesHit.Add(enemyhit);
                Paralized paralisis = new Paralized(1, 5);
                paralisis.Instantiate(enemyhit);
                enemyhit.effects.Add(paralisis);
                enemySlow.hasReacted = true;

            }
            if (myElement == element.electro && hasToxic && !enemiesHit.Contains(enemyhit))
            {
                enemiesHit.Add(enemyhit);
                ElectroAOE newSpellCast = new ElectroAOE(enemyhit.position, direction, 300, false, 2, 5);
                newSpellCast.Instantiate(game);

                game.spellsToSpawn.Add(newSpellCast);
                enemyPoison.hasReacted = true;
            }
            if(myElement == element.electro && hasWet && !enemiesHit.Contains(enemyhit))
            {
                enemiesHit.Add(enemyhit);
                Paralized paralisis = new Paralized(1.5f, damage);
                enemyhit.effects.Add(paralisis);
                enemyWet.hasReacted = true;
                paralisis.Instantiate(enemyhit);
                List<GameObject> nextEnemies = new List<GameObject>();
                float range = 128;
                foreach (GameObject enemy in game.enemyList)
                {
                    if (enemy == enemyhit) continue;
                    if(Vector2.Distance(enemyhit.position, enemy.position) < range)
                    {
                        nextEnemies.Add(enemy);
                    }
                    /*removed = null;
                    foreach (StatusEffect effect in enemyhit.effects)
                    {
                        if (effect.GetType() == typeof(Wet))
                        {
                            effect.hasReacted = true;
                            removed = effect;
                            enemy.effects.Add(paralisis);
                        }
                    }
                    enemyhit.effects.Remove(removed);*/
                }
                if (nextEnemies.Count > 0)
                {
                    foreach(GameObject enemy in nextEnemies)
                    {
                        Paralized paralisis2 = new Paralized(1.5f, damage);
                        paralisis2.Instantiate(enemy);
                        enemy.effects.Add(paralisis2);
                    }
                    /*Vector2 direction = (enemyhit.position - nextEnemy.position);
                    direction.Normalize();
                    Spell newSpellCast = (Spell)Activator.CreateInstance(this.GetType(), enemyhit.position - (direction * (game.playerRadius * 2f)), -direction, 300, false);
                    newSpellCast.Instantiate(game);
                    newSpellCast.damage = damage;
                    game.spellsToSpawn.Add(newSpellCast);*/
                }
            }
            if (myElement == element.fire && hasParalisis && !enemiesHit.Contains(enemyhit))
            {
                enemiesHit.Add(enemyhit);
                FireBlast newSpellCast = new FireBlast(enemyhit.position, direction, 300, false);
                newSpellCast.Instantiate(game);

                game.spellsToSpawn.Add(newSpellCast);
                enemyParalisis.hasReacted = true;
            }
        }
    }
    class FireBlast : Spell
    {
        public float lifetime { get; set; }
        public FireBlast(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = 0.25f;
            myElement = element.none;
            this.damage = 20;
        }

        public FireBlast(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _lifetime) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = _lifetime;
            myElement = element.none;
            this.damage = 20;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/blast");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (lifetime <= 0) return true;

            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, 64, enemy.position, game.playerRadius))
                    {
                        enemy.health -= damage * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
            }
            return false;
        }
    }
    #region Balls
    class Fireball : Spell
    {
        FireBlast myblast;
        public Fireball(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.cooldown = 2;
            myElement = element.fire;
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, 18, enemy.position, game.playerRadius))
                    {
                        enemy.health -= 7 + damage;
                        Hit(game, gameTime, enemy);
                        return true;
                    }
                }
            }
            return base.Update(game, gameTime);
            
        }
    }
    class ToxicBall : Spell
    {
        public ToxicBall(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            myElement = element.toxic;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/green_hand");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, 18, enemy.position, game.playerRadius))
                    {
                        Poisoned newPoison = new Poisoned(3, 7 + damage);
                        if (!newPoison.Instantiate(enemy))
                        {
                            enemy.effects.Add(newPoison);
                            return true;
                        }
                    }
                }
            }
            return base.Update(game, gameTime);
            
        }
    }
    class IceBall : Spell
    {
        public IceBall(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            cooldown = 1;
            myElement = element.ice;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/ice_ball");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, 18, enemy.position, game.playerRadius))
                    {
                        Slow newSlow = new Slow(1.5f, 0);
                        if (!newSlow.Instantiate(enemy))
                        {
                            
                            enemy.effects.Add(newSlow);
                            
                        }
                        enemy.health -= 5 + damage;
                        return true;
                    }
                }
            }
            return base.Update(game, gameTime);

        }
    }
    class ElectroBall : Spell
    {
        FireBlast myblast;
        public ElectroBall(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.cooldown = 1.5f;
            myElement = element.electro;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/yellow_hand");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, 18, enemy.position, game.playerRadius))
                    {
                        enemy.health -= 7 + damage;
                        Hit(game, gameTime, enemy);
                        return true;
                    }
                }
            }
            return base.Update(game, gameTime);

        }
    }
    class WaterBall : Spell
    {
        FireBlast myblast;
        public WaterBall(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.cooldown = 1f;
            myElement = element.water;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/water_ball");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, 18, enemy.position, game.playerRadius))
                    {
                        Wet newWet = new Wet(3, 0);
                        
                        if (!newWet.Instantiate(enemy))
                        {
                            enemy.effects.Add(newWet);
                        }
                        enemy.health -= 5 + damage;
                        return true;
                    }
                }
            }
            return base.Update(game, gameTime);

        }
    }
    #endregion
    #region AOEParticles
    class ToxicAOE : Spell
    {
        public float lifetime { get; set; }
        public ToxicAOE(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _damage) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            lifetime = 1;
            myElement = element.toxic;
            this.damage = _damage;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/green_hand");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (lifetime <= 0) return true;
            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, 18, enemy.position, game.playerRadius))
                    {
                        Poisoned newPoison = new Poisoned(2, 2 + damage);
                        if (!newPoison.Instantiate(enemy))
                        {
                            enemy.effects.Add(newPoison);
                            Hit(game, gameTime, enemy);
                            return true;
                        }
                    }
                }
            }
            return false;

        }
    }
    class IceAOE : Spell
    {
        public float lifetime { get; set; }
        public IceAOE(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _damage) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            lifetime = 3;
            myElement = element.ice;
            this.damage = _damage;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/ice_ball");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (lifetime <= 0) return true;
            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, 18, enemy.position, game.playerRadius))
                    {
                        Slow newSlow = new Slow(1.5f, 0);
                        if (!newSlow.Instantiate(enemy))
                        {
                            enemy.effects.Add(newSlow);
                            Hit(game, gameTime, enemy);
                        }
                    }
                }
            }
            return false;

        }
    }
    class FireAOE : Spell
    {
        public float lifetime {  get; set; }
        float reactionWait = 0;
        public FireAOE(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _damage) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = 2;
            myElement = element.fire;
            this.damage = _damage;
        }

        public FireAOE(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _damage, float _lifetime) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = _lifetime;
            myElement = element.fire;
            this.damage = _damage;
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (lifetime <= 0) return true;

            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, 18, enemy.position, game.playerRadius))
                    {
                        enemy.health -= (2 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if(reactionWait <= 0)
                            Hit(game, gameTime, enemy);
                        
                    }
                }
            }
            reactionWait -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            return false;
        }
    }
    class ElectroAOE : Spell
    {
        public float lifetime { get; set; }
        float reactionWait = 0;
        public ElectroAOE(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _damage) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = 3;
            myElement = element.electro;
            this.damage = _damage;
        }

        public ElectroAOE(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _damage, float _lifetime) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = _lifetime;
            myElement = element.electro;
            this.damage = _damage;
        }
        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/yellow_hand");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (lifetime <= 0) return true;

            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, 18, enemy.position, game.playerRadius))
                    {
                        enemy.health -= (2 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (reactionWait <= 0)
                            Hit(game, gameTime, enemy);

                    }
                }
            }
            reactionWait -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            return false;
        }
    }
    class WaterAOE : Spell
    {
        public float lifetime { get; set; }
        float reactionWait = 0;
        public WaterAOE(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _damage) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = 3;
            myElement = element.water;
            this.damage = _damage;
        }

        public WaterAOE(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _damage, float _lifetime) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = _lifetime;
            myElement = element.water;
            this.damage = _damage;
        }
        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/water_ball");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (lifetime <= 0) return true;

            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, 18, enemy.position, game.playerRadius))
                    {
                        enemy.health -= (1 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (reactionWait <= 0)
                            Hit(game, gameTime, enemy);
                        Wet newWet = new Wet(3, 0);

                        if (!newWet.Instantiate(enemy))
                        {
                            enemy.effects.Add(newWet);
                        }

                    }
                }
            }
            reactionWait -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            return false;
        }
    }
    #endregion


    #region Zones
    class FireZone : Spell
    {
        
        public List<FireAOE> FireAOEList = new List<FireAOE>();
        public FireZone(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            cooldown = 5;
            myElement = element.fire;
        }

        public override void Instantiate(Game1 game)
        {
            base.Instantiate(game);
            for (int y = 0; y <= 3; y++)
            {
                for (int x = -2; x <= 2; x++)
                {
                    Vector2 rightDirection = new Vector2(-direction.Y, direction.X);

                    Vector2 aoeposition = ((direction * y) * 18) + ((rightDirection * x) * 18);

                    FireAOE newSpellCast = new FireAOE(position + aoeposition, -direction, 300, false, damage);
                    newSpellCast.Instantiate(game);
                    FireAOEList.Add(newSpellCast);
                }
            }
        }

        public override void Draw(Game1 game, SpriteBatch _spriteBatch, GameTime gameTime)
        {
            foreach (FireAOE bullet in FireAOEList)
            {
                bullet.Draw(game, _spriteBatch, gameTime);
            }
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            FireAOE removed = null;

            foreach (FireAOE fireAOE in FireAOEList)
            {
                if (fireAOE.Update(game, gameTime)) removed = fireAOE;
            }
            if (removed != null) FireAOEList.Remove(removed);
            if (FireAOEList.Count == 0) return true;

            return false;
        }
    }
    class ToxicZone : Spell
    {

        public List<ToxicAOE> FireAOEList = new List<ToxicAOE>();
        public ToxicZone(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            cooldown = 5;
            myElement = element.toxic;
        }

        public override void Instantiate(Game1 game)
        {
            base.Instantiate(game);
            for (int y = 0; y <= 2; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    Vector2 rightDirection = new Vector2(-direction.Y, direction.X);

                    Vector2 aoeposition = ((direction * y) * 18) + ((rightDirection * x) * 18);

                    ToxicAOE newSpellCast = new ToxicAOE(position + aoeposition, -direction, 300, false, damage);
                    newSpellCast.Instantiate(game);
                    FireAOEList.Add(newSpellCast);
                }
            }
        }

        public override void Draw(Game1 game, SpriteBatch _spriteBatch, GameTime gameTime)
        {
            foreach (ToxicAOE bullet in FireAOEList)
            {
                bullet.Draw(game, _spriteBatch, gameTime);
            }
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            ToxicAOE removed = null;

            foreach (ToxicAOE fireAOE in FireAOEList)
            {
                if (fireAOE.Update(game, gameTime)) removed = fireAOE;
            }
            if (removed != null) FireAOEList.Remove(removed);
            if (FireAOEList.Count == 0) return true;
            return false;
        }
    }
    class IceZone : Spell
    {

        public List<IceAOE> FireAOEList = new List<IceAOE>();
        public IceZone(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            cooldown = 5;
            myElement = element.ice;
        }

        public override void Instantiate(Game1 game)
        {
            base.Instantiate(game);
            for (int y = 0; y <= 2; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    Vector2 rightDirection = new Vector2(-direction.Y, direction.X);

                    Vector2 aoeposition = ((direction * y) * 18) + ((rightDirection * x) * 18);

                    IceAOE newSpellCast = new IceAOE(position + aoeposition, -direction, 300, false, 0);
                    newSpellCast.Instantiate(game);
                    FireAOEList.Add(newSpellCast);
                }
            }
        }

        public override void Draw(Game1 game, SpriteBatch _spriteBatch, GameTime gameTime)
        {
            foreach (IceAOE bullet in FireAOEList)
            {
                bullet.Draw(game, _spriteBatch, gameTime);
            }
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            IceAOE removed = null;

            foreach (IceAOE fireAOE in FireAOEList)
            {
                if (fireAOE.Update(game, gameTime)) removed = fireAOE;
            }
            if (removed != null) FireAOEList.Remove(removed);
            if (FireAOEList.Count == 0) return true;
            return false;
        }
    }
    class ElectroZone : Spell
    {

        public List<ElectroAOE> FireAOEList = new List<ElectroAOE>();
        public ElectroZone(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            cooldown = 5;
            myElement = element.electro;
        }

        public override void Instantiate(Game1 game)
        {
            base.Instantiate(game);
            for (int y = 0; y <= 2; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    Vector2 rightDirection = new Vector2(-direction.Y, direction.X);

                    Vector2 aoeposition = ((direction * y) * 18) + ((rightDirection * x) * 18);

                    ElectroAOE newSpellCast = new ElectroAOE(position + aoeposition, -direction, 300, false, 0);
                    newSpellCast.Instantiate(game);
                    FireAOEList.Add(newSpellCast);
                }
            }
        }

        public override void Draw(Game1 game, SpriteBatch _spriteBatch, GameTime gameTime)
        {
            foreach (ElectroAOE bullet in FireAOEList)
            {
                bullet.Draw(game, _spriteBatch, gameTime);
            }
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            ElectroAOE removed = null;

            foreach (ElectroAOE fireAOE in FireAOEList)
            {
                if (fireAOE.Update(game, gameTime)) removed = fireAOE;
            }
            if (removed != null) FireAOEList.Remove(removed);
            if (FireAOEList.Count == 0) return true;
            return false;
        }
    }
    class WaterZone : Spell
    {

        public List<WaterAOE> FireAOEList = new List<WaterAOE>();
        public WaterZone(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            cooldown = 5;
            myElement = element.electro;
        }

        public override void Instantiate(Game1 game)
        {
            base.Instantiate(game);
            for (int y = 0; y <= 2; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    Vector2 rightDirection = new Vector2(-direction.Y, direction.X);

                    Vector2 aoeposition = ((direction * y) * 18) + ((rightDirection * x) * 18);

                    WaterAOE newSpellCast = new WaterAOE(position + aoeposition, -direction, 300, false, 0);
                    newSpellCast.Instantiate(game);
                    FireAOEList.Add(newSpellCast);
                }
            }
        }

        public override void Draw(Game1 game, SpriteBatch _spriteBatch, GameTime gameTime)
        {
            foreach (WaterAOE bullet in FireAOEList)
            {
                bullet.Draw(game, _spriteBatch, gameTime);
            }
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            WaterAOE removed = null;

            foreach (WaterAOE fireAOE in FireAOEList)
            {
                if (fireAOE.Update(game, gameTime)) removed = fireAOE;
            }
            if (removed != null) FireAOEList.Remove(removed);
            if (FireAOEList.Count == 0) return true;
            return false;
        }
    }
    #endregion

    #region Unique
    class Summon : Spell
    {
        Texture2D summonTexture;
        List<GameObject> summonList = new List<GameObject>();

        public Summon(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.cooldown = 5;
            myElement = element.dark;
            this.damage = 5;
        }

        public override void Instantiate(Game1 game)
        {
            summonTexture = game.Content.Load<Texture2D>("sprites/Characters/purple_character");

            summonList.Add(new GameObject(position, summonTexture, 48, 10));
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            GameObject removed = null;
            foreach(GameObject summon in summonList)
            {
                if (summon.health <= 0)
                {
                    removed = summon;
                    continue;
                }
                GameObject following = null;
                float closest = 99999;
                foreach(GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(summon.position, game.playerRadius, enemy.position, game.playerRadius))
                    {
                        enemy.health -= (5 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        summon.health -= 3.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        Hit(game, gameTime, enemy);
                        
                    }
                    if (Vector2.Distance(position, enemy.position) < closest)
                    {
                        following = enemy;
                        closest = Vector2.Distance(position, enemy.position);
                    }
                }
                if (following == null) continue;
                Vector2 direction = following.position - summon.position;
                direction.Normalize();
                summon.position += direction * summon.speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            summonList.Remove(removed);
            return false;
        }

        public override void Draw(Game1 game, SpriteBatch _spriteBatch, GameTime gameTime)
        {
            foreach(GameObject summon in summonList)
            {
                _spriteBatch.Draw(summonTexture, summon.position, Color.White);
            }
        }
    }
    class Dash : Spell
    {
        float dashStrength = 256;
        public Dash(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.cooldown = 5;
            myElement = element.none;
        }

        public override void Draw(Game1 game, SpriteBatch _spriteBatch, GameTime gameTime)
        {
            
        }

        public override void Instantiate(Game1 game)
        {
            Vector2 direction = (game.playerPosition - Mouse.GetState().Position.ToVector2()) + Vector2.UnitX * game.offsetX;
            direction.Normalize();
            game.playerPosition -= direction * dashStrength;
            game.isMoving = false;
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            
            return true;
        }
    }
    class FireWorm : Spell
    {
        public List<FireAOE> FireAOEList = new List<FireAOE>();
        public float timeBetweenAOE = 0.2f;
        int range = 15;
        public FireWorm(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            cooldown = 3;
            myElement = element.fire;
        }

        public override void Draw(Game1 game, SpriteBatch _spriteBatch, GameTime gameTime)
        {
            foreach (FireAOE bullet in FireAOEList)
            {
                bullet.Draw(game, _spriteBatch, gameTime);
            }
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            if (position.X > game.areaSize.X || position.Y > game.areaSize.Y || position.X < 0 || position.Y < 0)
            {
                if (FireAOEList.Count == 0) return true;
            }
            else
            {
                if (range > 0)
                {
                    timeBetweenAOE -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (timeBetweenAOE <= 0)
                    {
                        FireAOE newSpellCast = new FireAOE(position - (direction * game.staffTexture.Width), -direction, 300, false, damage);
                        newSpellCast.Instantiate(game);
                        FireAOEList.Add(newSpellCast);

                        timeBetweenAOE = 0.05f;
                        range--;
                    }
                }
            }
            FireAOE removed = null;

            foreach (FireAOE fireAOE in FireAOEList)
            {
                if (fireAOE.Update(game, gameTime)) removed = fireAOE;
            }
            if (removed != null) FireAOEList.Remove(removed);

            position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            return false;
        }
    }
    public class Turret : Spell
    {
        public float lifetime = 10;

        List<bool> canCast = new List<bool>(5);
        public Turret(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.cooldown = 5f;
            myElement = element.dark;
            this.damage = 5;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/purple_character");

            for(int i = 0; i < 5; i++)
            {
                canCast.Add(true);
            }

            game.turrets.Add(this);
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (lifetime <= 0) return true;
            return false;
        }

        public override void Draw(Game1 game, SpriteBatch _spriteBatch, GameTime gameTime)
        {
            if(spellTexture != null)
                _spriteBatch.Draw(spellTexture, position, Color.White);
        }
    }
    #endregion



    partial class Game1
    {
        public List<Type> mySpells = new List<Type>(5);
        public List<Type> allAvailableSpells = new List<Type>();
        public List<float> currentCooldowns = new List<float>(5);
        public List<int> spellDamageBonus = new List<int>(5);

        public List<Spell> spellsToSpawn = new List<Spell>();

        public Type currentSpell = null;
        public int currentSpellIndex = 0;

        public List<Turret> turrets = new List<Turret>();

        void SpellInstantiate ()
        {
            /*mySpells = new List<Type>(9);
            currentCooldowns = new List<float>(9);
            for (int i = 0; i < 9; i++)
            {
                mySpells.Add(null);
                currentCooldowns.Add(0);
            }
            
            mySpells[1] = typeof(ToxicBall);
            mySpells[2] = typeof(IceBall);
            mySpells[3] = typeof(FireZone);
            mySpells[4] = typeof(Summon);
            */


            currentSpell = mySpells[0];
        }

        void BulletUpdate(GameTime gameTime)
        {
            Spell removed = null;
            foreach (Spell bullet in bullets)
            {
                if (bullet.Update(this, gameTime)) removed = bullet;
            }
            if (removed != null)
            {
                if(removed.GetType() == typeof(Turret))
                {
                    turrets.Remove((Turret)removed);
                }
                bullets.Remove(removed);
            }
            if (spellsToSpawn.Count > 0)
            {
                for(int i = 0; i < spellsToSpawn.Count; i++)
                {
                    bullets.Add(spellsToSpawn[i]);
                    spellsToSpawn.Remove(spellsToSpawn[i]);
                }
            }
            spellsToSpawn = new List<Spell>();
            
        }

        void BulletDraw(GameTime gameTime)
        {
            foreach (Spell bullet in bullets)
            {
                bullet.Draw(this, _spriteBatch, gameTime);
            }
        }
    }
}
