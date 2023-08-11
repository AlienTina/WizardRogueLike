using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace WizardRogueLike
{

    partial class Game1
    {
        public int gamePhase = 0;

        bool chosingUpgrades = false;
        bool chosingTransform = false;

        List<Type> chosingSpells = new List<Type>();
        Dictionary<Type, Type> spellTransforms = new Dictionary<Type, Type>();

        List<int> choicesUpgrades = new List<int>();

        public void BetweenInitialize()
        {
            spellTransforms = new Dictionary<Type, Type>();
            chosingUpgrades = false;
            SpellInstantiate();
            ChooseSpells();
            spellTransforms.Add(typeof(FireBall), typeof(InfernoOrb));
            spellTransforms.Add(typeof(VenomDart), typeof(ToxicBarrage));
            spellTransforms.Add(typeof(FrostShard), typeof(GlacialBurst));
            spellTransforms.Add(typeof(ShockBolt), typeof(Thunderstrike));
            spellTransforms.Add(typeof(AquaJet), typeof(TorrentialWave));
            spellTransforms.Add(typeof(AirBlast), typeof(CycloneWhirl));
            spellTransforms.Add(typeof(EarthSpike), typeof(QuakeTremor));
            spellTransforms.Add(typeof(ShadowBolt), typeof(VoidEruption));
            spellTransforms.Add(typeof(Summon), typeof(Turret));
        }

        public void BetweenUpdate()
        {
            bool chosen = false;
            int chosenSpell = 4;
            for (int i = 0; i < mySpells.Count; i++)
            {
                if (mySpells[i] == null)
                {
                    chosenSpell = i;
                    break;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                if (!chosingUpgrades && !chosingTransform)
                {
                    mySpells[chosenSpell] = chosingSpells[0];
                    chosen = true;
                }
                else if (chosingUpgrades)
                {
                    spellDamageBonus[choicesUpgrades[0]]++;
                    chosen = true;
                }
                else if (chosingTransform)
                {
                    if (spellTransforms.ContainsKey(mySpells[0]))
                    {
                        mySpells[0] = spellTransforms[mySpells[0]];
                        chosen = true;
                    }
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {
                if (!chosingUpgrades && !chosingTransform)
                {
                    mySpells[chosenSpell] = chosingSpells[1];
                    chosen = true;
                }
                else if (chosingUpgrades)
                {
                    spellDamageBonus[choicesUpgrades[1]]++;
                    chosen = true;
                }
                else if (chosingTransform)
                {
                    if (spellTransforms.ContainsKey(mySpells[1]))
                    {
                        mySpells[1] = spellTransforms[mySpells[1]];
                        chosen = true;
                    }
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D3))
            {
                if (!chosingUpgrades && !chosingTransform)
                {
                    mySpells[chosenSpell] = chosingSpells[2];
                    chosen = true;
                }
                else if (chosingUpgrades)
                {
                    spellDamageBonus[choicesUpgrades[2]]++;
                    chosen = true;
                }
                else if (chosingTransform)
                {
                    if (spellTransforms.ContainsKey(mySpells[2]))
                    {
                        mySpells[2] = spellTransforms[mySpells[2]];
                        chosen = true;
                    }
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D4))
            {
                if (!chosingUpgrades && !chosingTransform)
                {
                    if (chosingSpells.ElementAtOrDefault(3) != null)
                    {
                        mySpells[chosenSpell] = chosingSpells[3];
                        chosen = true;
                    }
                }
                else if (chosingUpgrades)
                {
                    spellDamageBonus[choicesUpgrades[3]]++;
                    chosen = true;
                }
                else if (chosingTransform)
                {
                    if (spellTransforms.ContainsKey(mySpells[3]))
                    {
                        mySpells[3] = spellTransforms[mySpells[3]];
                        chosen = true;
                    }
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D5))
            {

                if (!chosingUpgrades && !chosingTransform)
                {
                    if (chosingSpells.ElementAtOrDefault(4) != null)
                    {
                        mySpells[chosenSpell] = chosingSpells[4];
                        chosen = true;
                    }
                }
                else if (chosingUpgrades)
                {
                    spellDamageBonus[choicesUpgrades[4]]++;
                    chosen = true;
                }
                else if (chosingTransform)
                {
                    if (spellTransforms.ContainsKey(mySpells[4]))
                    {
                        mySpells[4] = spellTransforms[mySpells[4]];
                        chosen = true;
                    }
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D6))
            {

                if (!chosingUpgrades && !chosingTransform)
                {
                    if (chosingSpells.ElementAtOrDefault(5) != null)
                    {
                        mySpells[chosenSpell] = chosingSpells[5];
                        chosen = true;
                    }
                }
                else if (chosingUpgrades)
                {
                    spellDamageBonus[choicesUpgrades[5]]++;
                    chosen = true;
                }

            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D7))
            {

                if (!chosingUpgrades && !chosingTransform)
                {
                    if (chosingSpells.ElementAtOrDefault(6) != null)
                    {
                        mySpells[chosenSpell] = chosingSpells[6];
                        chosen = true;
                    }
                }
                else if (chosingUpgrades)
                {
                    spellDamageBonus[choicesUpgrades[6]]++;
                    chosen = true;
                }

            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D8))
            {

                if (!chosingUpgrades && !chosingTransform)
                {
                    if (chosingSpells.ElementAtOrDefault(7) != null)
                    {
                        mySpells[chosenSpell] = chosingSpells[7];
                        chosen = true;
                    }
                }
                else if (chosingUpgrades)
                {
                    spellDamageBonus[choicesUpgrades[7]]++;
                    chosen = true;
                }

            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D9))
            {

                if (!chosingUpgrades && !chosingTransform)
                {
                    if (chosingSpells.ElementAtOrDefault(8) != null)
                    {
                        mySpells[chosenSpell] = chosingSpells[8];
                        chosen = true;
                    }
                }
                else if (chosingUpgrades)
                {
                    spellDamageBonus[choicesUpgrades[8]]++;
                    chosen = true;
                }

            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D0))
            {
                if (chosingTransform)
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
            float offsetYText = 64;
            if (!chosingUpgrades && !chosingTransform)
            {
                int index = -1;
                foreach (Type spell in chosingSpells)
                {
                    if (spell == null) continue;
                    _spriteBatch.DrawString(defaultfont, (index + 2).ToString() + ": " + spell.Name, areaSize / 2 + (Vector2.UnitY * ((index * 32) - offsetYText)), Color.White);

                    index += 1;
                }
            }
            else if (chosingUpgrades)
            {
                int index = -1;

                foreach (int i in choicesUpgrades)
                {
                    _spriteBatch.DrawString(defaultfont, (index + 2).ToString() + ": " + mySpells[i].Name + "( " + (i+1).ToString() + ")" + " +1 damage", areaSize / 2 + (Vector2.UnitY * ((index * 32) - offsetYText)), Color.White);
                    index += 1;
                }
            }
            else if (chosingTransform)
            {
                for(int i = 0; i < 5;  i++)
                {
                    if (spellTransforms.ContainsKey(mySpells[i]))
                    {
                        _spriteBatch.DrawString(defaultfont, (i + 1).ToString() + ": " + mySpells[i].Name + "( " + (i + 1).ToString() + ")" + " -> " + spellTransforms[mySpells[i]].Name, areaSize / 2 + (Vector2.UnitY * ((i * 32) - offsetYText)), Color.White);
                    }
                }
                _spriteBatch.DrawString(defaultfont, "0: Skip", areaSize / 2 + (Vector2.UnitY * 6 * 32), Color.White);
            }
        }

        public void ChooseSpells()
        {
            chosingSpells = new List<Type>();
            choicesUpgrades = new List<int>();
            if (gamePhase == 0)
            {
                
                chosingSpells.Add(typeof(FireBall));
                chosingSpells.Add(typeof(VenomDart));
                chosingSpells.Add(typeof(FrostShard));
                chosingSpells.Add(typeof(ShockBolt));
                chosingSpells.Add(typeof(AquaJet));
                chosingSpells.Add(typeof(AirBlast));
                chosingSpells.Add(typeof(VineSnare));
                chosingSpells.Add(typeof(EarthSpike));
                chosingSpells.Add(typeof(ShadowBolt));
            }
            else if (gamePhase < 5)
            {

                chosingSpells.Add(allAvailableSpells[rand.Next(allAvailableSpells.Count)]);
                chosingSpells.Add(allAvailableSpells[rand.Next(allAvailableSpells.Count)]);
                chosingSpells.Add(allAvailableSpells[rand.Next(allAvailableSpells.Count)]);
            }
            else if (gamePhase < 10)
            {
                chosingTransform = true;
                /*chosingSpells.Add(allAvailableSpells[rand.Next(allAvailableSpells.Count)]);
                chosingSpells.Add(allAvailableSpells[rand.Next(allAvailableSpells.Count)]);
                chosingSpells.Add(allAvailableSpells[rand.Next(allAvailableSpells.Count)]);*/

            }
            else
            {
                chosingUpgrades = true;
                choicesUpgrades.Add(rand.Next(0, 4));
                choicesUpgrades.Add(rand.Next(0, 4));
                choicesUpgrades.Add(rand.Next(0, 4));
            }
        }
    }
}
