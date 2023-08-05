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
    public abstract class Spell
    {
        public float cooldown = 0.5f;
        public Vector2 position { get; set; }
        public Vector2 direction { get; set; }
        public Texture2D spellTexture { get; set; }
        public float speed { get; set; }

        public bool isEnemy { get; set; }

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
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/yellow_hand");
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
    }

    class Fireball : Spell
    {
        public Fireball(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, 18, enemy.position, game.playerRadius))
                    {
                        enemy.health -= 5;
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
                        Poisoned newPoison = new Poisoned(2, 5);
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
    class ToxicAOE : Spell
    {
        public float lifetime { get; set; }
        public ToxicAOE(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            lifetime = 1;
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
                        Poisoned newPoison = new Poisoned(2, 5);
                        if (!newPoison.Instantiate(enemy))
                        {
                            enemy.effects.Add(newPoison);
                            return true;
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
        public FireAOE(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = 2;
        }

        public FireAOE(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _lifetime) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = _lifetime;
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
                        enemy.health -= 2 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
            }
            return false;
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
                        FireAOE newSpellCast = new FireAOE(position - (direction * game.staffTexture.Width), -direction, 300, false);
                        newSpellCast.Instantiate(game);
                        FireAOEList.Add(newSpellCast);

                        timeBetweenAOE = 0.05f;
                        range--;
                    }
                }
            }
            FireAOE removed = null;

            foreach(FireAOE fireAOE in FireAOEList)
            {
                if(fireAOE.Update(game, gameTime)) removed = fireAOE;
            }
            if(removed != null) FireAOEList.Remove(removed);

            position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            return false;
        }
    }

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

                    FireAOE newSpellCast = new FireAOE(position + aoeposition, -direction, 300, false);
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

                    ToxicAOE newSpellCast = new ToxicAOE(position + aoeposition, -direction, 300, false);
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

    class IceBall : Spell
    {
        public IceBall(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            cooldown = 2;
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
                        Slow newSlow = new Slow(1, 0);
                        if (!newSlow.Instantiate(enemy))
                        {
                            enemy.health -= 5;
                            enemy.effects.Add(newSlow);
                            return true;
                        }
                    }
                }
            }
            return base.Update(game, gameTime);

        }
    }

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
                        enemy.health -= 5 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        summon.health -= 3.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        
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
        }

        public override void Draw(Game1 game, SpriteBatch _spriteBatch, GameTime gameTime)
        {
            
        }

        public override void Instantiate(Game1 game)
        {
            Vector2 direction = (game.playerPosition - Mouse.GetState().Position.ToVector2()) + Vector2.UnitX * game.offsetX;
            direction.Normalize();
            game.playerPosition -= direction * dashStrength;
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            
            return true;
        }
    }

    partial class Game1
    {
        public List<Type> mySpells = new List<Type>(9);
        public List<Type> allAvailableSpells = new List<Type>();
        public List<float> currentCooldowns = new List<float>(9);

        Type currentSpell = null;
        int currentSpellIndex = 0;

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
                bullets.Remove(removed);
            }
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
