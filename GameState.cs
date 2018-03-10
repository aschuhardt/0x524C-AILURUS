using ailurus.Map;
using ailurus.Map.Tiles;
using ailurus.Player;
using ailurus.UI;
using DryIoc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        private Rectangle _prevDrawRegion;
        private KeyboardState _oldKeyState;
        private Color _bgColor;
        private Container _container;
        private PlayerController _player;
        private UserInterfaceController _ui;
        private Rectangle _uiDrawRectangle;
        private List<Message> _messages;

        private TimeSpan _inputCooldown = TimeSpan.FromMilliseconds(100);
        private TimeSpan _lastInputTime;
        private bool _showWholeMap;

        private const int MAP_LEFT_PAD = 2;
        private const int MAP_TOP_PAD = 4;
        private const int MAP_BOTTOM_PAD = 4;
        private const int MAP_RIGHT_PAD = 8;
        private const int UI_LEFT_PAD = 32;
        private const int UI_TOP_PAD = 4;
        private const double MAP_WIDTH_PERCENT = 0.90;
        private const double MAP_HEIGHT_PERCENT = 1.0;
        private const int MAP_REGION_SIZE = 17;
        private const string CONFIG_PATH = "config/config.json";
        private const string MAP_GEN_CONFIG_PATH = "config/map_generation.json";
        private const int MAP_SHRINK_AMOUNT = -8;

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

            _showWholeMap = false;

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
            _container.RegisterDelegate(x => new List<Message>(), Reuse.Singleton);
            _container.RegisterDelegate(x => new Random(), Reuse.Singleton);
            _container.RegisterDelegate(x => Helpers.GetConfiguration<Config>(CONFIG_PATH), Reuse.Singleton);
            _container.RegisterDelegate(x => Helpers.LoadTextures<TileType>(Content), Reuse.Singleton);
            _container.RegisterDelegate(x => Helpers.LoadTextures<DecorationType>(Content), Reuse.Singleton);
            _container.RegisterDelegate(x => Helpers.LoadTextures<PlayerTexture>(Content), Reuse.Singleton);
            _container.RegisterDelegate(x => Content.Load<SpriteFont>("fonts/vga"), Reuse.Singleton);
            _container.RegisterDelegate(x => Helpers.GetConfiguration<Generator.MapGenerationConfig>(MAP_GEN_CONFIG_PATH));
            _container.Register<PlayerController, PlayerController>(Reuse.Singleton);
            _container.Register<UserInterfaceController, UserInterfaceController>(Reuse.Singleton);

            // register tile types
            _container.Register<GrassTile, GrassTile>();
            _container.Register<DirtTile, DirtTile>();
            _container.Register<CobblestoneTile, CobblestoneTile>();

            _container.Register<TileMap, TileMap>(Reuse.Singleton);

            _map = _container.Resolve<TileMap>();

            _player = _container.Resolve<PlayerController>();
            _player.Position = _map.GetPlayerStartingPosition();
            _player.PlayerMoveAttempt += HandlePlayerMoveAttempt;
            _player.PlayerMoved += HandlePlayerMoved;

            _ui = _container.Resolve<UserInterfaceController>();

            _messages = _container.Resolve<List<Message>>();
        }

        private void HandlePlayerMoveAttempt(object sender, EventArgs e)
        {
            var args = e as PlayerMoveAttemptEventArgs;
            if (args == null) return;

            var destinationTile = _map.GetAbsoluteTile(args.Destination);
            args.Cancel = destinationTile.Obstacle;
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

                if (keys.IsKeyDown(Keys.M))
                {
                    _showWholeMap = !_showWholeMap;
                    if (_showWholeMap)
                    {
                        _prevDrawRegion = _drawRegion;
                        _drawRegion.Inflate(_map.Width / 4 - _prevDrawRegion.Width, _map.Height / 4 - _prevDrawRegion.Height);
                    }
                    else
                    {
                        _drawRegion = _prevDrawRegion;
                    }
                }

                _lastInputTime = gameTime.TotalGameTime;
            }

            _player.Update(gameTime);
            _ui.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_bgColor);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, sortMode: SpriteSortMode.Deferred);

            _ui.DrawRect(_mapDrawRectangle, _ui.BaseColor);
            _map.Draw(gameTime, _mapDrawRectangle, _drawRegion, _player.VisionRadius);

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

            var json = JsonConvert.SerializeObject(config);
            File.WriteAllText(CONFIG_PATH, json);

            SetupGUIPositioning();
        }

        private void SetupGUIPositioning()
        {
            var mapSize = Math.Min(Convert.ToInt32(Window.ClientBounds.Width * MAP_WIDTH_PERCENT),
                Convert.ToInt32(Window.ClientBounds.Height * MAP_HEIGHT_PERCENT));

            _mapDrawRectangle = new Rectangle(MAP_LEFT_PAD, MAP_TOP_PAD, mapSize, mapSize - MAP_BOTTOM_PAD);
            _mapDrawRectangle.Inflate(MAP_SHRINK_AMOUNT, MAP_SHRINK_AMOUNT);

            var uiWidth = Window.ClientBounds.Width - _mapDrawRectangle.Width;
            var uiHeight = Window.ClientBounds.Height;
            _uiDrawRectangle = new Rectangle(_mapDrawRectangle.Right + MAP_LEFT_PAD + MAP_RIGHT_PAD, UI_TOP_PAD, uiWidth - MAP_RIGHT_PAD * 2, uiHeight);
        }
    }
}
