using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ailurus.Map.Tiles
{
    class CobblestoneTile : TileBase
    {
        private const int COLOR_VARIANCE = 5;
        public CobblestoneTile(TextureMap<TileType> tileTextures, TextureMap<DecorationType> decorTextures, Decorations decorations, SpriteBatch spriteBatch, Random rand) 
            : base(tileTextures, decorTextures, decorations, spriteBatch, rand)
        {
            TileType = TileType.Stone;
            Obstacle = true;

            int variation = rand.Next(-COLOR_VARIANCE, COLOR_VARIANCE);
            SpriteColor = new Color(44 + variation, 51 + variation, 42 + variation);
        }
    }
}
