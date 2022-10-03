using System;

namespace ProjectTron
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Tron())
                game.Run();
        }
    }
}
