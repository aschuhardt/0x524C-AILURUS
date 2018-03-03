using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ailurus.Map
{
    public abstract class TileBase : ITile
    {
        public TileType TileType { get; protected set; }

        protected TimeSpan AnimationInterval = TimeSpan.FromSeconds(0.5);

        private TimeSpan _lastFrameSwitch;
        private int _frameIndex;
        private TextureMap<TileType> _textures;
        private SpriteBatch _spriteBatch;

        protected TileBase(TextureMap<TileType> textures, SpriteBatch spriteBatch)
        {
            _textures = textures;
            _spriteBatch = spriteBatch;
        }

        public void Draw(GameTime gameTime, Rectangle rect)
        {
            var availableTextures = _textures[TileType];

            _spriteBatch.Draw(availableTextures[_frameIndex], rect, Color.White);

            if (availableTextures.Count == 1) return;

            var elapsed = gameTime.ElapsedGameTime;
            if (elapsed - _lastFrameSwitch >= AnimationInterval)
            {
                if (_frameIndex < _textures[TileType].Count - 1)
                    _frameIndex++;
                else
                    _frameIndex = 0;
                _lastFrameSwitch = elapsed;
            }
        }

        public virtual void Update(GameTime gameTime) { }
    }
}
