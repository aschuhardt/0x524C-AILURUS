using ailurus.mapgen;
using System;
using System.Drawing;

namespace ailurus.mapimage
{
    class Program
    {
        static void Main(string[] args)
        {
            int w = 256, h = 256;
            if (args.Length > 0)
            {
                w = int.Parse(args[0]);
                h = int.Parse(args[1]);
            }
            string path = args.Length > 2 ? args[2] : "output.bmp";

            Cell[,] map = Generator.Generate(w, h);

            using (var bmp = new Bitmap(w, h))
            {
                for (int x = 0; x < w; x++)
                    for (int y = 0; y < h; y++)
                        switch (map[x, y].CellType)
                        {
                            case CellType.Nothing:
                                bmp.SetPixel(x, y, Color.White);
                                break;
                            case CellType.Wall:
                                bmp.SetPixel(x, y, Color.Blue);
                                break;
                            case CellType.Room:
                                bmp.SetPixel(x, y, Color.Green);
                                break;
                            case CellType.Corridor:
                                bmp.SetPixel(x, y, Color.Red);
                                break;
                            default:
                                break;
                        }
                bmp.Save(path);
            }
        }
    }
}
