using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace ailurus.Map.Tiles
{
    public class GrassTile : TileBase
    {
        protected GrassTile(IDictionary<TileType, IList<Texture2D>> textures, SpriteBatch spriteBatch) : base(textures, spriteBatch)
        {
            TileType = TileType.Grass;
        }
    }
}
