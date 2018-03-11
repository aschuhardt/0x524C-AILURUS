using System;

namespace ailurus
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GameState())
                game.Run();
        }
    }
}
