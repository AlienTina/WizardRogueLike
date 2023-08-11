using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public GameObject(Vector2 _position, Texture2D _texture, float _speed, float _health)
        {
            this.position = _position;
            this.texture = _texture;
            this.speed = _speed;
            this.baseSpeed = _speed;
            this.health = _health;
            this.maxHealth = health;
        }

        public void Draw(Game1 game, SpriteBatch _spriteBatch)
        {
            Color enemyColor = Color.White;

            if (effects.Count > 0)
                enemyColor = effects.Last().statusColor;

            _spriteBatch.Draw(texture, position, enemyColor);

            Rectangle healthbar = new Rectangle(Point.Zero, new Point((int)(16 * (health / 20)), 8));

            _spriteBatch.Draw(game.boxTexture, position - (Vector2.UnitY * (game.playerRadius / 2)) -Vector2.UnitX * (healthbar.Size.X / 2) + Vector2.UnitX * (game.playerRadius * 1.5f), healthbar, Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }

        public void Hit(Game1 game, float damage)
        {
            health -= damage * defenseShred;
            foreach (StatusEffect effect in effects) effect.Hit(game, this);
        }
    }
    partial class Game1
    {
        Texture2D enemyTexture;

        public List<GameObject> enemyList = new List<GameObject>();

        bool canSpawn = true;

        void enemyUpdate(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.J) && canSpawn)
            {
                GameObject newEnemy = new GameObject(generateRandomPosition(), enemyTexture, 64, rand.Next(10, 40));
                enemyList.Add(newEnemy);
                canSpawn = false;
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.J)) canSpawn = true;

            GameObject removed = null;

            foreach (GameObject obj in enemyList)
            {
                if (obj.isTargetPlayer) obj.target = playerPosition;
                if (obj.health <= 0) removed = obj;
                Vector2 direction = obj.target - obj.position;
                direction.Normalize();
                Vector2 newPosition = obj.position + direction * obj.speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                bool canMove = true;
                foreach(Spell spell in interactableSpells)
                {
                    if (!spell.isObstacle) continue;
                    if (CircleCollision(newPosition, playerRadius, spell.position, spell.hitbox)) canMove = false;
                }
                if(canMove) 
                    obj.position = newPosition;
                else
                    obj.position -= direction * obj.speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (CircleCollision(playerPosition, playerRadius, obj.position, playerRadius))
                {
                    if (invincibility <= 0)
                    {
                        playerHealth -= 10 + (10 * obj.damageBonus);
                        invincibility = 0.75f;
                    }
                }

                StatusEffect removedEffect = null;

                foreach(StatusEffect effect in obj.effects)
                {
                    if(effect.Update(this, gameTime, obj)) removedEffect = effect;
                }
                if(removedEffect != null) obj.effects.Remove(removedEffect);
            }
            if (removed != null) enemyList.Remove(removed);
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
