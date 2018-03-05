﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ailurus.Player
{
    public class PlayerController
    {
        public string Name { get; set; }
        public int Hitpoints { get; set; }
        public int Stamina { get; set; }
        public Point Position { get; set; }

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
        private const int COLOR_MIN = 100;
        private const int COLOR_MAX = 250;

        public event EventHandler PlayerCreated;
        protected virtual void OnPlayerCreated(EventArgs e) => PlayerCreated(this, e);

        public event EventHandler PlayerUpdated;
        protected virtual void OnPlayerUpdated(EventArgs e) => PlayerUpdated(this, e);

        public event EventHandler PlayerMoveAttempt;
        protected virtual void OnPlayerMoveAttempt(PlayerMoveAttemptEventArgs e) => PlayerMoveAttempt(this, e);
    
        public event EventHandler PlayerMoved;
        protected virtual void OnPlayerMoved(EventArgs e) => PlayerMoved(this, e);

        public PlayerController(Random rand, TextureMap<PlayerTexture> textures, SpriteBatch spriteBatch)
        {
            _rand = rand;
            _playerColor = new Color(_rand.Next(COLOR_MIN, COLOR_MAX), _rand.Next(COLOR_MIN, COLOR_MAX), _rand.Next(COLOR_MIN, COLOR_MAX));
            _hatColor = new Color(_rand.Next(COLOR_MIN, COLOR_MAX), _rand.Next(COLOR_MIN, COLOR_MAX), _rand.Next(COLOR_MIN, COLOR_MAX));
            _textures = textures;
            _spriteBatch = spriteBatch;
            
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
                if (keys.IsKeyDown(Keys.NumPad8))
                    dy -= 1;
                else if (keys.IsKeyDown(Keys.NumPad2))
                    dy += 1;
                else if (keys.IsKeyDown(Keys.NumPad4))
                    dx -= 1;
                else if (keys.IsKeyDown(Keys.NumPad6))
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