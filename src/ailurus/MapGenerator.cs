using System;
using System.Collections.Generic;
using System.Linq;

namespace ailurus
{

    public static class Generator
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

        public class MapGenerationConfig
        {
            public const int DEFAULT_STARTING_ROOM_SIZE = 6;
            public const int DEFAULT_ROOM_PADDING = 2;
            public const int DEFAULT_MIN_SEED_COUNT = 8;
            public const int DEFAULT_MAX_SEED_COUNT = 20;
            public const int DEFAULT_MIN_ROOM_SIZE = 2;
            public const int DEFAULT_MAX_ROOM_SIZE = 8;
            public const int DEFAULT_MAX_SEED_ROOM_PLACEMENT_ATTEMPTS = 4096;
            public const int DEFAULT_MIN_CORRIDOR_WIDTH = 1;
            public const int DEFAULT_MAX_CORRIDOR_WIDTH = 2;
            public const double DEFAULT_SECONDARY_ROOM_DENSITY = 0.1;
            public const int DEFAULT_MIN_SECONDARY_ROOM_DEVIATION = 2;
            public const int DEFAULT_MAX_SECONDARY_ROOM_DEVIATION = 8;
            public const int DEFAULT_SECONDARY_ROOM_COMPUTATION_PASSES = 1;

            public int StartingRoomSize { get; set; }
            public int RoomPadding { get; set; }
            public int MinimumSeedCount { get; set; }
            public int MaximumSeedCount { get; set; }
            public int MinimumRoomSize { get; set; }
            public int MaximumRoomSize { get; set; }
            public int MaximumSeedRoomPlacementAttempts { get; set; }
            public int MinimumCorridorWidth { get; set; }
            public int MaximumCorridorWidth { get; set; }
            public double SecondaryRoomDensity { get; set; }
            public int MinimumSecondaryRoomDeviation { get; set; }
            public int MaximumSecondaryRoomDeviation { get; set; }
            public int SecondaryRoomComputationPasses { get; set; }
            public int? Seed { get; set; }

            public MapGenerationConfig()
            {
                StartingRoomSize = DEFAULT_STARTING_ROOM_SIZE;
                RoomPadding = DEFAULT_ROOM_PADDING;
                MinimumSeedCount = DEFAULT_MIN_SEED_COUNT;
                MaximumSeedCount = DEFAULT_MAX_SEED_COUNT;
                MinimumRoomSize = DEFAULT_MIN_ROOM_SIZE;
                MaximumRoomSize = DEFAULT_MAX_ROOM_SIZE;
                MaximumSeedRoomPlacementAttempts = DEFAULT_MAX_SEED_ROOM_PLACEMENT_ATTEMPTS;
                MinimumCorridorWidth = DEFAULT_MIN_CORRIDOR_WIDTH;
                MaximumCorridorWidth = DEFAULT_MAX_CORRIDOR_WIDTH;
                SecondaryRoomDensity = DEFAULT_SECONDARY_ROOM_DENSITY;
                MinimumSecondaryRoomDeviation = DEFAULT_MIN_SECONDARY_ROOM_DEVIATION;
                MaximumSecondaryRoomDeviation = DEFAULT_MAX_SECONDARY_ROOM_DEVIATION;
                SecondaryRoomComputationPasses = DEFAULT_SECONDARY_ROOM_COMPUTATION_PASSES;
                Seed = null;
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

            public static bool operator ==(Point a, Point b)
            {
                return a.X == b.X && a.Y == b.Y;
            }

            public static bool operator !=(Point a, Point b)
            {
                return a.X != b.X || a.Y != b.Y;
            }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                return (Point)obj == this;
            }

            // override object.GetHashCode
            public override int GetHashCode() => X ^ Y;
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

        public static Cell[,] Generate(int w, int h, MapGenerationConfig config)
        {
            var map = new Cell[w, h];
            var rand = new Random(config.Seed ?? (int)(DateTime.Now.Ticks % int.MaxValue));

            // initialize
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    map[x, y] = new Cell(CellType.Nothing);

            var rooms = new List<Room>();
            var corridors = new List<Corridor>();

            // starting room
            var startingRoom = new Room(new Point(w / 2, h / 2),
                                        config.StartingRoomSize,
                                        config.StartingRoomSize);

            rooms.Add(startingRoom);

            // seed initial rooms
            int seedCount = rand.Next(config.MinimumSeedCount, config.MaximumSeedCount + 1);
            for (int i = 0; i < seedCount; i++)
            {
                int tries = 0;
                int edgePad = config.MaximumRoomSize / 2 + 1;
                Point center = new Point(-1, -1);
                int roomWidth = 0;
                int roomHeight = 0;

                bool valid = false;
                do
                {
                    // stop looking if we've exceeded our max attempts
                    if (tries > config.MaximumSeedRoomPlacementAttempts) break;

                    // choose a random point in the map
                    center = new Point(rand.Next(edgePad, w - edgePad),
                                       rand.Next(edgePad, h - edgePad));

                    // assign random dimensions
                    roomWidth = rand.Next(config.MinimumRoomSize, config.MaximumRoomSize + 1);
                    roomHeight = rand.Next(config.MinimumRoomSize, config.MaximumRoomSize + 1);

                    // check its distance against every other room
                    foreach (var room in rooms)
                    {
                        if (center == room.Center) continue;

                        int minDistance = config.RoomPadding
                                             + Math.Max(room.Width, room.Height) / 2
                                             + Math.Max(roomWidth, roomHeight) / 2;
                        if (!HasTypeSurrounding(ref map, center, new[] { CellType.Corridor, CellType.Room }, minDistance))
                        {
                            valid = true;
                            break;
                        }
                    }
                    tries++;
                } while (!valid);

                // if we couldn't find a suitable room location, then give up
                if (!valid) break;

                rooms.Add(new Room(center, roomWidth, roomHeight));
            }

            // create corridors to starting room
            foreach (var room in rooms)
            {
                if (room.Center != startingRoom.Center)
                {
                    var width = rand.Next(config.MinimumCorridorWidth,
                                          config.MaximumCorridorWidth + 1);
                    var corridor = new Corridor(room.Center,
                                                startingRoom.Center,
                                                width);
                    corridors.Add(corridor);
                    BuildCorridor(ref map, corridor);
                }
            }

            int primaryRoomsCount = rooms.Count;

            for (int n = 0; n < config.SecondaryRoomComputationPasses; n++)
            {
                // create secondary rooms/corridors off of the seeds            
                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        TryPlacingSecondaryRoomFromOrigin(x, y, map, rooms, rand, config);
                    }
                }
            }

