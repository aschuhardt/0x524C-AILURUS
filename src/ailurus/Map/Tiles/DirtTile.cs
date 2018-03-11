using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ailurus.Map.Tiles
{
    public class DirtTile : TileBase
    {
        private const int COLOR_VARIANCE = 5;

        public DirtTile(TextureMap<TileType> tileTextures, TextureMap<DecorationType> decorTextures, Decorations decorations, SpriteBatch spriteBatch, Random rand)
            : base(tileTextures, decorTextures, decorations, spriteBatch, rand)
        {
            TileType = TileType.Dirt;

            int variation = rand.Next(-COLOR_VARIANCE, COLOR_VARIANCE);
            SpriteColor = new Color(40 + variation, 31 + variation, 25 + variation);


            SetupDecoration();
        }
    }
}
