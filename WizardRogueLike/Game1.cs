using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace WizardRogueLike
{
    public enum GameState
    {
        playing = 0,
        ended,
        paused,
        inbetween
    }
    public partial class Game1 : Game
    {

        float tileSize;
        Vector2 gridSize;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D spellbarTexture;
        public SpriteFont defaultfont;
        public SpriteFont boldfont;

        public Vector2 areaSize = new Vector2(64 * 15, 64 * 10);

        public int offsetX = 300;
        public int offsetY = 0;

        Random rand = new Random();

        

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = (int)areaSize.X + offsetX;
            _graphics.PreferredBackBufferHeight = (int)areaSize.Y;

            

            _graphics.ApplyChanges();

            allAvailableSpells.Add(typeof(Fireball));
            allAvailableSpells.Add(typeof(ToxicBall));
            allAvailableSpells.Add(typeof(IceBall));
            allAvailableSpells.Add(typeof(FireWorm));
            allAvailableSpells.Add(typeof(FireZone));
            allAvailableSpells.Add(typeof(ToxicZone));
            allAvailableSpells.Add(typeof(Summon));
            allAvailableSpells.Add(typeof(Dash));

            mySpells = new List<Type>(9);
            currentCooldowns = new List<float>(9);
            for (int i = 0; i < 9; i++)
            {
                mySpells.Add(null);
                currentCooldowns.Add(0);
            }

            gamePhase = 0;

            PlayerInstantiate();

            BetweenInitialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            spellbarTexture = Content.Load<Texture2D>("spellbar");
            spellIconTexture = Content.Load<Texture2D>("spellicon");

            playerTexture = Content.Load<Texture2D>("sprites/Characters/green_character");
            tile = Content.Load<Texture2D>("sprites/tile");
            wallTile = Content.Load<Texture2D>("sprites/wall_tile");
            staffTexture = Content.Load<Texture2D>("sprites/Items/weapon_staff");
            enemyTexture = Content.Load<Texture2D>("sprites/Characters/red_character");
            boxTexture = Content.Load<Texture2D>("box");

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

                playerUpdate(gameTime);
                StaffUpdate();
                enemyUpdate(gameTime);
                BulletUpdate(gameTime);
                UIUpdate(gameTime);
                
            }
            else if(state == GameState.ended)
            {
                if(Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    Initialize();
                    state = GameState.inbetween;
                }
            }
            else if(state == GameState.inbetween)
            {
                BetweenUpdate();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.M))
            {
                gamePhase++;
                ChooseSpells();
                state = GameState.inbetween;
            }
                // TODO: Add your update logic here

                base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            Matrix Transform = Matrix.CreateTranslation(offsetX, offsetY, 0);

            

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Transform);

            if (state == GameState.playing)
            {
                Leveldraw();
                enemyDraw();
                BulletDraw(gameTime);
                PlayerDraw();
                UpdateWave(gameTime);
            }
            else if(state == GameState.ended)
            {
                _spriteBatch.DrawString(boldfont, "Game Over\nWaves survived: " + gamePhase.ToString(), areaSize / 2, Color.Red);
            }
            else if(state == GameState.inbetween)
            {
                BetweenDraw();
            }

            _spriteBatch.DrawString(defaultfont, playerHealth.ToString(), Vector2.One * 16, Color.Red, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
            _spriteBatch.End();

            _spriteBatch.Begin();
            _spriteBatch.Draw(spellbarTexture, Vector2.Zero, Color.White);
            UIDraw();
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}