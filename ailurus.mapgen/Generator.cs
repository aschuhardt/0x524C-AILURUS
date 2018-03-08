using System;
using System.Collections.Generic;

namespace ailurus.mapgen
{
    public enum CellType
    {
        Nothing = 0,
        Wall,
        Room,
        Corridor
    }

    public struct Cell
    {
        public CellType CellType;

        public Cell(CellType cellType)
        {
            CellType = cellType;
        }
    }

    struct Room
    {
        public Point Center;
        public int Width, Height;

        public Room(Point center, int w, int h)
        {
            Center = center;
            Width = w;
            Height = h;
        }
    }

    struct Point
    {
        public int X, Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    struct Corridor
    {
        public Point Start, End;
        public int Width;

        public Corridor(Point a, Point b, int w)
        {
            Start = a;
            End = b;
            Width = w;
        }
    }

    public static class Generator
    {
        private const int STARTING_ROOM_SIZE = 6;
        public static Cell[,] Generate(int w, int h)
        {
            Cell[,] map = new Cell[w, h];

            // initialize
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    map[x, y] = new Cell(CellType.Nothing);

            // starting room
            var startingRoom = new Room(new Point(w / 2, h / 2),
                                        STARTING_ROOM_SIZE,
                                        STARTING_ROOM_SIZE);

            BuildRoom(ref map, startingRoom);

            return map;
        }

        private static void BuildCorridor(ref Cell[,] map, Point current,
                                          Point end, int width)
        {
            int mapWidth = map.GetLength(0);
            int mapHeight = map.GetLength(1);

            // fill in surrounding area
            int radius = width / 2;
            for (int x = current.X - radius; x <= current.X + radius; x++)
                for (int y = current.Y - radius; y <= current.Y + radius; y++)
                    if (x > 0 && y > 0 && x < mapWidth - 1 && y < mapHeight - 1)
                        // leave a single-cell-wide border
                        if (map[x, y].CellType != CellType.Corridor)
                            map[x, y].CellType = CellType.Corridor;

            if (current.X != end.X)
            {
                if (current.X > end.X)
                    BuildCorridor(ref map, new Point(current.X - 1, current.Y),
                                  end, width);
                else
                    BuildCorridor(ref map, new Point(current.X + 1, current.Y),
                                  end, width);
            }
            else if (current.Y != end.Y)
            {
                if (current.Y > end.Y)
                    BuildCorridor(ref map, new Point(current.X, current.Y - 1),
                                  end, width);
                else
                    BuildCorridor(ref map, new Point(current.X, current.Y + 1),
                                  end, width);
            }
        }

        private static void BuildRoom(ref Cell[,] map, Room room)
        {
            int mapWidth = map.GetLength(0);
            int mapHeight = map.GetLength(1);
            int halfWidth = room.Width / 2;
            int halfHeight = room.Height / 2;
            int cx = room.Center.X;
            int cy = room.Center.Y;

            for (int x = cx - halfWidth; x <= cx + halfWidth; x++)
                for (int y = cy - halfHeight; y <= cy + halfHeight; y++)
                    if (x > 0 && y > 0 && x < mapWidth - 1 && y < mapHeight - 1)
                        map[x, y].CellType = CellType.Room;
        }
    }
}
