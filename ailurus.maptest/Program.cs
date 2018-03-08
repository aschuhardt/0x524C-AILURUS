using System;
using System.Diagnostics;
using ailurus.mapgen;

namespace ailurus.maptest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Too few arguments provided.");
                return;
            }

            int w = int.Parse(args[0]);
            int h = int.Parse(args[1]);

            Console.WriteLine($"Width: {w}, Height: {h}");

            var sw = new Stopwatch();
            sw.Start();

            var map = Generator.Generate(w, h);

            sw.Stop();
            var ms = sw.ElapsedMilliseconds;
            Console.WriteLine($"Map generated in {ms} milliseconds.");

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    var cell = map[x, y];
                    char c = ' ';
                    switch (cell.CellType)
                    {
                        case CellType.Nothing:
                            break;
                        case CellType.Wall:
                            c = '#';
                            break;
                        case CellType.Room:
                            c = '_';
                            break;
                        case CellType.Corridor:
                            c = '+';
                            break;
                    }
                    Console.Write(c);
                }
                Console.WriteLine();
            }
        }
    }
}
