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

        void playerUpdate(GameTime gameTime)
        {
            if (invincibility > 0) invincibility -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (playerHealth <= 0) playing = false;
            Vector2 direction = Vector2.Zero;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                direction.Y = -1;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                direction.Y = 1;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                direction.X = -1;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                direction.X = 1;
            }

            if(direction != Vector2.Zero) 
            {
                direction.Normalize();
            }

            Vector2 futurePosition = playerPosition + (direction * playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);

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
                bullets.Add(newSpellCast);
                canCast = false;
                currentCooldowns[currentSpellIndex] = newSpellCast.cooldown;
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released) canCast = true;
        }

        
    }
}
