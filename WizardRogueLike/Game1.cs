using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        public Vector2 areaSize = new Vector2(64 * 21, 64 * 11);

        public int offsetX = 0;
        public int offsetY = 0;
        public int offsetX2 = 0;
        public int offsetY2 = 33;

        public Random rand = new Random();

        public float blink = 0;

        //public string titlechange = "";

        KeyboardState oldstate = new KeyboardState();

        DungeonGenerator generator = new DungeonGenerator();

        char[,] dungeon;

        int gridWidth = 3;
        int gridHeight = 3;

        Texture2D minimapGrid;

        Vector2 currentDungeonPosition = Vector2.Zero;

        Texture2D cursorSprite;

        public Vector2 CursorPosition;

        Vector2 screenCenter;

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

            

            CursorPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            screenCenter = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            IsMouseVisible = false;

            _graphics.ApplyChanges();

            StartGame();

            //Window.Title = "Shadow Wizard Money Gang ⚫🧙‍♂️💸💪";

            base.Initialize();
        }

        void StartGame()
        {
            dungeon = generator.GenerateDungeon(gridWidth, gridHeight);
            currentDungeonPosition = generator.start;

            enemiesInRooms = new Dictionary<Vector2, List<GameObject>>();

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
            allAvailableSpells.Add(typeof(Dash));

            mySpells = new List<Type>(5);
            currentCooldowns = new List<float>(5);
            spellDamageBonus = new List<int>(5);
            for (int i = 0; i < 5; i++)
            {
                mySpells.Add(null);
                currentCooldowns.Add(0);
                spellDamageBonus.Add(0);
            }

            //mySpells[1] = typeof(Summon);



            gamePhase = 0;

            //enemyInitiate();

            PlayerInstantiate();

            BetweenInitialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;

            cursorSprite = Content.Load<Texture2D>("sprites/dwarven_gauntlet");

            spellbarTexture = Content.Load<Texture2D>("spellbar");
            spellIconTexture = Content.Load<Texture2D>("spellicon");

            playerTexture = Content.Load<Texture2D>("sprites/Characters/player");

            leftWall = Content.Load<Texture2D>("sprites/Tilemap/leftwall");
            rightWall = Content.Load<Texture2D>("sprites/Tilemap/rightwall");
            upWall = Content.Load<Texture2D>("sprites/Tilemap/upwall");
            downWall = Content.Load<Texture2D>("sprites/Tilemap/downWall");

            leftdowncorner = Content.Load<Texture2D>("sprites/Tilemap/leftdown");
            leftupcorner = Content.Load<Texture2D>("sprites/Tilemap/leftup");
            rightdowncorner = Content.Load<Texture2D>("sprites/Tilemap/rightdown");
            rightupcorner = Content.Load<Texture2D>("sprites/Tilemap/rightup");

            staffTexture = Content.Load<Texture2D>("sprites/Items/weapon_staff");
            boxTexture = Content.Load<Texture2D>("box");
            targetTexture = Content.Load<Texture2D>("sprites/Characters/player_target");

            tileSize = leftWall.Bounds.Size.X;

            gridSize = new Vector2(areaSize.X / tileSize, areaSize.Y / tileSize);

            truePlayerR = playerTexture.Width / 2;

            staffOrigin = new Vector2(staffTexture.Width + playerRadius, staffTexture.Height / 2);

            defaultfont = Content.Load<SpriteFont>("DefaultFont");
            boldfont = Content.Load<SpriteFont>("BoldFont");

            minimapGrid = Content.Load<Texture2D>("roomTile");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (IsActive)
            {
                CursorPosition -= screenCenter - Mouse.GetState().Position.ToVector2();
                CursorPosition.X = Math.Clamp(CursorPosition.X, 0, areaSize.X);
                CursorPosition.Y = Math.Clamp(CursorPosition.Y, 0, areaSize.Y);
                Mouse.SetPosition((int)screenCenter.X, (int)screenCenter.Y);
            }
            if (state == GameState.playing)
            {
                if (generator.rooms[(int)currentDungeonPosition.X, (int)currentDungeonPosition.Y].myType == roomType.fight && enemiesLeft > 0)
                    Window.Title = "Unleashing Chaos Upon the Mortals";
                else if (generator.rooms[(int)currentDungeonPosition.X, (int)currentDungeonPosition.Y].myType == roomType.fight && enemiesLeft <= 0)
                    Window.Title = "Everyone's dead. What now?";
                else if (generator.rooms[(int)currentDungeonPosition.X, (int)currentDungeonPosition.Y].myType == roomType.boss && enemiesLeft > 0)
                    Window.Title = "Nothing. . . Perhaps I should return here later. . .";
                else if (generator.rooms[(int)currentDungeonPosition.X, (int)currentDungeonPosition.Y].myType == roomType.boss && enemiesLeft <= 0)
                    Window.Title = "A worthy challenger.";
                else if (generator.rooms[(int)currentDungeonPosition.X, (int)currentDungeonPosition.Y].myType == roomType.item)
                    Window.Title = "What riches does this room hold?";
                playerUpdate(gameTime);
                StaffUpdate();
                enemyUpdate(gameTime);
                BulletUpdate(gameTime);
                UIUpdate(gameTime);

                //if (blink > 0) blink -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                //if (blink <= 0) blink = 0f;

            }
            else if(state == GameState.ended)
            {
                Window.Title = "Sanity Shattered, but the Madness Endures";
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    StartGame();
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
                _spriteBatch.DrawString(defaultfont, Math.Round(playerHealth, 0).ToString(), new Vector2(_graphics.PreferredBackBufferWidth - 64, 16), Color.Red, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
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
            if(state == GameState.playing)
                MinimapDraw();
            _spriteBatch.Draw(cursorSprite, CursorPosition, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}