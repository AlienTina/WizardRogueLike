using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace WizardRogueLike
{
    partial class Game1
    {
        
        Texture2D leftWall;
        Texture2D rightWall;
        Texture2D upWall;
        Texture2D downWall;

        Texture2D leftdowncorner;
        Texture2D leftupcorner;
        Texture2D rightdowncorner;
        Texture2D rightupcorner;

        int doorSize = 2;

        void Leveldraw()
        {
            for (int y = 0; y < gridSize.Y; y++)
            {
                for (int x = 0; x < gridSize.X; x++)
                {
                    bool isleft = false;
                    bool isright = false;
                    bool isup = false;
                    bool isdown = false;

                    bool hasDoor = false;


                    int xCenter = (int)gridSize.X / 2;
                    int yCenter = (int)gridSize.Y / 2;

                    /*if (x == 0 || x == gridSize.X - 1 || y == 0 || y == gridSize.Y - 1)
                    {
                        _spriteBatch.Draw(wallTile, new Vector2(x * tileSize, y * tileSize), Color.White);
                    }
                    else
                    {
                        _spriteBatch.Draw(tile, new Vector2(x * tileSize, y * tileSize), Color.White);
                    }*/
                    bool isWall = false;
                    if(x == 0)
                    {
                        isWall = true;
                        isleft = true;
                    }
                    else if(x == gridSize.X - 1)
                    {
                        isWall = true;
                        isright = true;
                    }
                    if(y == 0)
                    {
                        isWall = true;
                        isup = true;
                    }
                    else if (y == gridSize.Y - 1)
                    {
                        isWall = true;
                        isdown = true;
                    }

                    if (isleft && generator.rooms[(int)currentDungeonPosition.X, (int)currentDungeonPosition.Y].neighbours[2] == 1 && (y > yCenter - doorSize && y < yCenter + doorSize))
                    {
                        if(generator.rooms[(int)currentDungeonPosition.X-1, (int)currentDungeonPosition.Y] != null)
                        {
                            hasDoor = true;
                        }
                    }
                    else if (isright && generator.rooms[(int)currentDungeonPosition.X, (int)currentDungeonPosition.Y].neighbours[3] == 1 && (y > yCenter - doorSize && y < yCenter + doorSize))
                    {
                        if (generator.rooms[(int)currentDungeonPosition.X + 1, (int)currentDungeonPosition.Y] != null)
                        {
                            hasDoor = true;
                        }
                    }

                    if (isup && generator.rooms[(int)currentDungeonPosition.X, (int)currentDungeonPosition.Y].neighbours[0] == 1 && (x > xCenter - doorSize && x < xCenter + doorSize))
                    {
                        if (generator.rooms[(int)currentDungeonPosition.X, (int)currentDungeonPosition.Y - 1] != null)
                        {
                            hasDoor = true;
                        }
                    }
                    else if (isdown && generator.rooms[(int)currentDungeonPosition.X, (int)currentDungeonPosition.Y].neighbours[1] == 1 && (x > xCenter - doorSize && x < xCenter + doorSize))
                    {
                        if (generator.rooms[(int)currentDungeonPosition.X, (int)currentDungeonPosition.Y + 1] != null)
                        {
                            hasDoor = true;
                        }
                    }

                    if (hasDoor) { isWall = false; }

                    if (isWall)
                    {
                        if(isleft && isdown)
                            _spriteBatch.Draw(leftdowncorner, new Vector2(x * tileSize, y * tileSize), Color.White);
                        else if (isleft && isup)
                            _spriteBatch.Draw(leftupcorner, new Vector2(x * tileSize, y * tileSize), Color.White);
                        else if (isright && isup)
                            _spriteBatch.Draw(rightupcorner, new Vector2(x * tileSize, y * tileSize), Color.White);
                        else if (isright && isdown)
                            _spriteBatch.Draw(rightdowncorner, new Vector2(x * tileSize, y * tileSize), Color.White);
                        else if (isleft)
                            _spriteBatch.Draw(leftWall, new Vector2(x * tileSize, y * tileSize), Color.White);
                        else if (isright)
                            _spriteBatch.Draw(rightWall, new Vector2(x * tileSize, y * tileSize), Color.White);
                        else if (isup)
                            _spriteBatch.Draw(upWall, new Vector2(x * tileSize, y * tileSize), Color.White);
                        else if (isdown)
                            _spriteBatch.Draw(downWall, new Vector2(x * tileSize, y * tileSize), Color.White);
                    }
                    else
                    {
                        //_spriteBatch.Draw(tile, new Vector2(x * tileSize, y * tileSize), Color.White);
                    }
                }
            }
        }

        public Vector2 generateRandomPosition()
        {
            Vector2 randomPosition = new Vector2(rand.Next((int)tileSize, (int)areaSize.X - 128),
                rand.Next((int)tileSize, (int)areaSize.Y - 128));

            while (Vector2.Distance(playerPosition, randomPosition) <= 256)
            {
                Debug.WriteLine(Vector2.Distance(playerPosition, randomPosition));
                randomPosition = new Vector2(rand.Next((int)tileSize, (int)areaSize.X - 128),
                rand.Next((int)tileSize, (int)areaSize.Y - 128));
            }
            return randomPosition;
        }
    }
}