            int secondaryRoomsCount = rooms.Count - primaryRoomsCount;
            Console.WriteLine($"{secondaryRoomsCount} secondary rooms were created.");

            // build rooms
            foreach (var room in rooms)
            {
                BuildRoom(ref map, room);
            }

            // build walls
            BuildWalls(ref map);

            return map;
        }

        private static void BuildWalls(ref Cell[,] map)
        {
            int mapWidth = map.GetLength(0);
            int mapHeight = map.GetLength(1);

            for (int x = 0; x < mapWidth; x++)
                for (int y = 0; y < mapHeight; y++)
                    if (map[x, y].CellType == CellType.Nothing)
                    {
                        if (HasTypeSurrounding(ref map, new Point(x, y), new[] { CellType.Corridor, CellType.Room }, 1))
                            map[x, y].CellType = CellType.Wall;
                    }
        }

        private static void TryPlacingSecondaryRoomFromOrigin(int x, int y, Cell[,] map, List<Room> rooms, Random rand, MapGenerationConfig config)
        {
            int mapWidth = map.GetLength(0);
            int mapHeight = map.GetLength(1);

            if (map[x, y].CellType == CellType.Corridor
                                    && rand.NextDouble() < config.SecondaryRoomDensity)
            {
                var origin = new Point(x, y);

                // only look at cells on the edges of corridors
                if (!HasTypeSurrounding(ref map, origin,
                                        new[] { CellType.Nothing }, 1))
                    return;

                // narrow the search for viable room locations
                for (int radius = config.MaximumSecondaryRoomDeviation;
                     radius >= config.MinimumSecondaryRoomDeviation; radius--)
                {
                    // take random samples within a radius
                    for (int i = radius; i < radius * radius; i++)
                    {
                        var sample = new Point(x + rand.Next(-radius, radius + 1),
                                               y + rand.Next(-radius, radius + 1));

                        if (sample.X <= 0 || sample.X >= mapWidth - 1
                            || sample.Y <= 0 || sample.Y >= mapHeight - 1)
                            continue;

                        // if we find a spot that doesn't have anything surrounding it,
                        // then make a new room there
                        if (!HasTypeSurrounding(ref map, sample,
                                                new[] { CellType.Corridor, CellType.Room }, 1))
                        {
                            // try and get away with as big a room as we can
                            for (int size = config.MaximumRoomSize; size >= config.MinimumRoomSize; size--)
                            {
                                bool valid = false;
                                var secondaryRoom = new Room(sample, size, size);

                                // check its distance against every other room
                                foreach (var room in rooms)
                                {
                                    if (sample == room.Center) continue;

                                    int minDistance = config.RoomPadding
                                                        + Math.Max(room.Width, room.Height) / 2
                                                        + size / 2;
                                    if (!HasTypeSurrounding(ref map, sample, new[] { CellType.Corridor, CellType.Room }, minDistance))
                                    {
                                        valid = true;
                                        break;
                                    }
                                }

                                if (valid)
                                {
                                    rooms.Add(secondaryRoom);
                                    BuildCorridor(ref map, origin, secondaryRoom.Center, 1);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool HasTypeSurrounding(ref Cell[,] map, Point p, CellType[] ct, int radius)
        {
            int mapWidth = map.GetLength(0);
            int mapHeight = map.GetLength(1);

            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    if (i == 0 && j == 0) continue;

                    int x = p.X + i;
                    int y = p.Y + j;

                    if (x > 0 && x < mapWidth - 1
                        && y > 0 && y < mapHeight - 1)
                    {
                        if (ct.Contains(map[x, y].CellType))
                            return true;
                    }
                }
            }
            return false;
        }

        private static double Distance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        private static void BuildCorridor(ref Cell[,] map, Corridor c)
        {
            BuildCorridor(ref map, c.Start, c.End, c.Width);
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
