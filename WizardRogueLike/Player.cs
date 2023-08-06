using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace WizardRogueLike
{
    partial class Game1
    {
        Texture2D playerTexture;
        public Texture2D staffTexture;
        Texture2D targetTexture;
        
        public List<Spell> bullets = new List<Spell>();

        float playerHealth = 100;

        public Vector2 playerPosition = new Vector2(0, 0);
        public float playerRadius = 21;
        float truePlayerR = 0;
        float playerSpeed = 192;

        Vector2 staffPosition = new Vector2(0, 0);
        Vector2 staffOrigin = new Vector2(0, 0);
        float staffRotation = 0;

        float invincibility = 0;

        bool canCast = true;

        public bool isMoving = false;

        Vector2 playerTarget = new Vector2(0, 0);

        void PlayerInstantiate()
        {
            playerHealth = 100;
            bullets = new List<Spell>();
            playerPosition = new Vector2(areaSize.X / 2, areaSize.Y / 2);
            invincibility = 0;
            enemyList = new List<GameObject>();
            playerTarget = playerPosition;
        }

        void playerUpdate(GameTime gameTime)
        {
            if (invincibility > 0) invincibility -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (playerHealth <= 0) state = GameState.ended;
            Vector2 direction = Vector2.Zero;
            
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                playerTarget = (Mouse.GetState().Position.ToVector2() - Vector2.UnitX * offsetX) - Vector2.One * 32;
                isMoving = true;
            }

            if(Vector2.Distance(playerPosition, playerTarget) > 2 && isMoving)
            {
                Vector2 dPos = playerPosition - playerTarget;
                direction = dPos;
                
            }
            else
            {
                isMoving = false;
            }

            if(direction != Vector2.Zero) 
            {
                direction.Normalize();
            }

            Vector2 futurePosition = playerPosition + (-direction * playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);

            bool canMoveX = true;
            bool canMoveY = true;

            for (int y = 0; y < gridSize.Y; y++)
            {
                for(int x = 0; x < gridSize.X; x++)
                {
                    if (x == 0 || x == gridSize.X - 1)
                    {
                        if (CircleOnBox(new Vector2(futurePosition.X, 0), playerRadius, new Vector2(x * tileSize, y * tileSize), Vector2.One * tileSize))
                        {
                            canMoveX = false;
                        }
                    }
                    if (y == 0 || y == gridSize.Y - 1)
                    {
                        if (CircleOnBox(new Vector2(0, futurePosition.Y), playerRadius, new Vector2(x * tileSize, y * tileSize), Vector2.One * tileSize))
                        {
                            canMoveY = false;
                        }
                    }
                }
            }

            if(canMoveX) playerPosition.X = futurePosition.X;
            if(canMoveY) playerPosition.Y = futurePosition.Y;



        }

        void StaffUpdate()
        {
            Vector2 mouseState = Mouse.GetState().Position.ToVector2() - Vector2.UnitX * offsetX;
            Vector2 dPos = playerPosition - mouseState;
            staffRotation = (float)Math.Atan2(dPos.Y, dPos.X);

            staffPosition = playerPosition + (Vector2.One * truePlayerR);


            if (Mouse.GetState().LeftButton == ButtonState.Pressed && canCast && currentSpell != null && currentCooldowns[currentSpellIndex] <= 0)
            {

                Vector2 direction = (playerPosition - Mouse.GetState().Position.ToVector2()) + Vector2.UnitX * offsetX;
                direction.Normalize();
                Spell newSpellCast = (Spell)Activator.CreateInstance(currentSpell, playerPosition - (direction * staffTexture.Width), -direction, 300, false);
                newSpellCast.Instantiate(this);
                newSpellCast.damage = spellDamageBonus[currentSpellIndex];
                bullets.Add(newSpellCast);
                if (currentSpell != typeof(Turret))
                {
                    for (int i = 0; i < turrets.Count; i++)
                    {
                        Vector2 directionTurret = (turrets[i].position - Mouse.GetState().Position.ToVector2()) + Vector2.UnitX * offsetX;
                        directionTurret.Normalize();
                        Spell newSpellCastTurret = (Spell)Activator.CreateInstance(currentSpell, turrets[i].position - (directionTurret * staffTexture.Width), -directionTurret, 300, false);
                        newSpellCastTurret.Instantiate(this);
                        newSpellCastTurret.damage = spellDamageBonus[currentSpellIndex];
                        bullets.Add(newSpellCastTurret);
                    }
                }
                canCast = false;
                currentCooldowns[currentSpellIndex] = newSpellCast.cooldown;
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released) canCast = true;
        }

        void PlayerDraw()
        {
            if (invincibility <= 0) _spriteBatch.Draw(playerTexture, playerPosition, Color.White);
            else _spriteBatch.Draw(playerTexture, playerPosition, Color.Gray);
            if(isMoving)
                _spriteBatch.Draw(targetTexture, playerTarget, Color.White);
            _spriteBatch.Draw(staffTexture, staffPosition, null, Color.White, staffRotation, staffOrigin, Vector2.One, SpriteEffects.None, 0);
        }
        
    }
}
