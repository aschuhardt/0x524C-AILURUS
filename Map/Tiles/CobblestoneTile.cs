using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace ailurus.Map.Tiles
{
    class CobblestoneTile : TileBase
    {
        protected CobblestoneTile(TextureMap<TileType> tileTextures, TextureMap<DecorationType> decorTextures, Decorations decorations, SpriteBatch spriteBatch, Random rand) 
            : base(tileTextures, decorTextures, decorations, spriteBatch, rand)
        {
        }
    }
}
