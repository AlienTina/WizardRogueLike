using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WizardRogueLike
{
    partial class Game1
    {
        public GameState state = GameState.inbetween;
        public int gamePhase = 0;

        List<Type> chosingSpells = new List<Type>();

        public void BetweenInitialize()
        {
            SpellInstantiate();
            ChooseSpells();
        }

        public void BetweenUpdate()
        {
            bool chosen = false;
            int chosenSpell = 8;
            for (int i = 0; i < mySpells.Count; i++)
            {
                Debug.WriteLine("slot " + i.ToString() + " checked.");
                if (mySpells[i] == null)
                {
                    chosenSpell = i;
                    break;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                
                mySpells[chosenSpell] = chosingSpells[0];
                chosen = true;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {
                mySpells[chosenSpell] = chosingSpells[1];
                chosen = true;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D3))
            {
                mySpells[chosenSpell] = chosingSpells[2];
                chosen = true;
            }

            if (chosen)
            {
                
                UIInstantiate();
                StartWave();
                state = GameState.playing;
            }
        }

        public void BetweenDraw()
        {
            int index = -1;
            foreach(Type spell in chosingSpells)
            {
                if (spell == null) continue;
                _spriteBatch.DrawString(defaultfont, (index+2).ToString() + ": " + spell.Name, areaSize / 2 + (Vector2.UnitY * index * 32), Color.Black);

                index += 1;
            }
        }

        public void ChooseSpells()
        {
            chosingSpells = new List<Type>();
            if (gamePhase == 0)
            {
                chosingSpells.Add(typeof(Fireball));
                chosingSpells.Add(typeof(ToxicBall));
                chosingSpells.Add(typeof(IceBall));
            }
            else
            {
                chosingSpells.Add(allAvailableSpells[rand.Next(allAvailableSpells.Count)]);
                chosingSpells.Add(allAvailableSpells[rand.Next(allAvailableSpells.Count)]);
                chosingSpells.Add(allAvailableSpells[rand.Next(allAvailableSpells.Count)]);
            }
        }
    }
}
