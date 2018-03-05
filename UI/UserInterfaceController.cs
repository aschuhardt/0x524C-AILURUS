using ailurus.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ailurus.UI
{
    public class UserInterfaceController
    {
        private PlayerController _player;
        private SpriteFont _font;
        private SpriteBatch _spriteBatch;

        private string _hitpointsText;
        private string _staminaText;
        private string _positionText;
        private Color _hitpointsColor;
        private Color _staminaColor;
        private Color _positionColor;
        private Texture2D _barTexture;
        private Color _barBgColor;
        private const int VERTICAL_SPACING = 14;

        public UserInterfaceController(PlayerController player, SpriteFont font, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            _player = player;
            _font = font;
            _spriteBatch = spriteBatch;

            _hitpointsColor = new Color(124, 58, 66);
            _staminaColor = new Color(130, 128, 60);
            _positionColor = new Color(106, 112, 123);

            _barTexture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _barTexture.SetData(new[] { Color.White });
            _barBgColor = new Color(18, 21, 26);
        }

        public void Update(GameTime gameTime)
        {
            _hitpointsText = $"HP: {_player.Hitpoints}/{_player.MaxHitpoints}";
            _staminaText = $"ST: {_player.Stamina}/{_player.MaxStamina}";
            _positionText = $"X: {_player.Position.X}, Y: {_player.Position.Y}";
        }

        public void Draw(GameTime gameTime, Rectangle rect)
        {
            // draw hitpoints
            _spriteBatch.DrawString(_font, _hitpointsText, new Vector2(rect.Left, rect.Top), _hitpointsColor);
            DrawBar(new Point(rect.Left + rect.Width / 2, rect.Top), new Point((rect.Width / 2) - 18, 14), _hitpointsColor, 
                CalculatePercent(_player.Hitpoints, _player.MaxHitpoints));

            // draw stamina
            _spriteBatch.DrawString(_font, _staminaText, new Vector2(rect.Left, rect.Top + VERTICAL_SPACING), _staminaColor);
            DrawBar(new Point(rect.Left + rect.Width / 2, rect.Top + VERTICAL_SPACING + 1), new Point((rect.Width / 2) - 18, 14), _staminaColor,
                CalculatePercent(_player.Stamina, _player.MaxStamina));

            _spriteBatch.DrawString(_font, _positionText, new Vector2(rect.Left, rect.Top + VERTICAL_SPACING * 2), _positionColor);
        }

        private static float CalculatePercent(int val, int max)
        {
            return Convert.ToSingle(val) / Convert.ToSingle(max);
        }

        private void DrawBar(Point position, Point size, Color color, float percentage)
        {
            var percSize = new Point(Convert.ToInt32(size.X * percentage), size.Y);
            var barRectOutline = new Rectangle(position, size);
            var barRectInterior = new Rectangle(position.X + 1, position.Y + 1, size.X - 2, size.Y - 2);
            var barRectValue = new Rectangle(position.X + 1, position.Y + 1, percSize.X - 2, percSize.Y - 2);
            _spriteBatch.Draw(_barTexture, barRectOutline, color);
            _spriteBatch.Draw(_barTexture, barRectInterior, _barBgColor);
            _spriteBatch.Draw(_barTexture, barRectValue, color);
        }
    }
}
