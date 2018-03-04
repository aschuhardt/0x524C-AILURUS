using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ailurus.Map.Tiles
{
    public class GrassTile : TileBase
    {
        public GrassTile(TextureMap<TileType> tileTextures, TextureMap<DecorationType> decorTextures, Decorations decorations, SpriteBatch spriteBatch, Random rand)
            : base(tileTextures, decorTextures, decorations, spriteBatch, rand)
        {
            TileType = TileType.Grass;
            SpriteColor = Color.ForestGreen;

            SetupDecoration();
        }
    }
}
