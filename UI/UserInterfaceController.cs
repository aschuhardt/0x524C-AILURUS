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

        public Color BgColor { get; }
        public Color BaseColor { get; }

        private string _hitpointsText;
        private string _staminaText;
        private string _positionText;
        private Color _hitpointsColor;
        private Color _staminaColor;
        private Texture2D _basicTexture;
        private const int VERTICAL_SPACING = 14;
        private const int MESSAGES_PAD = 8;
        private Rectangle _messagesArea;
        private List<Message> _messages;

        public UserInterfaceController(PlayerController player, SpriteFont font, SpriteBatch spriteBatch, GraphicsDeviceManager graphics, List<Message> messages)
        {
            _player = player;
            _font = font;
            _spriteBatch = spriteBatch;
            _messages = messages;

            _hitpointsColor = new Color(124, 58, 66);
            _staminaColor = new Color(130, 128, 60);
            BaseColor = new Color(106, 112, 123);

            _basicTexture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _basicTexture.SetData(new[] { Color.White });
            BgColor = new Color(18, 21, 26);
        }

        public void UnloadContent()
        {
            _basicTexture.Dispose();
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

            _spriteBatch.DrawString(_font, _positionText, new Vector2(rect.Left, rect.Top + VERTICAL_SPACING * 2), BaseColor);

            var msgTop = rect.Top + VERTICAL_SPACING * 3;
            _messagesArea = new Rectangle(rect.Left, msgTop + MESSAGES_PAD, rect.Width - MESSAGES_PAD * 2, rect.Height - msgTop - MESSAGES_PAD * 2);

            DrawRect(_messagesArea, BaseColor);
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
            _spriteBatch.Draw(_basicTexture, barRectOutline, color);
            _spriteBatch.Draw(_basicTexture, barRectInterior, BgColor);
            _spriteBatch.Draw(_basicTexture, barRectValue, color);
        }

        public void DrawRect(Rectangle rect, Color color, int padding = 1)
        {
            var barRectInterior = rect;
            rect.Inflate(padding + 1, padding + 1);
            barRectInterior.Inflate(padding, padding);
            _spriteBatch.Draw(_basicTexture, rect, color);
            _spriteBatch.Draw(_basicTexture, barRectInterior, BgColor);
        }
    }
}
