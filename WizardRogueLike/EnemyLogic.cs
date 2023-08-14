using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WizardRogueLike
{
    public class GameObject
    {
        public Vector2 position { get; set; }
        public Texture2D texture { get; set; }
        public float speed { get; set; }
        public float baseSpeed { get; set; }
        public List<StatusEffect> effects = new List<StatusEffect>();
        public float health = 20;
        public float maxHealth = 20;
        public float defenseShred = 1;
        public float damageBonus = 0;

        public Vector2 target;
        public bool isTargetPlayer = true;

        public float hitBox = 21;
        public GameObject(Vector2 _position, float _health)
        {
            this.position = _position;
            this.speed = 64;
            this.baseSpeed = this.speed;
            this.health = _health;
            this.maxHealth = health;
        }

        public virtual void Initiate(Game1 game)
        {
            texture = game.Content.Load<Texture2D>("sprites/Characters/red_character");
        }

        public virtual void Draw(Game1 game, SpriteBatch _spriteBatch)
        {
            Color enemyColor = Color.White;

            if (effects.Count > 0)
                enemyColor = effects.Last().statusColor;

            _spriteBatch.Draw(texture, position, enemyColor);

            Rectangle healthbar = new Rectangle(Point.Zero, new Point((int)(16 * (health / 20)), 8));

            _spriteBatch.Draw(game.boxTexture, position - (Vector2.UnitY * (game.playerRadius / 2)) -Vector2.UnitX * (healthbar.Size.X / 2) + Vector2.UnitX * (game.playerRadius * 1.5f), healthbar, Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }

        public virtual bool Update(Game1 game, GameTime gameTime)
        {
            if (isTargetPlayer) target = game.playerPosition;
            if (health <= 0) return true;
            

            StatusEffect removedEffect = null;

            foreach (StatusEffect effect in effects)
            {
                if (effect.Update(game, gameTime, this)) removedEffect = effect;
            }
            if (removedEffect != null) effects.Remove(removedEffect);
            return false;
        }

        public virtual void Hit(Game1 game, float damage)
        {
            health -= damage * defenseShred;
            foreach (StatusEffect effect in effects) effect.Hit(game, this);
        }
    }
    public class Slime : GameObject
    {
        public Slime(Vector2 _position, float _health) : base(_position, _health)
        {
            this.position = _position;
            this.speed = 64;
            this.baseSpeed = this.speed;
            this.health = _health;
            this.maxHealth = health;
        }

        public override void Initiate(Game1 game)
        {
            texture = game.Content.Load<Texture2D>("sprites/Characters/Slime");
        }

        public override bool Update(Game1 game, GameTime gameTime)
        {
            Vector2 direction = target - position;
            direction.Normalize();
            Vector2 newPosition = position + direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            bool canMove = true;
            foreach (Spell spell in game.interactableSpells)
            {
                if (!spell.isObstacle) continue;
                if (game.CircleCollision(newPosition, hitBox, spell.position, spell.hitbox)) canMove = false;
            }
            if (canMove)
                position = newPosition;
            else
                position -= direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (game.CircleCollision(game.playerPosition, game.playerRadius, position, hitBox))
            {
                if (game.invincibility <= 0)
                {
                    game.playerHealth -= 10 + (10 * damageBonus);
                    game.invincibility = 0.75f;
                    //game.blink = 0.1f;
                }
            }
            return base.Update(game, gameTime);
        }
    }

    public class TestBoss : GameObject
    {
        float attackTime = 0;
        float nextAttack = 1;
        int currentAttack = -1;
        Random random = new Random();
        Texture2D attackIndicator;

        bool showAttack = false;
        Vector2 attackPosition = Vector2.Zero;
        public TestBoss(Vector2 _position, float _health) : base(_position, _health)
        {
            this.position = _position;
            this.speed = 32;
            this.baseSpeed = this.speed;
            this.health = _health;
            this.maxHealth = health;
            this.hitBox = 56;
        }

        public override void Initiate(Game1 game)
        {
            texture = game.Content.Load<Texture2D>("sprites/Characters/test_boss");
            attackIndicator = game.Content.Load<Texture2D>("sprites/Characters/boss_attack");
        }

        public override void Draw(Game1 game, SpriteBatch _spriteBatch)
        {
            if (showAttack)
            {
                _spriteBatch.Draw(attackIndicator, attackPosition, Color.White);
            }
            Color enemyColor = Color.White;

            if (effects.Count > 0)
                enemyColor = effects.Last().statusColor;

            _spriteBatch.Draw(texture, position + offset, enemyColor);

            Rectangle healthbar = new Rectangle(Point.Zero, new Point((int)(16 * (health / 20)), 8));

            _spriteBatch.Draw(game.boxTexture, (position - (Vector2.UnitY * (hitBox)) - Vector2.UnitX * (healthbar.Size.X / 2) + Vector2.UnitX * (hitBox * 1.5f)), healthbar, Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            
        }

        Vector2 offset = Vector2.Zero;

        bool moved = false;

        public override bool Update(Game1 game, GameTime gameTime)
        {
            Vector2 direction = target - position;
            direction.Normalize();
            Vector2 newPosition = position + direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            bool canMove = true;
            if (nextAttack > 0) nextAttack -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (nextAttack < 0)
            {
                nextAttack = random.Next(1, 10);
                currentAttack = random.Next(0, 2);
                if(currentAttack == 0)
                    attackTime = 1;
                else if (currentAttack == 1)
                    attackTime = 3;
                showAttack = true;
                attackPosition = target;
            }
            if (attackTime > 0)
            {
                attackTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (currentAttack == 0)
                {
                    isTargetPlayer = false;
                    speed = baseSpeed * 15;
                }
                if(currentAttack == 1)
                {
                    isTargetPlayer = false;
                    if(attackTime > 2)
                    {
                        offset.Y -= (float)gameTime.ElapsedGameTime.TotalSeconds * 1200;
                    }
                    else
                    {
                        if (!moved)
                        {
                            offset = Vector2.Zero;
                            speed = baseSpeed * 25;
                            newPosition = new Vector2(target.X, -hitBox);
                            moved = true;
                        }
                    }
                }
            }
            else if (attackTime < 0)
            {
                showAttack = false;
                if (currentAttack == 0)
                {
                    isTargetPlayer = true;
                    speed = baseSpeed;
                }
                else if(currentAttack == 1)
                {
                    isTargetPlayer = true;
                    speed = baseSpeed;
                    moved = false;
                    offset = Vector2.Zero;
                }
            }
            foreach (Spell spell in game.interactableSpells)
            {
                if (!spell.isObstacle) continue;
                if (game.CircleCollision(newPosition, hitBox, spell.position, spell.hitbox)) canMove = false;
            }
            if (canMove)
                position = newPosition;
            else
                position -= direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (game.CircleCollision(game.playerPosition, game.playerRadius, position, hitBox))
            {
                if (game.invincibility <= 0)
                {
                    game.playerHealth -= 10 + (10 * damageBonus);
                    game.invincibility = 0.75f;
                    //game.blink = 0.1f;
                }
            }
            if (isTargetPlayer) target = game.playerPosition;
            if (health <= 0) 
            {
                game.WaveEnded();
                return true; 
            }


            StatusEffect removedEffect = null;

            foreach (StatusEffect effect in effects)
            {
                if (effect.Update(game, gameTime, this)) removedEffect = effect;
            }
            if (removedEffect != null) effects.Remove(removedEffect);
            return false;
        }

        public override void Hit(Game1 game, float damage)
        {
            if (currentAttack != 1 || attackTime < 2)
            {
                health -= damage * defenseShred;
                foreach (StatusEffect effect in effects) effect.Hit(game, this);
            }
        }
    }
    partial class Game1
    {
        Texture2D enemyTexture;

        public Dictionary<Vector2, List<GameObject>> enemiesInRooms = new Dictionary<Vector2, List<GameObject>>();
        public Dictionary<Vector2, bool> enemiesSpawned = new Dictionary<Vector2, bool>();

        public List<GameObject> enemyList = new List<GameObject>();

        bool canSpawn = true;

        int enemiesPerRoom = 3;

        void enemyInitiate()
        {
            enemiesPerRoom = rand.Next(3, 3 + gamePhase);
            enemiesInRooms = new Dictionary<Vector2, List<GameObject>>();
            enemiesSpawned = new Dictionary<Vector2, bool>();
            for (int y = 0; y < gridHeight; y++)
            {
                for(int x = 0; x < gridWidth; x++)
                {
                    if (dungeon[x, y] == ' ')
                    {
                        enemiesInRooms.Add(new Vector2(x, y), new List<GameObject>());
                        enemiesSpawned.Add(new Vector2(x, y), false);
                    }
                }
            }
        }


        void roomChanged()
        {
            enemyList = new List<GameObject>();
            if (enemiesSpawned[currentDungeonPosition] == false)
            {
                if (generator.rooms[(int)currentDungeonPosition.X, (int)currentDungeonPosition.Y].myType == roomType.fight)
                {
                    for (int i = 0; i < enemiesPerRoom; i++)
                    {
                        Slime newEnemy = new Slime(generateRandomPosition(), rand.Next(5, 5 + (5 * gamePhase)));
                        newEnemy.Initiate(this);
                        enemiesInRooms[currentDungeonPosition].Add(newEnemy);
                        enemyList.Add(newEnemy);

                    }
                    enemiesSpawned[currentDungeonPosition] = true;
                }
                if (generator.rooms[(int)currentDungeonPosition.X, (int)currentDungeonPosition.Y].myType == roomType.boss && enemiesLeft <= 0)
                {
                    TestBoss newEnemy = new TestBoss(generateRandomPosition(), rand.Next(30, 30 + (10 * gamePhase)));
                    newEnemy.Initiate(this);
                    enemiesInRooms[currentDungeonPosition].Add(newEnemy);
                    enemyList.Add(newEnemy);
                    enemiesSpawned[currentDungeonPosition] = true;
                }
            }
            else
            {
                for (int i = 0; i < enemiesInRooms[currentDungeonPosition].Count; i++)
                {
                    enemyList.Add(enemiesInRooms[currentDungeonPosition][i]);
                }
            }
        }

        void enemyUpdate(GameTime gameTime)
        {
            enemiesInRooms[currentDungeonPosition] = enemyList;
            if (Keyboard.GetState().IsKeyDown(Keys.J) && canSpawn)
            {
                GameObject newEnemy = new GameObject(generateRandomPosition(), rand.Next(10, 40));
                newEnemy.Initiate(this);
                enemyList.Add(newEnemy);
                canSpawn = false;
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.J)) canSpawn = true;

            GameObject removed = null;

            foreach (GameObject obj in enemyList)
            {
                if(obj.Update(this, gameTime)) removed = obj;
            }
            if (removed != null)
            {
                enemyList.Remove(removed);
                enemiesLeft--;
            }
        }

        void enemyDraw()
        {
            foreach(GameObject obj in enemyList)
            {
                obj.Draw(this, _spriteBatch);
            }
        }
    }
}
