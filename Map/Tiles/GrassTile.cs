using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ailurus.Map.Tiles
{
    public class GrassTile : TileBase
    {
        private const int COLOR_VARIANCE = 5;

        public GrassTile(TextureMap<TileType> tileTextures, TextureMap<DecorationType> decorTextures, Decorations decorations, SpriteBatch spriteBatch, Random rand)
            : base(tileTextures, decorTextures, decorations, spriteBatch, rand)
        {
            TileType = TileType.Grass;
            AnimationInterval = TimeSpan.FromMilliseconds(rand.Next(1000, 3000));

            int variation = rand.Next(-COLOR_VARIANCE, COLOR_VARIANCE);
            SpriteColor = new Color(28 + variation, 42 + variation, 9 + variation);
            SetupDecoration();
        }
    }
}
