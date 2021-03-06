﻿using ailurus.Map.Tiles;
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
        public int Width => _width;
        public int Height => _height;

        private ITile[,] _tiles;
        private TextureMap<TileType> _textures;
        private TextureMap<DecorationType> _decorTextures;
        private Decorations _decorations;
        private SpriteBatch _spriteBatch;
        private int _width;
        private int _height;
        private Container _container;
        private Random _rand;
        private Generator.MapGenerationConfig _mapGenConfig;

        public TileMap(TextureMap<TileType> textures, TextureMap<DecorationType> decorTextures, Decorations decorations, SpriteBatch spriteBatch, Container container, Config config, Random rand, Generator.MapGenerationConfig mapGenConfig)
        {
            _textures = textures;
            _spriteBatch = spriteBatch;
            _decorTextures = decorTextures;
            _decorations = decorations;
            _mapGenConfig = mapGenConfig;

            _width = config.MapWidth;
            _height = config.MapWidth;
            _container = container;
            _rand = rand;

            Generate();
        }

        public Point GetPlayerStartingPosition()
        {
            return new Point(_width / 2, _height / 2);
        }

        public Rectangle GetScreenCoordinates(Point p, Rectangle drawRect, Rectangle drawRegion)
        {
            var tileWidth = drawRect.Width / drawRegion.Width;
            var tileHeight = drawRect.Height / drawRegion.Height;

            var pos = new Point(drawRect.Left + tileWidth * (p.X - drawRegion.Left), drawRect.Top + tileHeight * (p.Y - drawRegion.Top));
            return new Rectangle(pos, new Point(tileWidth, tileHeight));
        }

        protected virtual void Generate()
        {
            _tiles = new ITile[_width, _height];

            var dungeonMap = Generator.Generate(_width, _height, _mapGenConfig);

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    var cellType = dungeonMap[x, y].CellType;
                    if (cellType == Generator.CellType.Wall)
                    {
                        _tiles[x, y] = _container.Resolve<CobblestoneTile>();
                    }
                    else if (cellType == Generator.CellType.Room
                             || cellType == Generator.CellType.Corridor)
                    {
                        if (SimplexNoise.Noise.Generate(x * 0.07f, y * 0.07f) > 0.5f - _rand.NextDouble())
                            _tiles[x, y] = _container.Resolve<GrassTile>();
                        else
                            _tiles[x, y] = _container.Resolve<DirtTile>();
                    }
                }
            }
        }

        public ITile GetRelativeTile(Point coords, Rectangle region)
        {
            int x = coords.X + region.Left;
            int y = coords.Y + region.Top;
            if (x > 0 && x < _width && y > 0 && y < _height)
                return _tiles[coords.X + region.Left, coords.X + region.Top];
            else
                return null;
        }

        public ITile GetAbsoluteTile(Point coords)
        {
            int x = coords.X;
            int y = coords.Y;
            if (x > 0 && x < _width && y > 0 && y < _height)
                return _tiles[coords.X, coords.Y];
            else
                return null;
        }

        public void Draw(GameTime gameTime, Rectangle rect, Rectangle region, int visibleRadius)
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

                    if (Helpers.Distance(region.Center.X, region.Center.Y, x, y) > visibleRadius
                        && !(_tiles[x, y]?.Revealed ?? false))
                        continue;

                    var tile = _tiles[x, y];
                    if (tile == null) continue;

                    bool draw = false;
                    Bresenhams.Line(region.Center.X, region.Center.Y, x, y,
                                               (tx, ty) =>
                                               {
                                                   if (tx > 0 && tx < _width && ty > 0 && ty < _height)
                                                   {
                                                       if (tx == x && ty == y)
                                                           draw = true;
                                                       else if (tx == region.Center.X && ty == region.Center.Y)
                                                           return true;

                                                       if (_tiles[tx, ty]?.Obstacle ?? true)
                                                           return false; // stop
                                                       else
                                                           return true;  // continue
                                                   }
                                                   else
                                                   {
                                                       return false;
                                                   }
                                               });

                    if (draw)
                    {
                        tile.Revealed = true;
                        tile.Draw(gameTime, GetScreenCoordinates(new Point(x, y), rect, region));
                    }
                    else if (tile.Revealed)
                    {
                        tile.Draw(gameTime, GetScreenCoordinates(new Point(x, y), rect, region), false);
                    }
                }
            }
        }
    }
}
