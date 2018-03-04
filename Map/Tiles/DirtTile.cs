using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ailurus.Map.Tiles
{
    public class DirtTile : TileBase
    {
        public DirtTile(TextureMap<TileType> tileTextures, TextureMap<DecorationType> decorTextures, Decorations decorations, SpriteBatch spriteBatch, Random rand)
            : base(tileTextures, decorTextures, decorations, spriteBatch, rand)
        {
            TileType = TileType.Dirt;
            SpriteColor = Color.SandyBrown;

            SetupDecoration();
        }
    }
}
