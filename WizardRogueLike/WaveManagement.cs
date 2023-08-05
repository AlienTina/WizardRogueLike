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
            enemiesLeft = (int)(5 + Math.Pow(gamePhase, 1.5f));
        }

        void UpdateWave(GameTime gameTime)
        {
            timeUntilNextEnemy -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(timeUntilNextEnemy <= 0 && enemiesLeft > 0)
            {
                Vector2 spawnPosition = generateRandomPosition();
                while(CircleCollision(playerPosition, 128, spawnPosition, playerRadius))
                {
                    spawnPosition = generateRandomPosition();
                }
                GameObject newEnemy = new GameObject(spawnPosition, enemyTexture, 64, 20);
                enemyList.Add(newEnemy);
                timeUntilNextEnemy = (float)Math.Clamp(rand.NextDouble() * (7.0f - (gamePhase * 0.5f)), 0.5f, 10.0f);
                enemiesLeft--;
                
            }
            if (enemiesLeft == 0)
            {
                if (enemyList.Count == 0)
                {
                    gamePhase++;
                    state = GameState.inbetween;
                    ChooseSpells();
                }
            }
        }
    }
}
