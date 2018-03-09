using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ailurus.Map
{
    public abstract class TileBase : ITile
    {
        public TileType TileType { get; protected set; }

        public bool Obstacle { get; protected set; }

        protected TimeSpan AnimationInterval = TimeSpan.FromSeconds(0.5);
        protected Color SpriteColor = Color.White;
        private DecorationAttribute Decoration;

        private TimeSpan _lastFrameSwitch;
        private int _frameIndexTile;
        private int _frameIndexDecor;
        private TextureMap<TileType> _tileTextures;
        private TextureMap<DecorationType> _decorTextures;
        private SpriteBatch _spriteBatch;
        private Decorations _decorations;
        private Random _rand;

        protected TileBase(TextureMap<TileType> tileTextures, TextureMap<DecorationType> decorTextures, Decorations decorations, SpriteBatch spriteBatch, Random rand)
        {
            _tileTextures = tileTextures;
            _decorTextures = decorTextures;
            _spriteBatch = spriteBatch;
            _rand = rand;
            _decorations = decorations;
            Obstacle = false;
        }

        protected void SetupDecoration()
        {
            var val = _rand.NextDouble();
            var available = _decorations[TileType].Where(x => x.Frequency >= val);
            if (available.Any())
            {
                if (available.Count() > 1)
                    Decoration = available.ElementAt(_rand.Next(available.Count()));
                else
                    Decoration = available.First();
            }
        }

        public void Draw(GameTime gameTime, Rectangle rect)
        {
            var availableTextures = _tileTextures[TileType];

            _spriteBatch.Draw(availableTextures[_frameIndexTile], rect, SpriteColor);

            if (Decoration != null)
            {
                _spriteBatch.Draw(_decorTextures[Decoration.DecorationType][_frameIndexDecor], rect, Decoration.Color);
            }

            var elapsed = gameTime.TotalGameTime;
            if (elapsed - _lastFrameSwitch >= AnimationInterval)
            {
                if (availableTextures.Count > 1)
                {
                    if (_frameIndexTile < availableTextures.Count - 1)
                        _frameIndexTile++;
                    else
                        _frameIndexTile = 0;
                }

                if (Decoration != null)
                {
                    var decorTextures = _decorTextures[Decoration.DecorationType];
                    if (decorTextures.Count > 1)
                    {
                        if (_frameIndexDecor < decorTextures.Count - 1)
                            _frameIndexDecor++;
                        else
                            _frameIndexDecor = 0;
                    }
                }

                _lastFrameSwitch = elapsed;
            }
        }
    }
}
