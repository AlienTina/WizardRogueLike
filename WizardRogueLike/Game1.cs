using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime;

namespace WizardRogueLike
{
    public enum GameState
    {
        playing = 0,
        ended,
        paused,
        inbetween,
        starting
    }
    public static class Vector2Extensions
    {
        public static Vector2 Rotate(this Vector2 v, double degrees)
        {
            return new Vector2(
                (float)(v.X * Math.Cos(degrees) - v.Y * Math.Sin(degrees)),
                (float)(v.X * Math.Sin(degrees) + v.Y * Math.Cos(degrees))
            );
        }
    }

    public partial class Game1 : Game
    {
        public GameState state = GameState.starting;

        float tileSize;
        Vector2 gridSize;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D spellbarTexture;
        public SpriteFont defaultfont;
        public SpriteFont boldfont;

        public Vector2 areaSize = new Vector2(64 * 20, 64 * 10);

        public int offsetX = 0;
        public int offsetY = 0;
        public int offsetX2 = 0;
        public int offsetY2 = 33;

        public Random rand = new Random();

        float blink = 1;

        //public string titlechange = "";

        KeyboardState oldstate = new KeyboardState();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = (int)areaSize.X + offsetX + offsetX2;
            _graphics.PreferredBackBufferHeight = (int)areaSize.Y + offsetY + offsetY2;

            

            _graphics.ApplyChanges();

            interactableSpells = new List<Spell>();

            allAvailableSpells = new List<Type>();

            allAvailableSpells.Add(typeof(FireBall));
            allAvailableSpells.Add(typeof(VenomDart));
            allAvailableSpells.Add(typeof(FrostShard));
            allAvailableSpells.Add(typeof(ShockBolt));
            allAvailableSpells.Add(typeof(AquaJet));
            allAvailableSpells.Add(typeof(AirBlast));
            allAvailableSpells.Add(typeof(VineSnare));
            allAvailableSpells.Add(typeof(EarthSpike));
            allAvailableSpells.Add(typeof(ShadowBolt));
            allAvailableSpells.Add(typeof(Summon));

            mySpells = new List<Type>(5);
            currentCooldowns = new List<float>(5);
            spellDamageBonus = new List<int>(5);
            for (int i = 0; i < 5; i++)
            {
                mySpells.Add(null);
                currentCooldowns.Add(0);
                spellDamageBonus.Add(0);
            }

            //mySpells[1] = typeof(VoidEruption);

            gamePhase = 0;

            PlayerInstantiate();

            BetweenInitialize();

            //Window.Title = "Shadow Wizard Money Gang ⚫🧙‍♂️💸💪";

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;

            spellbarTexture = Content.Load<Texture2D>("spellbar");
            spellIconTexture = Content.Load<Texture2D>("spellicon");

            playerTexture = Content.Load<Texture2D>("sprites/Characters/green_character");
            tile = Content.Load<Texture2D>("sprites/tile");
            wallTile = Content.Load<Texture2D>("sprites/wall_tile");
            staffTexture = Content.Load<Texture2D>("sprites/Items/weapon_staff");
            enemyTexture = Content.Load<Texture2D>("sprites/Characters/red_character");
            boxTexture = Content.Load<Texture2D>("box");
            targetTexture = Content.Load<Texture2D>("sprites/Characters/player_target");

            tileSize = tile.Bounds.Size.X;

            gridSize = new Vector2(areaSize.X / tileSize, areaSize.Y / tileSize);

            truePlayerR = playerTexture.Width / 2;

            staffOrigin = new Vector2(staffTexture.Width + playerRadius, staffTexture.Height / 2);

            defaultfont = Content.Load<SpriteFont>("DefaultFont");
            boldfont = Content.Load<SpriteFont>("BoldFont");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            
            if (state == GameState.playing)
            {
                Window.Title = "Unleashing Chaos Upon the Mortals";
                if(blink > 0) blink -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                playerUpdate(gameTime);
                StaffUpdate();
                enemyUpdate(gameTime);
                BulletUpdate(gameTime);
                UIUpdate(gameTime);
                UpdateWave(gameTime);

            }
            else if(state == GameState.ended)
            {
                Window.Title = "Sanity Shattered, but the Madness Endures";
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    Initialize();
                    state = GameState.inbetween;
                }
            }
            else if(state == GameState.inbetween)
            {
                Window.Title = "Dwelling in the Shadows of Madness";
                BetweenUpdate();
            }
            else if (state == GameState.starting)
            {
                Window.Title = "The Perilous Abyss Awaits";
                if (Keyboard.GetState().IsKeyDown(Keys.Enter)) state = GameState.inbetween;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.M))
            {
                gamePhase++;
                ChooseSpells();
                state = GameState.inbetween;
            }
            // TODO: Add your update logic here

            oldstate = Keyboard.GetState();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Matrix Transform = Matrix.CreateTranslation(offsetX, offsetY, 0);

            

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Transform);

            if (state == GameState.playing)
            {
                Leveldraw();
                BulletDraw(gameTime);
                enemyDraw();
                PlayerDraw();
                _spriteBatch.DrawString(defaultfont, Math.Round(playerHealth, 0).ToString(), Vector2.One * 16, Color.Red, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
            }
            else if(state == GameState.ended)
            {
                _spriteBatch.DrawString(boldfont, "Game Over\nWaves survived: " + gamePhase.ToString(), areaSize / 2, Color.Red);
            }
            else if(state == GameState.inbetween)
            {
                BetweenDraw();
            }
            else if(state == GameState.starting)
            {
                int alignText = (int)defaultfont.MeasureString("Dare to conquer the Perilous Abyss? \nPress Enter to enter.").X;
                _spriteBatch.DrawString(defaultfont, "Dare to conquer the Perilous Abyss? \nPress the 'Enter' key to embark on your quest.", areaSize / 2 - ((Vector2.UnitX * alignText) / 2), Color.White);
            }
            _spriteBatch.End();

            _spriteBatch.Begin();
            //_spriteBatch.Draw(spellbarTexture, Vector2.Zero, Color.White);
            UIDraw();
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}