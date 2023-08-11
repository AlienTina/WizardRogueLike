using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace WizardRogueLike
{
    public class SpellIcon
    {
        public Type mySpell { get; set; }

        public SpellIcon(Type _mySpell)
        {
            this.mySpell = _mySpell;
        }

        public void Draw(Game1 game, SpriteBatch _spriteBatch, int index, bool selected)
        {
            Vector2 iconPosition = new Vector2((255 * index), game.areaSize.Y);
            if (mySpell != null && game.mySpells[index] != null)
            {
                float cooldown = (float)Math.Round(game.currentCooldowns[index], 2);
                Spell newSpellCast = (Spell)Activator.CreateInstance(mySpell, Vector2.Zero, Vector2.Zero, 0, false);
                float maxCooldown = newSpellCast.cooldown;
                Rectangle cooldownRect = new Rectangle(iconPosition.ToPoint(), new Point((int)(((cooldown) / maxCooldown) * 256), 64));
                _spriteBatch.Draw(game.boxTexture, cooldownRect, Color.Gray);
            }
            _spriteBatch.Draw(game.spellIconTexture, iconPosition, Color.White);
            if (mySpell != null)
            {

                //Vector2 cooldownSize = game.defaultfont.MeasureString(game.currentCooldowns[index].ToString());
                //Vector2 cooldownPosition = iconPosition + new Vector2(300, 0);
                String spellName = mySpell.Name;
                if (game.spellDamageBonus[index] > 0)
                {
                    spellName = mySpell.Name + " +" + game.spellDamageBonus[index].ToString();
                }
                if (!selected)
                    _spriteBatch.DrawString(game.defaultfont, spellName, iconPosition + (Vector2.One * 3), Color.White);
                else
                    _spriteBatch.DrawString(game.boldfont, spellName, iconPosition + (Vector2.One * 3), Color.LimeGreen);
                //_spriteBatch.DrawString(game.defaultfont, Math.Round(game.currentCooldowns[index], 2).ToString(), cooldownPosition, Color.Black, 0, new Vector2(cooldownSize.X, 0), 1, SpriteEffects.None, 0);

            }

        }
    }
    partial class Game1
    {
        public Texture2D spellIconTexture;
        public Texture2D boxTexture;

        public List<SpellIcon> spellIcons = new List<SpellIcon>();

        int oldScrollValue = 0;

        bool quickcast = false;


        void UIInstantiate()
        {
            spellIcons = new List<SpellIcon>();
            foreach (Type spell in mySpells)
            {
                spellIcons.Add(new SpellIcon(spell));
            }
        }

        void UIUpdate(GameTime gameTime)
        {
            int chosenSpell = 69;
            if (Keyboard.GetState().IsKeyDown(Keys.Q)) chosenSpell = 0;
            else if (Keyboard.GetState().IsKeyDown(Keys.W)) chosenSpell = 1;
            else if (Keyboard.GetState().IsKeyDown(Keys.E)) chosenSpell = 2;
            else if (Keyboard.GetState().IsKeyDown(Keys.R)) chosenSpell = 3;
            else if (Keyboard.GetState().IsKeyDown(Keys.F)) chosenSpell = 4;

            if (Keyboard.GetState().IsKeyDown(Keys.Z) && oldstate.IsKeyUp(Keys.Z)) quickcast = !quickcast;

            if (chosenSpell != 69)
            {
                if (mySpells[chosenSpell] != null)
                {
                    currentSpellIndex = chosenSpell;
                    currentSpell = mySpells[currentSpellIndex];
                    if(quickcast) Cast();
                }
            }
            else
            {
                
                int spellchange = 0;
                if(Mouse.GetState().ScrollWheelValue != oldScrollValue)
                {
                    spellchange = Math.Clamp(oldScrollValue - Mouse.GetState().ScrollWheelValue, -1, 1);
                    oldScrollValue = Mouse.GetState().ScrollWheelValue;
                }
                chosenSpell = Math.Clamp(currentSpellIndex + spellchange, 0, 8);
                if (chosenSpell != 69)
                {
                    if (mySpells[chosenSpell] != null)
                        currentSpellIndex = chosenSpell;
                }
            }


            currentSpell = mySpells[currentSpellIndex];

            for(int i = 0; i < currentCooldowns.Count; i++)
            {
                if (currentCooldowns[i] > 0)
                    currentCooldowns[i] -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                else currentCooldowns[i] = 0;

            }
        }

        void UIDraw()
        {
            int index = 0;
            foreach(SpellIcon spellIcon in spellIcons)
            {
                bool selected = index == currentSpellIndex;
                spellIcon.Draw(this, _spriteBatch, index, selected);
                index++;
            }

            _spriteBatch.DrawString(defaultfont, "Current Wave: " + gamePhase.ToString() + ", Enemies left: " + enemiesLeft.ToString(), new Vector2(10 + offsetX, areaSize.Y - 32), Color.Green);
        }
    }
}
