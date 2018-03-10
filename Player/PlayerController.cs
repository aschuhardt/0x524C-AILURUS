using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ailurus.Player
{
    public class PlayerController
    {
        public string Name { get; set; }

        public int Hitpoints { get; set; }
        public int MaxHitpoints { get; set; }

        public int Stamina { get; set; }
        public int MaxStamina { get; set; }

        public Point Position { get; set; }
        public int VisionRadius { get; set; }

        public enum PlayerTexture
        {
            [Texture("player/body")]
            Body,

            [Texture("player/hat")]
            Hat
        }

        private TextureMap<PlayerTexture> _textures;
        private SpriteBatch _spriteBatch;
        private Color _playerColor;
        private Color _hatColor;
        private Random _rand;
        private TimeSpan _movementCooldown = TimeSpan.FromMilliseconds(150);
        private TimeSpan _lastMovementTime;
        private List<Message> _messages;
        private const int COLOR_MIN = 100;
        private const int COLOR_MAX = 250;
        private const int MIN_HP = 50;
        private const int MAX_HP = 200;
        private const int MIN_ST = 10;
        private const int MAX_ST = 40;
        private const int DEFAULT_VISION_RADIUS = 8;

        public event EventHandler PlayerCreated;
        protected virtual void OnPlayerCreated(EventArgs e) => PlayerCreated(this, e);

        public event EventHandler PlayerUpdated;
        protected virtual void OnPlayerUpdated(EventArgs e) => PlayerUpdated(this, e);

        public event EventHandler PlayerMoveAttempt;
        protected virtual void OnPlayerMoveAttempt(PlayerMoveAttemptEventArgs e) => PlayerMoveAttempt(this, e);
    
        public event EventHandler PlayerMoved;
        protected virtual void OnPlayerMoved(EventArgs e) => PlayerMoved(this, e);

        public PlayerController(Random rand, TextureMap<PlayerTexture> textures, SpriteBatch spriteBatch, List<Message> messages)
        {
            _rand = rand;
            _messages = messages;
            _playerColor = new Color(_rand.Next(COLOR_MIN, COLOR_MAX), _rand.Next(COLOR_MIN, COLOR_MAX), _rand.Next(COLOR_MIN, COLOR_MAX));
            _hatColor = new Color(_rand.Next(COLOR_MIN, COLOR_MAX), _rand.Next(COLOR_MIN, COLOR_MAX), _rand.Next(COLOR_MIN, COLOR_MAX));
            _textures = textures;
            _spriteBatch = spriteBatch;

            MaxHitpoints = rand.Next(MIN_HP, MAX_HP + 1);
            Hitpoints = MaxHitpoints;

            MaxStamina = rand.Next(MIN_ST, MAX_ST + 1);
            Stamina = MaxStamina;

            VisionRadius = DEFAULT_VISION_RADIUS;

            _messages.Add(new Message("Welcome!", MessageType.Info));
            _messages.Add(new Message("Use either the numpad or WASD to move.", MessageType.Dialog));
            _messages.Add(new Message("The +/- keys on the numpad can be used to zoom the map.", MessageType.Healing));
            _messages.Add(new Message("The M key will toggle a zoomed-out state.", MessageType.Healing));

            if (PlayerCreated != null)
                OnPlayerCreated(EventArgs.Empty);
        }

        public void Update(GameTime gameTime)
        {
            var keys = Keyboard.GetState();
            if (gameTime.TotalGameTime - _lastMovementTime >= _movementCooldown)
            {
                var (dx, dy) = (0, 0);

                // check for movement input
                if (keys.IsKeyDown(Keys.NumPad8) || keys.IsKeyDown(Keys.W))
                    dy -= 1;
                else if (keys.IsKeyDown(Keys.NumPad2) || keys.IsKeyDown(Keys.S))
                    dy += 1;
                else if (keys.IsKeyDown(Keys.NumPad4) || keys.IsKeyDown(Keys.A))
                    dx -= 1;
                else if (keys.IsKeyDown(Keys.NumPad6) || keys.IsKeyDown(Keys.D))
                    dx += 1;
                else if (keys.IsKeyDown(Keys.NumPad7))
                {
                    dx -= 1;
                    dy -= 1;
                }
                else if (keys.IsKeyDown(Keys.NumPad9))
                {
                    dx += 1;
                    dy -= 1;
                }
                else if (keys.IsKeyDown(Keys.NumPad1))
                {
                    dx -= 1;
                    dy += 1;
                }
                else if (keys.IsKeyDown(Keys.NumPad3))
                {
                    dx += 1;
                    dy += 1;
                }

                if (dx != 0 || dy != 0)
                {
                    var destination = new Point(Position.X + dx, Position.Y + dy);
                    var moveAttemptArgs = new PlayerMoveAttemptEventArgs(destination);
                    if (PlayerMoveAttempt != null)
                        OnPlayerMoveAttempt(moveAttemptArgs);
                    if (!moveAttemptArgs.Cancel)
                    {
                        Position = destination;
                        if (PlayerMoved != null)
                            OnPlayerMoved(EventArgs.Empty);
                        _lastMovementTime = gameTime.TotalGameTime;
                    }
                }
            }
        }

        public void Draw(GameTime gameTime, Rectangle rect)
        {
            _spriteBatch.Draw(_textures[PlayerTexture.Body].First(), rect, _playerColor);
            _spriteBatch.Draw(_textures[PlayerTexture.Hat].First(), rect, _hatColor);
        }
    }
}
