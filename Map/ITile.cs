using Microsoft.Xna.Framework;

namespace ailurus.Map
{
    public interface ITile
    {
        TileType TileType { get; }
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime, Rectangle rect);
    }
}
