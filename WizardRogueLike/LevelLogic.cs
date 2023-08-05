using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WizardRogueLike
{
    partial class Game1
    {
        Texture2D tile;
        Texture2D wallTile;

        void Leveldraw()
        {
            for (int y = 0; y < gridSize.Y; y++)
            {
                for (int x = 0; x < gridSize.X; x++)
                {
                    if (x == 0 || x == gridSize.X - 1 || y == 0 || y == gridSize.Y - 1)
                    {
                        _spriteBatch.Draw(wallTile, new Vector2(x * tileSize, y * tileSize), Color.White);
                    }
                    else
                    {
                        _spriteBatch.Draw(tile, new Vector2(x * tileSize, y * tileSize), Color.White);
                    }
                }
            }
        }

        Vector2 generateRandomPosition()
        {
            return new Vector2(rand.Next((int)tileSize, (int)areaSize.X - 128), 
                rand.Next((int)tileSize, (int)areaSize.Y - 128));
        }
    }
}
