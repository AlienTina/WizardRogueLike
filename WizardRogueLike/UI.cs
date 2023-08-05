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
            Vector2 iconPosition = new Vector2(0, 116 + (30 * index));
            if (mySpell != null)
            {
                float cooldown = (float)Math.Round(game.currentCooldowns[index], 2);
                Spell newSpellCast = (Spell)Activator.CreateInstance(mySpell, Vector2.Zero, Vector2.Zero, 0, false);
                float maxCooldown = newSpellCast.cooldown;
                Rectangle cooldownRect = new Rectangle(iconPosition.ToPoint(), new Point((int)(((cooldown) / maxCooldown) * 300), 32));
                _spriteBatch.Draw(game.boxTexture, cooldownRect, Color.Gray);
            }
            _spriteBatch.Draw(game.spellIconTexture, iconPosition, Color.White);
            if (mySpell != null)
            {
                
                //Vector2 cooldownSize = game.defaultfont.MeasureString(game.currentCooldowns[index].ToString());
                //Vector2 cooldownPosition = iconPosition + new Vector2(300, 0);
                if(!selected)
                    _spriteBatch.DrawString(game.defaultfont, mySpell.Name, iconPosition + (Vector2.One * 3), Color.Black);
                else
                    _spriteBatch.DrawString(game.boldfont, mySpell.Name, iconPosition + (Vector2.One * 3), Color.LimeGreen);
                //_spriteBatch.DrawString(game.defaultfont, Math.Round(game.currentCooldowns[index], 2).ToString(), cooldownPosition, Color.Black, 0, new Vector2(cooldownSize.X, 0), 1, SpriteEffects.None, 0);

            }
        }
    }
    partial class Game1
    {
        public Texture2D spellIconTexture;
        public Texture2D boxTexture;

        public List<SpellIcon> spellIcons = new List<SpellIcon>();

        void UIInstantiate()
        {
            foreach(Type spell in mySpells)
            {
                spellIcons.Add(new SpellIcon(spell));
            }
        }

        void UIUpdate(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D1)) currentSpellIndex = 0;
            else if (Keyboard.GetState().IsKeyDown(Keys.D2)) currentSpellIndex = 1;
            else if (Keyboard.GetState().IsKeyDown(Keys.D3)) currentSpellIndex = 2;
            else if (Keyboard.GetState().IsKeyDown(Keys.D4)) currentSpellIndex = 3;
            else if (Keyboard.GetState().IsKeyDown(Keys.D5)) currentSpellIndex = 4;
            else if (Keyboard.GetState().IsKeyDown(Keys.D6)) currentSpellIndex = 5;
            else if (Keyboard.GetState().IsKeyDown(Keys.D7)) currentSpellIndex = 6;
            else if (Keyboard.GetState().IsKeyDown(Keys.D8)) currentSpellIndex = 7;
            else if (Keyboard.GetState().IsKeyDown(Keys.D9)) currentSpellIndex = 8;

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
        }
    }
}
