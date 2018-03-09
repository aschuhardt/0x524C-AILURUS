using Microsoft.Xna.Framework;

namespace ailurus.Map
{
    public interface ITile
    {
        TileType TileType { get; }
        bool Obstacle { get; }
        void Draw(GameTime gameTime, Rectangle rect);
    }
}
