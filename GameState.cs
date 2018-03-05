using ailurus.Map;
using ailurus.Map.Tiles;
using ailurus.Player;
using ailurus.UI;
using DryIoc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using static ailurus.Player.PlayerController;

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
        private PlayerController _player;
        private UserInterfaceController _ui;
        private Rectangle _uiDrawRectangle;

        private TimeSpan _inputCooldown = TimeSpan.FromMilliseconds(100);
        private TimeSpan _lastInputTime;

        private const int MAP_LEFT_PAD = 4;
        private const int MAP_TOP_PAD = 4;
        private const int MAP_BOTTOM_PAD = 4;
        private const int UI_LEFT_PAD = 32;
        private const int UI_TOP_PAD = 4;
        private const double MAP_WIDTH_PERCENT = 0.90;
        private const double MAP_HEIGHT_PERCENT = 1.0;
        private const int MAP_REGION_SIZE = 16;
        private const string CONFIG_PATH = "config.json";
        
        public GameState()
        {
            graphics = new GraphicsDeviceManager(this);

            _container = new Container();

            Content.RootDirectory = "Content";

            IsMouseVisible = true;

            Window.ClientSizeChanged += HandleWindowResize;
        }

        protected override void Initialize()
        {
            base.Initialize();

            var config = _container.Resolve<Config>();

            graphics.PreferredBackBufferWidth = config.WindowWidth;
            graphics.PreferredBackBufferHeight = config.WindowHeight;

            _drawRegion = new Rectangle(config.MapWidth / 2 - (MAP_REGION_SIZE / 2), 
                config.MapHeight / 2 - (MAP_REGION_SIZE / 2), MAP_REGION_SIZE, MAP_REGION_SIZE);

            _bgColor = new Color(18, 21, 26);
            
            _oldKeyState = Keyboard.GetState();
            
            SetupGUIPositioning();
            Window.AllowUserResizing = true;
            Window.Title = "0x524C-AILURUS";
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _container.UseInstance(spriteBatch);

            _container.RegisterDelegate(x => Helpers.GetDecorations(), Reuse.Singleton);
            _container.UseInstance(graphics);
            _container.UseInstance(_container);
            _container.RegisterDelegate(x => new Random(), Reuse.Singleton);
            _container.RegisterDelegate(x => Helpers.LoadConfig(CONFIG_PATH), Reuse.Singleton);
            _container.RegisterDelegate(x => Helpers.LoadTextures<TileType>(Content), Reuse.Singleton);
            _container.RegisterDelegate(x => Helpers.LoadTextures<DecorationType>(Content), Reuse.Singleton);
            _container.RegisterDelegate(x => Helpers.LoadTextures<PlayerTexture>(Content), Reuse.Singleton);
            _container.RegisterDelegate(x => Content.Load<SpriteFont>("fonts/vga"), Reuse.Singleton);
            _container.Register<PlayerController, PlayerController>(Reuse.Singleton);
            _container.Register<UserInterfaceController, UserInterfaceController>(Reuse.Singleton);

            // register tile types
            _container.Register<GrassTile, GrassTile>();
            _container.Register<DirtTile, DirtTile>();

            _container.Register<TileMap, TileMap>(Reuse.Singleton);

            _map = _container.Resolve<TileMap>();

            _player = _container.Resolve<PlayerController>();
            _player.Position = _map.GetPlayerStartingPosition();
            _player.PlayerMoved += HandlePlayerMoved;

            _ui = _container.Resolve<UserInterfaceController>();
        }

        private void HandlePlayerMoved(object sender, EventArgs e)
        {
            _drawRegion.Location = new Point(_player.Position.X - _drawRegion.Width / 2,
                _player.Position.Y - _drawRegion.Height / 2);
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            _ui.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var keys = Keyboard.GetState();

            if (gameTime.TotalGameTime - _lastInputTime >= _inputCooldown)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                if (keys.IsKeyDown(Keys.Add))
                    _drawRegion.Inflate(1, 1);
                else if (keys.IsKeyDown(Keys.Subtract) && _drawRegion.Width > 1 && _drawRegion.Height > 1)
                    _drawRegion.Inflate(-1, -1);

                if (keys.IsKeyDown(Keys.Up))
                    _drawRegion.Offset(0, -1);
                else if (keys.IsKeyDown(Keys.Down))
                    _drawRegion.Offset(0, 1);
                else if (keys.IsKeyDown(Keys.Left))
                    _drawRegion.Offset(-1, 0);
                else if (keys.IsKeyDown(Keys.Right))
                    _drawRegion.Offset(1, 0);

                _lastInputTime = gameTime.TotalGameTime;
            }

            _player.Update(gameTime);
            _ui.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_bgColor);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            _map.Draw(gameTime, _mapDrawRectangle, _drawRegion);

            var playerRect = _map.GetScreenCoordinates(_player.Position, _mapDrawRectangle, _drawRegion);
            if (_mapDrawRectangle.Contains(playerRect))
                _player.Draw(gameTime, playerRect);

            _ui.Draw(gameTime, _uiDrawRectangle);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void HandleWindowResize(object sender, EventArgs e)
        {
            var config = _container.Resolve<Config>();
            config.WindowWidth = Window.ClientBounds.Width;
            config.WindowHeight = Window.ClientBounds.Height;
            Helpers.SaveConfig(config, CONFIG_PATH);

            SetupGUIPositioning();
        }

        private void SetupGUIPositioning()
        {
            var mapSize = Math.Min(Convert.ToInt32(Window.ClientBounds.Width * MAP_WIDTH_PERCENT),
                Convert.ToInt32(Window.ClientBounds.Height * MAP_HEIGHT_PERCENT));

            _mapDrawRectangle = new Rectangle(MAP_LEFT_PAD, MAP_TOP_PAD, mapSize, mapSize - MAP_BOTTOM_PAD);

            var uiWidth = Window.ClientBounds.Width - _mapDrawRectangle.Width;
            var uiHeight = Window.ClientBounds.Height;
            _uiDrawRectangle = new Rectangle(_mapDrawRectangle.Right + MAP_LEFT_PAD, UI_TOP_PAD, uiWidth, uiHeight);
        }
    }
}
