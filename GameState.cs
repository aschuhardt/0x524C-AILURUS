using ailurus.Map;
using ailurus.Map.Tiles;
using DryIoc;
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
        private TileMap _map;
        private Rectangle _mapDrawRectangle;
        private Rectangle _drawRegion;
        private KeyboardState _oldKeyState;
        private Color _bgColor;

        private Container _container;

        private const int MAP_LEFT_PAD = 4;
        private const int MAP_TOP_PAD = 4;
        private const double MAP_WIDTH_PERCENT = 0.75;
        private const double MAP_HEIGHT_PERCENT = 0.95;
        private const string CONFIG_PATH = "config.json";

        public GameState()
        {
            _container = new Container();

            _container.RegisterDelegate(x => Helpers.GetDecorations(), Reuse.Singleton);
            _container.RegisterDelegate(x => _container, Reuse.Singleton);
            _container.RegisterDelegate(x => new Random(), Reuse.Singleton);
            _container.RegisterDelegate(x => Helpers.LoadConfig(CONFIG_PATH), Reuse.Singleton);

            var config = _container.Resolve<Config>();

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = config.WindowWidth;
            graphics.PreferredBackBufferHeight = config.WindowHeight;

            _drawRegion = new Rectangle(config.MapWidth / 2, config.MapHeight / 2, 16, 16);

            _bgColor = new Color(18, 21, 26);

            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            Window.ClientSizeChanged += HandleWindowResize;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _oldKeyState = Keyboard.GetState();
            
            SetMapDrawSize();
            Window.AllowUserResizing = true;
            Window.Title = "0x524C-AILURUS";
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _container.UseInstance(Helpers.LoadTextures<TileType>(Content));
            _container.UseInstance(Helpers.LoadTextures<DecorationType>(Content));
            _container.UseInstance(spriteBatch);

            // register tile types
            _container.Register<GrassTile, GrassTile>();
            _container.Register<DirtTile, DirtTile>();

            _container.Register<TileMap, TileMap>();
            _map = _container.Resolve<TileMap>();
        }

        protected override void Update(GameTime gameTime)
        {
            var currentKeyState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (KeyPressed(Keys.Add))
                _drawRegion.Inflate(1, 1);
            else if (KeyPressed(Keys.Subtract))
                _drawRegion.Inflate(-1, -1);

            if (KeyPressed(Keys.Up))
                _drawRegion.Offset(0, -1);
            else if (KeyPressed(Keys.Down))
                _drawRegion.Offset(0, 1);
            else if (KeyPressed(Keys.Left))
                _drawRegion.Offset(-1, 0);
            else if (KeyPressed(Keys.Right))
                _drawRegion.Offset(1, 0);

            _oldKeyState = currentKeyState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_bgColor);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            _map.Draw(gameTime, _mapDrawRectangle, _drawRegion);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void HandleWindowResize(object sender, EventArgs e)
        {
            var config = _container.Resolve<Config>();
            config.WindowWidth = Window.ClientBounds.Width;
            config.WindowHeight = Window.ClientBounds.Height;
            Helpers.SaveConfig(config, CONFIG_PATH);

            SetMapDrawSize();
        }

        private void SetMapDrawSize()
        {
            var size = Math.Min(Convert.ToInt32(Window.ClientBounds.Width * MAP_WIDTH_PERCENT),
                Convert.ToInt32(Window.ClientBounds.Height * MAP_HEIGHT_PERCENT));

            _mapDrawRectangle = new Rectangle(MAP_LEFT_PAD, MAP_TOP_PAD, size, size);
        }

        private bool KeyPressed(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key) && _oldKeyState.IsKeyUp(key);
        }
    }
}
