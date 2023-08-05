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
        public List<StatusEffect> effects = new List<StatusEffect>();
        public float health = 20;
        public GameObject(Vector2 _position, Texture2D _texture, float _speed, float _health)
        {
            this.position = _position;
            this.texture = _texture;
            this.speed = _speed;
            this.health = _health;
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
                GameObject newEnemy = new GameObject(generateRandomPosition(), enemyTexture, 64, 20);
                enemyList.Add(newEnemy);
                canSpawn = false;
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.J)) canSpawn = true;

            GameObject removed = null;

            foreach (GameObject obj in enemyList)
            {
                if (obj.health <= 0) removed = obj;
                Vector2 direction = playerPosition - obj.position;
                direction.Normalize();
                obj.position += direction * obj.speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (CircleCollision(playerPosition, playerRadius, obj.position, playerRadius))
                {
                    if (invincibility <= 0)
                    {
                        playerHealth -= 10;
                        invincibility = 0.75f;
                    }
                }

                StatusEffect removedEffect = null;

                foreach(StatusEffect effect in obj.effects)
                {
                    if(effect.Update(gameTime, obj)) removedEffect = effect;
                }
                if(removedEffect != null) obj.effects.Remove(removedEffect);
            }
            if (removed != null) enemyList.Remove(removed);
        }

        void enemyDraw()
        {
            foreach(GameObject obj in enemyList)
            {
                Color enemyColor = Color.Black;
                foreach(StatusEffect effect in obj.effects)
                {
                    enemyColor = new Color(enemyColor.R + effect.statusColor.R, enemyColor.G + effect.statusColor.G, enemyColor.B + effect.statusColor.B);
                }
                if (enemyColor == Color.Black) enemyColor = Color.White;
                _spriteBatch.Draw(obj.texture, obj.position, enemyColor);
            }
        }
    }
}
