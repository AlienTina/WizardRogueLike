using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime;

namespace WizardRogueLike
{
    public partial class Game1 : Game
    {
        bool playing = true;

        float tileSize;
        Vector2 gridSize;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D spellbarTexture;
        public SpriteFont defaultfont;
        public SpriteFont boldfont;

        public Vector2 areaSize = new Vector2(64 * 15, 64 * 10);

        int offsetX = 300;
        int offsetY = 0;

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

            playerPosition = new Vector2(areaSize.X / 2, areaSize.Y / 2);

            _graphics.ApplyChanges();

            SpellInstantiate();
            UIInstantiate();

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

            if (playing)
            {

                playerUpdate(gameTime);
                StaffUpdate();
                enemyUpdate(gameTime);
                BulletUpdate(gameTime);
                UIUpdate(gameTime);
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            Matrix Transform = Matrix.CreateTranslation(offsetX, offsetY, 0);

            _spriteBatch.Begin();
            _spriteBatch.Draw(spellbarTexture, Vector2.Zero, Color.White);
            UIDraw();
            _spriteBatch.End();

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Transform);

            Leveldraw();
            enemyDraw();
            BulletDraw(gameTime);


            

            if(invincibility <= 0) _spriteBatch.Draw(playerTexture, playerPosition, Color.White);
            else _spriteBatch.Draw(playerTexture, playerPosition, Color.Gray);
            _spriteBatch.Draw(staffTexture, staffPosition, null, Color.White, staffRotation, staffOrigin, Vector2.One, SpriteEffects.None, 0);

            _spriteBatch.DrawString(defaultfont, playerHealth.ToString(), Vector2.One * 16, Color.Red, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}