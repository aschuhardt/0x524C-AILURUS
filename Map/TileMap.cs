using ailurus.Map.Tiles;
using DryIoc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ailurus.Map
{
    public class TileMap
    {
        private ITile[,] _tiles;
        private TextureMap<TileType> _textures;
        private TextureMap<DecorationType> _decorTextures;
        private Decorations _decorations;
        private SpriteBatch _spriteBatch;
        private int _width;
        private int _height;
        private Container _container;

        public TileMap(TextureMap<TileType> textures, TextureMap<DecorationType> decorTextures, Decorations decorations, SpriteBatch spriteBatch, Container container, Config config)
        {
            _textures = textures;
            _spriteBatch = spriteBatch;
            _decorTextures = decorTextures;
            _decorations = decorations;

            _width = config.MapWidth;
            _height = config.MapWidth;
            _container = container;

            Generate();
        }
        
        protected virtual void Generate()
        {
            _tiles = new ITile[_width, _height];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (SimplexNoise.Noise.Generate(x * 0.07f, y * 0.07f) > 0.5f)
                        _tiles[x, y] = _container.Resolve<GrassTile>();
                    else
                        _tiles[x, y] = _container.Resolve<DirtTile>();
                }
            }
        }

        public void Draw(GameTime gameTime, Rectangle rect, Rectangle region)
        {
            if (region.Width <= 0 || region.Height <= 0) return;

            var tileWidth = rect.Width / region.Width;
            var tileHeight = rect.Height / region.Height;

            var tileSize = new Point(tileWidth, tileHeight);
            
            for (int x = region.Left; x < region.Left + region.Width; x++)
            {
                if (x < 0 || x >= _width) continue;

                for (int y = region.Top; y < region.Top + region.Height; y++)
                {
                    if (y < 0 || y >= _height) continue;

                    var tilePosition = new Point(rect.Left + tileWidth * (x - region.Left), rect.Top + tileHeight * (y - region.Top));

                    _tiles[x, y].Draw(gameTime, new Rectangle(tilePosition, tileSize));
                }
            }
        }
    }
}
