using ailurus.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ailurus
{
    public class GameState : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private TextureMap<TileType> _tileTextures;
        private TileMap _map;
        private Rectangle _mapRectangle;

        private const int MAP_LEFT_PAD = 4;
        private const int MAP_TOP_PAD = 4;
        private const double MAP_WIDTH_PERCENT = 0.75;
        private const double MAP_HEIGHT_PERCENT = 0.95;

        public GameState()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Window.ClientSizeChanged += HandleWindowResize;
        }

        private void HandleWindowResize(object sender, EventArgs e)
        {
            SetMapSize();
        }

        private void SetMapSize()
        {
            var size = Math.Min(Convert.ToInt32(Window.ClientBounds.Width * MAP_WIDTH_PERCENT),
                Convert.ToInt32(Window.ClientBounds.Height * MAP_HEIGHT_PERCENT));

            _mapRectangle = new Rectangle(MAP_LEFT_PAD, MAP_TOP_PAD, size, size);
        }

        protected override void Initialize()
        {
            base.Initialize();

            SetMapSize();
            Window.AllowUserResizing = true;
            Window.Title = "0x524C-AILURUS";
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load textures
            _tileTextures = Helpers.LoadTextures<TileType>(Content);


            _map = new TileMap(32, 32, _tileTextures, spriteBatch);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            _map.Draw(gameTime, _mapRectangle);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
