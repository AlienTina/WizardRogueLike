using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardRogueLike
{
    partial class Game1
    {
        Vector2 minimapPosition = new Vector2(16, 16);
        void MinimapDraw()
        {
            Rectangle minimapBackground = new Rectangle((int)minimapPosition.X, (int)minimapPosition.Y, gridWidth * minimapGrid.Width, gridHeight * minimapGrid.Height);
            _spriteBatch.Draw(boxTexture, minimapBackground, Color.Gray);
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    if (dungeon[x, y] == ' ')
                    {
                        Vector2 currentPosition = new Vector2(x, y);
                        if (currentDungeonPosition == currentPosition)
                            _spriteBatch.Draw(minimapGrid, new Vector2(x, y) * minimapGrid.Width + minimapPosition, Color.Green);
                        else if ((enemiesSpawned[currentPosition] && enemiesInRooms[currentPosition].Count == 0) || (generator.rooms[x, y].myType == roomType.none))
                            _spriteBatch.Draw(minimapGrid, new Vector2(x, y) * minimapGrid.Width + minimapPosition, Color.Gray);
                        else if((generator.rooms[x, y].myType == roomType.item))
                            _spriteBatch.Draw(minimapGrid, new Vector2(x, y) * minimapGrid.Width + minimapPosition, Color.Gold);
                        else if ((generator.rooms[x, y].myType == roomType.boss))
                            _spriteBatch.Draw(minimapGrid, new Vector2(x, y) * minimapGrid.Width + minimapPosition, Color.Crimson);
                        else
                            _spriteBatch.Draw(minimapGrid, new Vector2(x, y) * minimapGrid.Width + minimapPosition, Color.White);
                    }
                }
            }
        }
    }
}
