using Microsoft.Xna.Framework;

namespace ailurus.Map
{
    public interface ITile
    {
        TileType TileType { get; }
        void Draw(GameTime gameTime, Rectangle rect);
    }
}
