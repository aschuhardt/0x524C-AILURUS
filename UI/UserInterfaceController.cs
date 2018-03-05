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
        private const int VERTICAL_SPACING = 14;

        public UserInterfaceController(PlayerController player, SpriteFont font, SpriteBatch spriteBatch)
        {
            _player = player;
            _font = font;
            _spriteBatch = spriteBatch;

            _hitpointsColor = Color.White;
            _staminaColor = Color.White;
            _positionColor = Color.White;
        }

        public void Update(GameTime gameTime)
        {
            _hitpointsText = $"HP: {_player.Hitpoints}/{_player.MaxHitpoints}";
            _staminaText = $"ST: {_player.Stamina}/{_player.MaxStamina}";
            _positionText = $"X: {_player.Position.X}, Y: {_player.Position.Y}";
        }

        public void Draw(GameTime gameTime, Rectangle rect)
        {
            _spriteBatch.DrawString(_font, _hitpointsText, new Vector2(rect.Left, rect.Top), _hitpointsColor);
            _spriteBatch.DrawString(_font, _staminaText, new Vector2(rect.Left, rect.Top + VERTICAL_SPACING), _staminaColor);
            _spriteBatch.DrawString(_font, _positionText, new Vector2(rect.Left, rect.Top + VERTICAL_SPACING * 2), _positionColor);
        }
    }
}
