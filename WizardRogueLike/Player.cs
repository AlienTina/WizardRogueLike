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

        public float playerHealth = 100;
        public float playerMaxHealth = 100;

        public Vector2 playerPosition = new Vector2(0, 0);
        public float playerRadius = 21;
        float truePlayerR = 0;
        float playerSpeed = 192;

        Vector2 staffPosition = new Vector2(0, 0);
        Vector2 staffOrigin = new Vector2(0, 0);
        float staffRotation = 0;

        public float invincibility = 0;

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
            else if (playerHealth > playerMaxHealth) playerHealth = playerMaxHealth;
            Vector2 direction = Vector2.Zero;
            
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                playerTarget = (CursorPosition - Vector2.UnitX * offsetX) - Vector2.One * 32;
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
            bool hasRoomChanged = false;
            for (int y = 0; y < gridSize.Y; y++)
            {
                for(int x = 0; x < gridSize.X; x++)
                {
                    int xCenter = (int)gridSize.X / 2;
                    int yCenter = (int)gridSize.Y / 2;

                    // Check if the player is moving out of the room through the wall
                    if (x == 0 && generator.rooms[(int)currentDungeonPosition.X, (int)currentDungeonPosition.Y].neighbours[2] == 0 && futurePosition.X < (tileSize / 4))
                    {
                        canMoveX = false;
                    }
                    else if (x == gridSize.X - 1 && generator.rooms[(int)currentDungeonPosition.X, (int)currentDungeonPosition.Y].neighbours[3] == 0 && futurePosition.X > _graphics.PreferredBackBufferWidth - (tileSize))
                    {
                        canMoveX = false;
                    }

                    if (y == 0 && generator.rooms[(int)currentDungeonPosition.X, (int)currentDungeonPosition.Y].neighbours[0] == 0 && futurePosition.Y < (tileSize / 4))
                    {
                        canMoveY = false;
                    }
                    else if (y == gridSize.Y - 1 && generator.rooms[(int)currentDungeonPosition.X, (int)currentDungeonPosition.Y].neighbours[1] == 0 && futurePosition.Y > areaSize.Y - (tileSize) - playerRadius)
                    {
                        canMoveY = false;
                    }

                    Vector2 newDungeongPosition = currentDungeonPosition;

                    // Handle room transitions
                    if (canMoveX && futurePosition.X < 0)
                    {
                        if (futurePosition.Y > ((yCenter - doorSize) * tileSize) && futurePosition.Y < ((yCenter + doorSize) * tileSize))
                        {
                            // Transition to the neighboring room on the left
                            newDungeongPosition.X -= 1;
                        }
                        else canMoveX = false;
                    }
                    else if (canMoveX && futurePosition.X > _graphics.PreferredBackBufferWidth - (playerRadius * 2))
                    {
                        if (futurePosition.Y > ((yCenter - doorSize) * tileSize)  && futurePosition.Y < ((yCenter + doorSize) * tileSize) )
                        {
                            // Transition to the neighboring room on the right
                            newDungeongPosition.X += 1;
                        }
                        else canMoveX = false;
                    }

                    if (canMoveY && futurePosition.Y < 0)
                    {
                        if (futurePosition.X > ((xCenter - doorSize) * tileSize)  && futurePosition.X < ((xCenter + doorSize) * tileSize))
                        {
                            // Transition to the neighboring room above
                            newDungeongPosition.Y -= 1;
                        }
                        else canMoveY = false;
                    }
                    else if (canMoveY && futurePosition.Y > areaSize.Y - (playerRadius * 3))
                    {
                        if (futurePosition.X > ((xCenter - doorSize) * tileSize) && futurePosition.X < ((xCenter + doorSize) * tileSize))
                        {
                            // Transition to the neighboring room below
                            newDungeongPosition.Y += 1;
                            
                            
                        }
                        else canMoveY = false;
                    }
                    
                    if(newDungeongPosition != currentDungeonPosition)
                    {
                        if (newDungeongPosition.X < gridWidth && newDungeongPosition.X >= 0 && newDungeongPosition.Y < gridHeight && newDungeongPosition.Y >= 0)
                        {
                            if (newDungeongPosition.Y - currentDungeonPosition.Y == 1) futurePosition.Y = (tileSize * 3);
                            else if (newDungeongPosition.Y - currentDungeonPosition.Y == -1) futurePosition.Y = _graphics.PreferredBackBufferHeight - (tileSize * 3);
                            if (newDungeongPosition.X - currentDungeonPosition.X == 1) futurePosition.X = (tileSize * 2);
                            else if (newDungeongPosition.X - currentDungeonPosition.X == -1) futurePosition.X = _graphics.PreferredBackBufferWidth - (tileSize * 2);
                            currentDungeonPosition = newDungeongPosition;
                            hasRoomChanged = true;
                        }
                    }
                }
            }

            if(canMoveX) playerPosition.X = futurePosition.X;
            if(canMoveY) playerPosition.Y = futurePosition.Y;
            if (hasRoomChanged) roomChanged();


        }

        void StaffUpdate()
        {
            Vector2 mouseState = CursorPosition - Vector2.UnitX * offsetX;
            Vector2 dPos = playerPosition - mouseState;
            staffRotation = (float)Math.Atan2(dPos.Y, dPos.X);

            staffPosition = playerPosition + (Vector2.One * truePlayerR);


            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Cast();
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released) canCast = true;
        }

        //int healthBarOffset = 0;

        void PlayerDraw()
        {
            
            Rectangle healthbar = new Rectangle(Point.Zero, new Point((int)(16 * (playerHealth / 30)), 8));
            if (invincibility <= 0)
            {
                //healthBarOffset = 0;
                _spriteBatch.Draw(playerTexture, playerPosition, Color.White);
                _spriteBatch.Draw(boxTexture, playerPosition - (Vector2.UnitY * (playerRadius / 2)) - Vector2.UnitX * (healthbar.Size.X / 2) + Vector2.UnitX * (playerRadius * 1.5f), healthbar, Color.Green, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
            else 
            {
                //if(blink > 0)
                    //healthBarOffset += rand.Next(-8, 8);
                _spriteBatch.Draw(playerTexture, playerPosition, Color.Gray);
                _spriteBatch.Draw(boxTexture, playerPosition - (Vector2.UnitY * (playerRadius / 2)) - Vector2.UnitX * (healthbar.Size.X / 2) + Vector2.UnitX * (playerRadius * 1.5f), healthbar, Color.DarkGreen, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                //healthBarOffset = 0;
            }



            if (isMoving)
                _spriteBatch.Draw(targetTexture, playerTarget, Color.White);
            _spriteBatch.Draw(staffTexture, staffPosition, null, Color.White, staffRotation, staffOrigin, Vector2.One, SpriteEffects.None, 0);
        }

        public void Cast()
        {
            if (canCast && currentSpell != null && currentCooldowns[currentSpellIndex] <= 0)
            {
                Vector2 direction = (playerPosition - CursorPosition) + Vector2.UnitX * offsetX;
                direction.Normalize();
                Spell newSpellCast = (Spell)Activator.CreateInstance(currentSpell, playerPosition - (direction * staffTexture.Width), -direction, 300, false);
                newSpellCast.Instantiate(this);
                newSpellCast.damage = spellDamageBonus[currentSpellIndex];
                bullets.Add(newSpellCast);
                if (currentSpell != typeof(Turret))
                {
                    for (int i = 0; i < turrets.Count; i++)
                    {
                        Vector2 directionTurret = (turrets[i].position - CursorPosition) + Vector2.UnitX * offsetX;
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
        }
        
    }
}
