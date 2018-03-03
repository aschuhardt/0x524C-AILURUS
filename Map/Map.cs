using System;
using System.Collections.Generic;
using System.Text;

namespace ailurus.Map
{
    public class Map
    {
        private ITile[,] _tiles;
        private TextureMap<TileType> _textures;

        public Map(int w, int h, TextureMap<TileType> textures)
        {
            _tiles = new ITile[w, h];
            _textures = textures;
        }
    }
}
