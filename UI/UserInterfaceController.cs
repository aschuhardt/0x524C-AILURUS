using ailurus.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private const int MESSAGE_TEXT_LEFT_PAD = 4;
        private const int MESSAGE_TEXT_BOTTOM_PAD = 4;
        private Rectangle _messagesArea;
        private List<Message> _messages;

        private Color _msgColorInfo;
        private Color _msgColorHealing;
        private Color _msgColorDialog;
        private Color _msgColorCombat;

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

            _msgColorInfo = new Color(130, 108, 60);
            _msgColorHealing = new Color(47, 101, 54);
            _msgColorCombat = new Color(124, 58, 66);
            _msgColorDialog = new Color(115, 109, 126);
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

            int lineHeight = Convert.ToInt32(Math.Floor(_font.MeasureString("A").Y));
            int maxMsgLines = _messagesArea.Height / lineHeight;
            int currentLines = 0;
            foreach (var msg in _messages.OrderByDescending(x => x.Timestamp))
            {
                if (currentLines >= maxMsgLines) break;
                var text = WrapText(_font, msg.Contents, _messagesArea.Width - MESSAGE_TEXT_LEFT_PAD);
                var lineCount = Convert.ToInt32(Math.Round(_font.MeasureString(text).Y)) / lineHeight;
                currentLines += lineCount;
                
                float alpha = 1.0f - Convert.ToSingle(Math.Pow(Convert.ToSingle(currentLines) / Convert.ToSingle(maxMsgLines), 4));

                Color color;
                switch (msg.MessageType)
                {
                    case MessageType.Info:
                        color = _msgColorInfo;
                        break;
                    case MessageType.Dialog:
                        color = _msgColorDialog;
                        break;
                    case MessageType.Combat:
                        color = _msgColorCombat;
                        break;
                    case MessageType.Healing:
                        color = _msgColorHealing;
                        break;
                    default:
                        color = _msgColorInfo;
                        break;
                }

                _spriteBatch.DrawString(_font, text, 
                    new Vector2(_messagesArea.Left + MESSAGE_TEXT_LEFT_PAD, _messagesArea.Bottom - lineHeight * currentLines), 
                    new Color(color, alpha));
            }
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

        public static string WrapText(SpriteFont font, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = font.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = font.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    if (size.X > maxLineWidth)
                    {
                        if (sb.ToString() == "")
                        {
                            sb.Append(WrapText(font, word.Insert(word.Length / 2, " ") + " ", maxLineWidth));
                        }
                        else
                        {
                            sb.Append("\n" + WrapText(font, word.Insert(word.Length / 2, " ") + " ", maxLineWidth));
                        }
                    }
                    else
                    {
                        sb.Append("\n" + word + " ");
                        lineWidth = size.X + spaceWidth;
                    }
                }
            }

            return sb.ToString();
        }
    }
}
