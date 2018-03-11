using Microsoft.Xna.Framework;

namespace ailurus.Map
{
    public interface ITile
    {
        TileType TileType { get; }
        bool Obstacle { get; }
        bool Revealed { get; set; }
        void Draw(GameTime gameTime, Rectangle rect, bool visible = true);
    }
}
