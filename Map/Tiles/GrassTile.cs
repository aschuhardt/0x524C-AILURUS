using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ailurus.Map.Tiles
{
    public class GrassTile : TileBase
    {
        public GrassTile(TextureMap<TileType> textures, SpriteBatch spriteBatch, Random rand) : base(textures, spriteBatch)
        {
            TileType = TileType.Grass;
            SpriteColor = Color.ForestGreen;
            AnimationInterval = TimeSpan.FromMilliseconds(rand.Next(500, 5000));
        }
    }
}
