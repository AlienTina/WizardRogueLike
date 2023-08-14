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
        int enemiesLeft = 0;

        float timeUntilNextEnemy = 0;

        void StartWave()
        {
            //enemiesLeft = (int)(5 + Math.Pow(gamePhase, 1.5f));
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    if (dungeon[x, y] == ' ')
                    {
                        if (generator.rooms[x, y].myType == roomType.fight)
                            enemiesLeft += enemiesPerRoom;
                    }
                }
            }
        }

        public void WaveEnded()
        {
            gamePhase++;

            state = GameState.inbetween;
            ChooseSpells();
        }
    }
}
