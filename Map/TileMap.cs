using ailurus.Map.Tiles;
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
        private SpriteBatch _spriteBatch;
        private int _width;
        private int _height;
        private Random _rand;

        public TileMap(int w, int h, TextureMap<TileType> textures, SpriteBatch spriteBatch)
        {
            _width = w;
            _height = h;
            _textures = textures;
            _spriteBatch = spriteBatch;
            _rand = new Random();
            Generate();
        }

        protected virtual void Generate()
        {
            _tiles = new ITile[_width, _height];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _tiles[x, y] = new GrassTile(_textures, _spriteBatch, _rand);
                }
            }
        }

        public void Draw(GameTime gameTime, Rectangle rect)
        {
            var tileWidth = rect.Width / _width;
            var tileHeight = rect.Height / _height;

            var tileSize = new Point(tileWidth, tileHeight);

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    var tilePosition = new Point(rect.Left + tileWidth * x, rect.Top + tileHeight * y);

                    _tiles[x, y].Draw(gameTime, new Rectangle(tilePosition, tileSize));
                }
            }
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}
