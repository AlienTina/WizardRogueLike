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
using static System.Net.Mime.MediaTypeNames;

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
        public float cooldown = 1f;
        public Vector2 position { get; set; }
        public Vector2 direction { get; set; }
        public Texture2D spellTexture { get; set; }
        public float speed { get; set; }

        public bool isEnemy { get; set; }

        public element myElement;

        bool isDOT = false;

        public float damage = 0;

        public float hitbox = 18;

        List<GameObject> enemiesHit = new List<GameObject>();

        List<Spell> spellsHit = new List<Spell>();

        public bool toRemove = false;

        public bool isInteractable = false;

        public bool isObstacle = false;

        public bool removeAfterReaction = false;

        public Spell(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            cooldown = 1f;
        }

        public virtual void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/red_hand");
        }

        public virtual bool Update(Game1 game, GameTime gameTime)
        {
            if (toRemove) return true;
            position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (position.X > game.areaSize.X || position.Y > game.areaSize.Y || position.X < 0 || position.Y < 0)
            {
                return true;
            }
            for (int i = 0; i < game.interactableSpells.Count; i++)
            {
                if (spellsHit.Contains(game.interactableSpells[i])) continue;
                if (game.CircleCollision(position, hitbox, game.interactableSpells[i].position, game.interactableSpells[i].hitbox))
                {
                    spellsHit.Add(game.interactableSpells[i]);
                    if (HitSpell(game, gameTime, game.interactableSpells[i])) return true;
                }

            }
            return false;
        }
        public virtual void Draw(Game1 game, SpriteBatch _spriteBatch, GameTime gameTime)
        {
            if (spellTexture == null) return;
            _spriteBatch.Draw(spellTexture, position, Color.White);
        }

        public void Hit(Game1 game, GameTime gameTime, GameObject enemyhit, float damage)
        {
            if(myElement == element.fire)
            {
                Fire newStatus = new Fire(3, 0);

                if (!newStatus.Instantiate(enemyhit))
                {
                    enemyhit.effects.Add(newStatus);
                }
            }
            else if (myElement == element.toxic)
            {
                Poisoned newStatus = new Poisoned(3, 7.0f/3.0f);

                if (!newStatus.Instantiate(enemyhit))
                {
                    enemyhit.effects.Add(newStatus);
                }
            }
            else if (myElement == element.ice)
            {
                Ice newStatus = new Ice(3, 0);

                if (!newStatus.Instantiate(enemyhit))
                {
                    enemyhit.effects.Add(newStatus);
                }
            }
            else if (myElement == element.electro)
            {
                Lightning newStatus = new Lightning(3, 0);

                if (!newStatus.Instantiate(enemyhit))
                {
                    enemyhit.effects.Add(newStatus);
                }
            }
            else if (myElement == element.water)
            {
                Water newStatus = new Water(3, 0);

                if (!newStatus.Instantiate(enemyhit))
                {
                    enemyhit.effects.Add(newStatus);
                }
            }
            else if (myElement == element.wind)
            {
                Wind newStatus = new Wind(3, 0);

                if (!newStatus.Instantiate(enemyhit))
                {
                    enemyhit.effects.Add(newStatus);
                }
            }
            else if (myElement == element.nature)
            {
                Nature newStatus = new Nature(3, 0);

                if (!newStatus.Instantiate(enemyhit))
                {
                    enemyhit.effects.Add(newStatus);
                }
            }
            else if (myElement == element.ground)
            {
                Ground newStatus = new Ground(3, 0);

                if (!newStatus.Instantiate(enemyhit))
                {
                    enemyhit.effects.Add(newStatus);
                }
            }
            else if(myElement == element.dark)
            {
                Dark newStatus = new Dark(3, 0);

                if (!newStatus.Instantiate(enemyhit))
                {
                    enemyhit.effects.Add(newStatus);
                }
                game.playerHealth += damage / 5;
            }
            if (enemiesHit.Contains(enemyhit)) return;
            if (game.reationCooldown > 0) return;
            List<StatusEffect> removed = new List<StatusEffect>();
            List<StatusEffect> newEffects = new List<StatusEffect>();
            foreach (StatusEffect effect in enemyhit.effects)
            {
                bool hasReacted = false;
                Debug.WriteLine(effect.GetType());
                if (effect.hasReacted) continue;
                if (effect.GetType() == typeof(Fire)) 
                {
                    if (myElement == element.ground)
                    {
                        MagmaZone newSpellCast = new MagmaZone(enemyhit.position, -direction, 300, false);
                        newSpellCast.Instantiate(game);
                        game.spellsToSpawn.Add(newSpellCast);
                        removed.Add(effect);
                        hasReacted = true;
                    }
                    else if(myElement == element.water)
                    {
                        SteamCloud newSpellCast = new SteamCloud(enemyhit.position - (Vector2.One * 32), -direction, 300, false);
                        newSpellCast.Instantiate(game);
                        game.spellsToSpawn.Add(newSpellCast);
                        removed.Add(effect);
                        hasReacted = true;
                    }
                    else if (myElement == element.dark)
                    {
                        ShadowFlames newSpellCast = new ShadowFlames(enemyhit.position - (Vector2.One * 32), -direction, 300, false, 1);
                        newSpellCast.Instantiate(game);
                        game.spellsToSpawn.Add(newSpellCast);
                        removed.Add(effect);
                        hasReacted = true;
                    }
                    else if (myElement == element.electro)
                    {
                        FireBlast newSpellCast = new FireBlast(enemyhit.position, -direction, 300, false);
                        newSpellCast.Instantiate(game);
                        game.spellsToSpawn.Add(newSpellCast);
                        removed.Add(effect);
                        hasReacted = true;

                    }
                }
                else if (effect.GetType() == typeof(Poisoned)) 
                {
                    if (myElement == element.water)
                    {
                        enemyhit.defenseShred++;
                        hasReacted = true;
                    }
                    else if (myElement == element.dark)
                    {
                        DarkCorrosion newStatus = new DarkCorrosion(3, 7.0f / 3.0f);

                        if (!newStatus.Instantiate(enemyhit))
                        {
                            newEffects.Add(newStatus);
                        }
                        enemyhit.damageBonus -= 0.5f;
                        hasReacted = true;
                    }
                }
                else if (effect.GetType() == typeof(Ice))
                {
                    if(myElement == element.wind)
                    {
                        Frostbite newStatus = new Frostbite(3, 0);

                        if (!newStatus.Instantiate(enemyhit))
                        {
                            newEffects.Add(newStatus);
                        }
                        hasReacted = true;

                    }
                    else if (myElement == element.water)
                    {
                        Frozen newStatus = new Frozen(1.5f, 0);

                        if (!newStatus.Instantiate(enemyhit))
                        {
                            newEffects.Add(newStatus);
                        }
                        hasReacted = true;
                    }
                }
                else if (effect.GetType() == typeof(Lightning))
                {
                    if(myElement == element.wind)
                    {
                        List<GameObject> closeEnemies = new List<GameObject>();
                        float range = 516;
                        foreach(GameObject enemy in game.enemyList)
                        {
                            if (enemy == enemyhit) continue;
                            if (Vector2.Distance(enemyhit.position, enemy.position) < range)
                                closeEnemies.Add(enemy);
                        }
                        foreach(GameObject enemy in closeEnemies)
                        {
                            Vector2 direction = enemyhit.position - enemy.position;
                            direction.Normalize();
                            ShockBolt newSpellCast = new ShockBolt(enemyhit.position - (direction * (game.playerRadius * 2f)), -direction, 300, false);
                            newSpellCast.Instantiate(game);
                            newSpellCast.damage = damage;
                            game.spellsToSpawn.Add(newSpellCast);
                        }
                        hasReacted = true;

                    }
                    else if(myElement == element.dark)
                    {
                        ElectroZone newSpellCast = new ElectroZone(enemyhit.position, -direction, 300, false);
                        newSpellCast.Instantiate(game);
                        game.spellsToSpawn.Add(newSpellCast);
                        removed.Add(effect);
                        hasReacted = true;

                    }
                    else if (myElement == element.fire)
                    {
                        FireBlast newSpellCast = new FireBlast(enemyhit.position, -direction, 300, false);
                        newSpellCast.Instantiate(game);
                        game.spellsToSpawn.Add(newSpellCast);
                        removed.Add(effect);
                        hasReacted = true;

                    }
                }
                else if (effect.GetType() == typeof(Water))
                {
                    if(myElement == element.toxic)
                    {
                        enemyhit.defenseShred++;
                        hasReacted = true;
                    }
                    else if (myElement == element.nature)
                    {
                        RootZone newSpellCast = new RootZone(enemyhit.position, -direction, 300, false);
                        newSpellCast.Instantiate(game);
                        game.spellsToSpawn.Add(newSpellCast);
                        removed.Add(effect);
                        hasReacted = true;
                    }
                    else if (myElement == element.fire)
                    {
                        SteamCloud newSpellCast = new SteamCloud(enemyhit.position - (Vector2.One * 32), -direction, 300, false);
                        newSpellCast.Instantiate(game);
                        game.spellsToSpawn.Add(newSpellCast);
                        removed.Add(effect);
                        hasReacted = true;
                    }
                    else if (myElement == element.ice)
                    {
                        Frozen newStatus = new Frozen(1.5f, 0);

                        if (!newStatus.Instantiate(enemyhit))
                        {
                            newEffects.Add(newStatus);
                        }
                        hasReacted = true;
                    }
                }
                else if (effect.GetType() == typeof(Wind))
                {
                    if (myElement == element.ice)
                    {
                        Frostbite newStatus = new Frostbite(3, 0);

                        if (!newStatus.Instantiate(enemyhit))
                        {
                            newEffects.Add(newStatus);
                        }
                        hasReacted = true;

                    }
                    else if (myElement == element.electro)
                    {
                        List<GameObject> closeEnemies = new List<GameObject>();
                        float range = 516;
                        foreach (GameObject enemy in game.enemyList)
                        {
                            if (enemy == enemyhit) continue;
                            if (Vector2.Distance(enemyhit.position, enemy.position) < range)
                                closeEnemies.Add(enemy);
                        }
                        foreach (GameObject enemy in closeEnemies)
                        {
                            Vector2 direction = enemyhit.position - enemy.position;
                            direction.Normalize();
                            ShockBolt newSpellCast = new ShockBolt(enemyhit.position - (direction * (game.playerRadius * 2f)), -direction, 300, false);
                            newSpellCast.Instantiate(game);
                            newSpellCast.damage = damage;
                            game.spellsToSpawn.Add(newSpellCast);
                        }
                        hasReacted = true;
                    }
                    else if(myElement == element.dark)
                    {
                        ShadowZone newSpellCast = new ShadowZone(enemyhit.position - (Vector2.One * 32), -direction, 300, false);
                        newSpellCast.Instantiate(game);
                        game.spellsToSpawn.Add(newSpellCast);
                        removed.Add(effect);
                        hasReacted = true;

                    }
                }
                else if (effect.GetType() == typeof(Nature))
                {
                    if (myElement == element.water)
                    {
                        RootZone newSpellCast = new RootZone(enemyhit.position, -direction, 300, false);
                        newSpellCast.Instantiate(game);
                        game.spellsToSpawn.Add(newSpellCast);
                        removed.Add(effect);
                        hasReacted = true;
                    }
                    else if (myElement == element.ground)
                    {
                        Petrified newStatus = new Petrified(1.5f, 0);

                        if (!newStatus.Instantiate(enemyhit))
                        {
                            newEffects.Add(newStatus);
                        }
                        hasReacted = true;
                    }
                }
                else if (effect.GetType() == typeof(Ground))
                {
                    if(myElement == element.fire)
                    {
                        MagmaZone newSpellCast = new MagmaZone(enemyhit.position, -direction, 300, false);
                        newSpellCast.Instantiate(game);
                        game.spellsToSpawn.Add(newSpellCast);
                        removed.Add(effect);
                        hasReacted = true;
                    }
                    else if(myElement == element.nature)
                    {
                        Petrified newStatus = new Petrified(1.5f, 0);

                        if (!newStatus.Instantiate(enemyhit))
                        {
                            newEffects.Add(newStatus);
                        }
                        hasReacted = true;
                    }
                }
                else if (effect.GetType() == typeof(Dark))
                {
                    if(myElement == element.toxic)
                    {
                        DarkCorrosion newStatus = new DarkCorrosion(3, 7.0f/3.0f);

                        if (!newStatus.Instantiate(enemyhit))
                        {
                            newEffects.Add(newStatus);
                        }
                        enemyhit.damageBonus -= 0.5f;
                        hasReacted = true;
                    }
                    else if (myElement == element.electro)
                    {
                        ElectroZone newSpellCast = new ElectroZone(enemyhit.position, -direction, 300, false);
                        newSpellCast.Instantiate(game);
                        game.spellsToSpawn.Add(newSpellCast);
                        removed.Add(effect);
                        hasReacted = true;
                    }
                    else if (myElement == element.wind)
                    {
                        ShadowZone newSpellCast = new ShadowZone(enemyhit.position - (Vector2.One * 32), -direction, 300, false);
                        newSpellCast.Instantiate(game);
                        game.spellsToSpawn.Add(newSpellCast);
                        removed.Add(effect);
                        hasReacted = true;
                    }
                    else if (myElement == element.fire)
                    {
                        ShadowFlames newSpellCast = new ShadowFlames(enemyhit.position - (Vector2.One * 32), -direction, 300, false, 1);
                        newSpellCast.Instantiate(game);
                        game.spellsToSpawn.Add(newSpellCast);
                        removed.Add(effect);
                        hasReacted = true;
                    }
                }
                if (hasReacted) game.reationCooldown = 1f;
            }

            enemyhit.effects.AddRange(newEffects);
            if (removed.Count > 0)
            {
                foreach(StatusEffect effect in removed)
                    enemyhit.effects.Remove(effect);
            }
            enemiesHit.Add(enemyhit);
        }

        public bool HitSpell(Game1 game, GameTime gameTime, Spell spell)
        {
            element spellElement = spell.myElement;


            
            return false;
        }
    }
    #region reactionSpells
    class MagmaZone : Spell
    {
        public float lifetime { get; set; }
        public MagmaZone(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = 1f;
            myElement = element.none;
            this.damage = 0;
            this.hitbox = 64;
        }

        public MagmaZone(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _lifetime) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = _lifetime;
            myElement = element.none;
            this.damage = 0;
            this.hitbox = 64;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/MagmaZone");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (lifetime <= 0) return true;

            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position + (Vector2.One * hitbox), hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, (10 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        Hit(game, gameTime, enemy, (1 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                }
            }
            return false;
        }
    }
    class RootZone : Spell
    {
        public float lifetime { get; set; }
        public RootZone(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = 1f;
            myElement = element.nature;
            this.damage = 0;
            this.hitbox = 64;
        }

        public RootZone(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _lifetime) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = _lifetime;
            myElement = element.nature;
            this.damage = 0;
            this.hitbox = 64;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/RootZone");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (lifetime <= 0) return true;

            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleOnBox((enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox, position, spellTexture.Bounds.Size.ToVector2()))
                    {
                        enemy.Hit(game, (10 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        Hit(game, gameTime, enemy, (1 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                }
            }
            return false;
        }
    }
    class ElectroZone : Spell
    {
        public float lifetime { get; set; }
        public float pullForce = 64;
        public ElectroZone(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = 1f;
            myElement = element.dark;
            this.damage = 0;
            this.hitbox = 64;
        }

        public ElectroZone(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _lifetime) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = _lifetime;
            myElement = element.dark;
            this.damage = 0;
            this.hitbox = 64;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/ElectroField");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (lifetime <= 0) return true;

            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision((enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox, position, hitbox))
                    {
                        enemy.Hit(game, (5 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        Hit(game, gameTime, enemy, (5 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        Vector2 direction = (position + Vector2.One * (hitbox/2)) - enemy.position;
                        direction.Normalize();
                        enemy.position += direction * pullForce * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
            }
            return false;
        }
    }
    class SteamCloud : Spell
    {
        public float lifetime { get; set; }
        public SteamCloud(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = 1f;
            myElement = element.none;
            this.damage = 0;
            this.hitbox = 64;
        }

        public SteamCloud(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _lifetime) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = _lifetime;
            myElement = element.none;
            this.damage = 0;
            this.hitbox = 64;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/SteamCloud");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (lifetime <= 0) return true;

            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, (5 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        Slow newStatus = new Slow(3, 0);

                        if (!newStatus.Instantiate(enemy))
                        {
                            enemy.effects.Add(newStatus);
                        }
                        Hit(game, gameTime, enemy, (5 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        
                    }
                }
            }
            return false;
        }
    }
    class ShadowZone : Spell
    {
        public float lifetime { get; set; }
        public ShadowZone(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = 2f;
            myElement = element.none;
            this.damage = 0;
            this.hitbox = 64;
        }

        public ShadowZone(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _lifetime) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = _lifetime;
            myElement = element.none;
            this.damage = 0;
            this.hitbox = 64;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/ShadowZone");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (lifetime <= 0) return true;

            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        //enemy.health -= (5 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        Confused newStatus = new Confused(3, 0);

                        if (!newStatus.Instantiate(enemy))
                        {
                            enemy.effects.Add(newStatus);
                        }
                        Hit(game, gameTime, enemy, 0);

                    }
                }
            }
            return false;
        }
    }
    class ShadowFlames : Spell
    {
        public float lifetime { get; set; }
        public ShadowFlames(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = 2f;
            myElement = element.none;
            this.damage = 0;
            this.hitbox = 64;
        }

        public ShadowFlames(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _lifetime) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = _lifetime;
            myElement = element.none;
            this.damage = 0;
            this.hitbox = 64;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/ShadowFlames");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (lifetime <= 0) return true;

            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.health -= (5 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        Confused newStatus = new Confused(3, 0);

                        if (!newStatus.Instantiate(enemy))
                        {
                            enemy.effects.Add(newStatus);
                        }
                        Hit(game, gameTime, enemy, 0);

                    }
                }
            }
            return false;
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
            myElement = element.fire;
            this.damage = 20;
            this.hitbox = 64;
        }

        public FireBlast(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _lifetime) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = _lifetime;
            myElement = element.fire;
            this.damage = 20;
            this.hitbox = 64;
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
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, damage * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        Hit(game, gameTime, enemy, damage * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                }
            }
            return false;
        }
    }

    #endregion

    #region Balls
    class FireBall : Spell
    {
        public FireBall(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            myElement = element.fire;
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, 5 + damage);
                        Hit(game, gameTime, enemy, 5 + damage);
                        return true;
                    }
                }
            }
            return base.Update(game, gameTime);
            
        }
    }
    class VenomDart : Spell
    {
        public VenomDart(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
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
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, 5 + damage);
                        Hit(game, gameTime, enemy, 5 + damage);
                        return true;

                    }
                }
            }
            return base.Update(game, gameTime);
            
        }
    }
    class FrostShard : Spell
    {
        public FrostShard(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
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
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, 5 + damage);
                        Hit(game, gameTime, enemy, 5 + damage);
                        return true;
                    }
                }
            }
            return base.Update(game, gameTime);

        }
    }
    class FreezeBall : Spell
    {
        public FreezeBall(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
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
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, 5 + damage);
                        Stun newStatus = new Stun(3, 0);

                        if (!newStatus.Instantiate(enemy))
                        {
                            enemy.effects.Add(newStatus);
                        }
                        //Hit(game, gameTime, enemy, 5 + damage);
                        return true;
                    }
                }
            }
            return base.Update(game, gameTime);

        }
        
    }
    class ShockBolt : Spell
    {
        public ShockBolt(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            myElement = element.electro;
            this.removeAfterReaction = true;
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
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, 5 + damage);
                        Hit(game, gameTime, enemy, 5 + damage);
                        return true;
                    }
                }
            }
            return base.Update(game, gameTime);

        }
    }
    class AquaJet : Spell
    {
        public AquaJet(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
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
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, 5 + damage);
                        Hit(game, gameTime, enemy, 5 + damage);
                        return true;
                    }
                }
            }
            foreach (Spell spell in game.interactableSpells)
            {
                if (game.CircleCollision(position, hitbox, spell.position, spell.hitbox))
                {
                    if (HitSpell(game, gameTime, spell)) return true;
                }
            }
            return base.Update(game, gameTime);

        }
    }
    class AirBlast : Spell
    {
        public AirBlast(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            myElement = element.wind;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/wind_ball");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, 5 + damage);
                        Hit(game, gameTime, enemy, 5 + damage);
                        return true;
                    }
                }
            }
            foreach (Spell spell in game.interactableSpells)
            {
                if (game.CircleCollision(position, hitbox, spell.position, spell.hitbox))
                {
                    if (HitSpell(game, gameTime, spell)) return true;
                }
            }
            return base.Update(game, gameTime);

        }
    }
    class VineSnare : Spell
    {
        public VineSnare(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            myElement = element.nature;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/grass_ball");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, 5 + damage);
                        Hit(game, gameTime, enemy, 5 + damage);
                        return true;
                    }
                }
            }
            foreach (Spell spell in game.interactableSpells)
            {
                if (game.CircleCollision(position, hitbox, spell.position, spell.hitbox))
                {
                    if (HitSpell(game, gameTime, spell)) return true;
                }
            }
            return base.Update(game, gameTime);

        }
    }
    class EarthSpike : Spell
    {
        public EarthSpike(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            myElement = element.ground;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/ground_ball");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, 5 + damage);
                        Hit(game, gameTime, enemy, 5 + damage);
                        return true;
                    }
                }
            }
            foreach (Spell spell in game.interactableSpells)
            {
                if (game.CircleCollision(position, hitbox, spell.position, spell.hitbox))
                {
                    if (HitSpell(game, gameTime, spell)) return true;
                }
            }
            return base.Update(game, gameTime);

        }
    }
    class ShadowBolt : Spell
    {
        public ShadowBolt(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            myElement = element.dark;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/dark_ball");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            
            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, 5 + damage);
                        Hit(game, gameTime, enemy, 5 + damage);
                        return true;
                    }
                }
            }
            foreach (Spell spell in game.interactableSpells)
            {
                if (game.CircleCollision(position, hitbox, spell.position, spell.hitbox))
                {
                    if (HitSpell(game, gameTime, spell)) return true;
                }
            }
            return base.Update(game, gameTime);

        }
    }
    #endregion

    #region SubSpells
    class WaterWaveParticle : Spell
    {
        public float lifetime { get; set; }
        public WaterWaveParticle(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = 1.5f;
            myElement = element.water;
            this.damage = 0;
            this.hitbox = 18;
        }

        public WaterWaveParticle(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _lifetime) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = _lifetime;
            myElement = element.water;
            this.damage = 0;
            this.hitbox = 18;
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
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, (5 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        enemy.position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        Hit(game, gameTime, enemy, (5 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                }
            }
            return false;
        }
    }

    class VineWallParticle : Spell
    {
        public float lifetime { get; set; }
        public VineWallParticle(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = 2.5f;
            myElement = element.nature;
            this.damage = 0;
            this.hitbox = 18;
        }

        public VineWallParticle(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _lifetime) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = _lifetime;
            myElement = element.nature;
            this.damage = 0;
            this.hitbox = 18;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/bloom_ball");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (lifetime <= 0) return true;

            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, (5 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        Vector2 enemydirection = enemy.target - enemy.position;
                        enemydirection.Normalize();
                        enemy.position -= enemydirection * (enemy.speed * 1.2f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        Hit(game, gameTime, enemy, (5 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                }
            }
            return false;
        }
    }

    class Darkness : Spell
    {
        public float lifetime { get; set; }
        public Darkness(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = 1f;
            myElement = element.dark;
            this.damage = 0;
            this.hitbox = 64;
        }

        public Darkness(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _lifetime) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = _lifetime;
            myElement = element.dark;
            this.damage = 0;
            this.hitbox = 64;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/Darkness");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (lifetime <= 0) return true;

            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        //enemy.health -= (5 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        Weakened newStatus = new Weakened(3, 0);

                        if (!newStatus.Instantiate(enemy))
                        {
                            enemy.effects.Add(newStatus);
                        }
                        Hit(game, gameTime, enemy, 0);

                    }
                }
            }
            return false;
        }
    }

    #endregion

    #region TransformedSpells

    class InfernoOrb : Spell
    {
        public InfernoOrb(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.cooldown = 3;
            myElement = element.fire;
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, 10 + damage);
                        Hit(game, gameTime, enemy, 10 + damage);
                        List<GameObject> closeEnemies = new List<GameObject>();
                        float range = 516;
                        foreach (GameObject enemy2 in game.enemyList)
                        {
                            if (Vector2.Distance(enemy.position, enemy2.position) < range)
                                closeEnemies.Add(enemy2);
                        }
                        int ballsLeft = 3;
                        foreach (GameObject enemy2 in closeEnemies)
                        {
                            if (ballsLeft <= 0) break;
                            Vector2 direction = enemy.position - enemy2.position;
                            direction.Normalize();
                            FireBall newSpellCast = new FireBall(enemy.position - (direction * (game.playerRadius * 2f)), -direction, 300, false);
                            newSpellCast.Instantiate(game);
                            newSpellCast.damage = damage;
                            game.spellsToSpawn.Add(newSpellCast);
                            ballsLeft--;
                        }
                        return true;
                    }
                }
            }
            return base.Update(game, gameTime);

        }
    }
    class ToxicBarrage : Spell
    {
        public ToxicBarrage(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.cooldown = 3;
            myElement = element.toxic;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/green_hand");
            float angle1 = 10;
            float angle2 = 350;
            for (int i = -1; i <= 1; i++)
            {
                //if (i == 0) continue;
                Vector2 direction = (position - game.CursorPosition) + Vector2.UnitX * game.offsetX;
                direction.Normalize();
                Vector2 rightDirection = new Vector2(direction.Y, -direction.X);
                Vector2 newDirection = (position - game.CursorPosition) + Vector2.UnitX * game.offsetX + (rightDirection * (32 * i));
                newDirection.Normalize();
                VenomDart newSpellCast = new VenomDart(position - (newDirection), -newDirection, 300, false);
                newSpellCast.Instantiate(game);
                newSpellCast.damage = damage;
                game.spellsToSpawn.Add(newSpellCast);
            }
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, 5 + damage);
                        Hit(game, gameTime, enemy, 5 + damage);
                        return true;
                    }
                }
            }
            return base.Update(game, gameTime);

        }
    }
    class GlacialBurst : Spell
    {
        public GlacialBurst(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.cooldown = 3;
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
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, 10 + damage);
                        Hit(game, gameTime, enemy, 10 + damage);
                        Stun newStatus = new Stun(3, 0);

                        if (!newStatus.Instantiate(enemy))
                        {
                            enemy.effects.Add(newStatus);
                        }
                        List<GameObject> closeEnemies = new List<GameObject>();
                        float range = 516;
                        foreach (GameObject enemy2 in game.enemyList)
                        {
                            if (Vector2.Distance(enemy.position, enemy2.position) < range)
                                closeEnemies.Add(enemy2);
                        }
                        int ballsLeft = 3;
                        foreach (GameObject enemy2 in closeEnemies)
                        {
                            if (ballsLeft <= 0) break;
                            Vector2 direction = enemy.position - enemy2.position;
                            direction.Normalize();
                            FreezeBall newSpellCast = new FreezeBall(enemy.position - (direction * (game.playerRadius * 2f)), -direction, 300, false);
                            newSpellCast.Instantiate(game);
                            newSpellCast.damage = damage;
                            game.spellsToSpawn.Add(newSpellCast);
                            ballsLeft--;
                        }
                        return true;
                    }
                }
            }
            return base.Update(game, gameTime);

        }
    }
    class Thunderstrike : Spell
    {
        public Thunderstrike(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.cooldown = 3;
            myElement = element.electro;
        }
        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/yellow_hand");
        }
        public override bool Update(Game1 game, GameTime gameTime)
        {
            if (damage < -3) damage = -3;
            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, 3 + damage);
                        Hit(game, gameTime, enemy, 3 + damage);
                        GameObject closeEnemy = null;
                        float range = float.PositiveInfinity;
                        foreach (GameObject enemy2 in game.enemyList)
                        {
                            if (enemy2 == enemy) continue;
                            if (Vector2.Distance(enemy.position, enemy2.position) < range)
                                closeEnemy = enemy2;
                        }

                        if (closeEnemy != null)
                        {
                            Vector2 newDirection = enemy.position - closeEnemy.position;
                            newDirection.Normalize();
                            Thunderstrike newSpellCast = new Thunderstrike(enemy.position - (newDirection * (closeEnemy.hitBox * 2f)), -newDirection, 600, false);
                            newSpellCast.Instantiate(game);
                            newSpellCast.damage = damage-1;
                            game.spellsToSpawn.Add(newSpellCast);
                        }
                        return true;
                    }
                }
            }
            return base.Update(game, gameTime);

        }
    }

    class TorrentialWave : Spell
    {

        public List<WaterWaveParticle> FireAOEList = new List<WaterWaveParticle>();
        public TorrentialWave(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed / 2.5f;
            this.isEnemy = _isEnemy;
            cooldown = 3;
            myElement = element.water;
        }

        public override void Instantiate(Game1 game)
        {
            base.Instantiate(game);
            for (int y = 0; y < 2; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    Vector2 rightDirection = new Vector2(-direction.Y, direction.X);

                    Vector2 aoeposition = ((direction * y) * 18) + ((rightDirection * x) * 18);

                    WaterWaveParticle newSpellCast = new WaterWaveParticle(position + aoeposition, direction, speed / 8f, false);
                    newSpellCast.Instantiate(game);
                    FireAOEList.Add(newSpellCast);
                }
            }
        }

        public override void Draw(Game1 game, SpriteBatch _spriteBatch, GameTime gameTime)
        {
            foreach (WaterWaveParticle bullet in FireAOEList)
            {
                bullet.Draw(game, _spriteBatch, gameTime);
            }
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            WaterWaveParticle removed = null;

            foreach (WaterWaveParticle fireAOE in FireAOEList)
            {
                fireAOE.position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (fireAOE.Update(game, gameTime)) removed = fireAOE;
            }
            if (removed != null) FireAOEList.Remove(removed);
            if (FireAOEList.Count == 0) return true;
            return false;
        }
    }

    class CycloneWhirl : Spell
    {
        public float lifetime { get; set; }
        public float pullForce = 128;
        public CycloneWhirl(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed / 3;
            this.isEnemy = _isEnemy;
            this.lifetime = 1f;
            myElement = element.wind;
            this.damage = 0;
            this.hitbox = 32;
        }

        public CycloneWhirl(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _lifetime) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed / 3;
            this.isEnemy = _isEnemy;
            this.lifetime = _lifetime;
            myElement = element.wind;
            this.damage = 0;
            this.hitbox = 32;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/Tornado");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (lifetime <= 0) return true;

            position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleOnBox((enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox, position, spellTexture.Bounds.Size.ToVector2()))
                    {
                        enemy.Hit(game, (10 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        Hit(game, gameTime, enemy, (10 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        Vector2 direction = (position + Vector2.One * (hitbox / 2)) - enemy.position;
                        direction.Normalize();
                        enemy.position += direction * pullForce * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
            }
            return false;
        }
    }

    class ThornedGrove : Spell
    {

        public List<VineWallParticle> FireAOEList = new List<VineWallParticle>();
        public ThornedGrove(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed / 2.5f;
            this.isEnemy = _isEnemy;
            cooldown = 3;
            myElement = element.nature;
        }

        public override void Instantiate(Game1 game)
        {
            position += direction * 64;
            base.Instantiate(game);
            for (int y = 0; y < 1; y++)
            {
                for (int x = -2; x <= 2; x++)
                {
                    
                    Vector2 rightDirection = new Vector2(-direction.Y, direction.X);

                    Vector2 aoeposition = ((direction * y) * 18) + ((rightDirection * x) * 18);

                    VineWallParticle newSpellCast = new VineWallParticle(position + aoeposition, direction, speed / 8f, false);
                    newSpellCast.Instantiate(game);
                    FireAOEList.Add(newSpellCast);
                }
            }
        }

        public override void Draw(Game1 game, SpriteBatch _spriteBatch, GameTime gameTime)
        {
            foreach (VineWallParticle bullet in FireAOEList)
            {
                bullet.Draw(game, _spriteBatch, gameTime);
            }
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            VineWallParticle removed = null;

            foreach (VineWallParticle fireAOE in FireAOEList)
            {
                //fireAOE.position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (fireAOE.Update(game, gameTime)) removed = fireAOE;
            }
            if (removed != null) FireAOEList.Remove(removed);
            if (FireAOEList.Count == 0) return true;
            return false;
        }
    }
    class QuakeTremor : Spell
    {
        public float lifetime { get; set; }
        Vector2 baseposition;
        public QuakeTremor(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = 1;
            this.cooldown = 3;
            myElement = element.ground;
            this.damage = 0;
            this.hitbox = 128;
        }

        public QuakeTremor(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy, float _lifetime) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.lifetime = _lifetime;
            this.cooldown = 3;
            myElement = element.ground;
            this.damage = 0;
            this.hitbox = 128;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/Quake");
            position -= Vector2.One * (hitbox / 3.5f);
            position += direction * (hitbox / 2);
            baseposition = position;
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (lifetime <= 0) return true;

            position = baseposition;

            Vector2 randomDirection = position - game.generateRandomPosition();
            randomDirection.Normalize();

            position += randomDirection * 128 * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleOnBox((enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox, position + (Vector2.One * (hitbox / 3.5f)), Vector2.One * hitbox))
                    {
                        enemy.Hit(game, (11 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        Stun newStatus = new Stun(2, 0);

                        if (!newStatus.Instantiate(enemy))
                        {
                            enemy.effects.Add(newStatus);
                        }
                        Hit(game, gameTime, enemy, (11 + damage) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                }
            }
            return false;
        }
    }

    class VoidEruption : Spell
    {
        public VoidEruption(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.cooldown = 3;
            myElement = element.dark;
        }

        public override void Instantiate(Game1 game)
        {
            spellTexture = game.Content.Load<Texture2D>("sprites/Characters/dark_ball");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, 10 + damage);
                        Hit(game, gameTime, enemy, 10 + damage);
                        Darkness newSpellCast = new Darkness(enemy.position - (direction * (game.playerRadius * 2f)), direction, 300, false);
                        newSpellCast.Instantiate(game);
                        game.spellsToSpawn.Add(newSpellCast);
                        return true;
                    }
                }
            }
            return base.Update(game, gameTime);

        }
    }

    #endregion

    #region Unique

    public class SummonedOBJ : GameObject
    {
        Spell mySpell;
        public SummonedOBJ(Vector2 _position, float _health, Spell mySpell) : base(_position, _health)
        {
            this.position = _position;
            this.speed = 48;
            this.baseSpeed = this.speed;
            this.health = _health;
            this.maxHealth = health;
            this.mySpell = mySpell;
        }

        public override void Initiate(Game1 game)
        {
            texture = game.Content.Load<Texture2D>("sprites/Characters/skull");
        }

        public virtual void Draw(Game1 game, SpriteBatch _spriteBatch)
        {
            Color enemyColor = Color.White;

            if (effects.Count > 0)
                enemyColor = effects.Last().statusColor;

            _spriteBatch.Draw(texture, position, enemyColor);

            Rectangle healthbar = new Rectangle(Point.Zero, new Point((int)(16 * (health / 20)), 8));

            _spriteBatch.Draw(game.boxTexture, position - (Vector2.UnitY * (game.playerRadius / 2)) - Vector2.UnitX * (healthbar.Size.X / 2) + Vector2.UnitX * (game.playerRadius * 1.5f), healthbar, Color.Green, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            if (health <= 0)
            {
                return true;
            }
            GameObject following = null;
            float closest = 99999;
            foreach (GameObject enemy in game.enemyList)
            {
                if (game.CircleCollision(position, game.playerRadius, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                {
                    enemy.Hit(game, (5 + mySpell.damage) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    health -= 3.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    mySpell.Hit(game, gameTime, enemy, (5 + mySpell.damage) * (float)gameTime.ElapsedGameTime.TotalSeconds);

                }
                if (Vector2.Distance(position, enemy.position) < closest)
                {
                    following = enemy;
                    closest = Vector2.Distance(position, enemy.position);
                }
            }
            if (following == null) return false;
            Vector2 direction = following.position - position;
            direction.Normalize();
            position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            return base.Update(game, gameTime);
        }
    }
    class Summon : Spell
    {
        List<SummonedOBJ> summonList = new List<SummonedOBJ>();

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
            SummonedOBJ newSummon = new SummonedOBJ(position, 10, this);
            newSummon.Initiate(game);
            summonList.Add(newSummon);
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            SummonedOBJ removed = null;
            foreach(SummonedOBJ summon in summonList)
            {
                if(summon.Update(game, gameTime)) removed = summon;
            }
            summonList.Remove(removed);
            return false;
        }

        public override void Draw(Game1 game, SpriteBatch _spriteBatch, GameTime gameTime)
        {
            foreach(SummonedOBJ summon in summonList)
            {
                summon.Draw(game, _spriteBatch);
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
            this.hitbox = 0;
        }

        public override void Draw(Game1 game, SpriteBatch _spriteBatch, GameTime gameTime)
        {
            
        }

        public override void Instantiate(Game1 game)
        {
            Vector2 direction = (game.playerPosition - game.CursorPosition) + Vector2.UnitX * game.offsetX;
            direction.Normalize();
            game.playerPosition -= direction * dashStrength;
            game.isMoving = false;
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            
            return true;
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
            this.hitbox = 0;
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
    class BanHammer : Spell
    {
        public BanHammer(Vector2 _position, Vector2 _direction, float _speed, bool _isEnemy) : base(_position, _direction, _speed, _isEnemy)
        {
            this.position = _position;
            this.direction = _direction;
            this.speed = _speed;
            this.isEnemy = _isEnemy;
            this.cooldown = 0;
            myElement = element.dark;
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            if (!isEnemy)
            {
                foreach (GameObject enemy in game.enemyList)
                {
                    if (game.CircleCollision(position, hitbox, (enemy.position + Vector2.One * enemy.hitBox), enemy.hitBox))
                    {
                        enemy.Hit(game, 999 + damage);
                        Hit(game, gameTime, enemy, 999 + damage);
                        return true;
                    }
                }
            }
            return base.Update(game, gameTime);

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

        public float reationCooldown = 0;

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
            if (reationCooldown > 0) reationCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            List<Spell> toRemove = new List<Spell>();
            foreach (Spell bullet in bullets)
            {
                if (bullet.Update(this, gameTime)) toRemove.Add(bullet);
            }
            foreach (Spell removed in toRemove)
            {
                if (removed != null)
                {
                    if (removed.GetType() == typeof(Turret))
                    {
                        turrets.Remove((Turret)removed);
                    }
                    if (removed.isInteractable)
                    {
                        interactableSpells.Remove(removed);
                    }
                    bullets.Remove(removed);
                }
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
