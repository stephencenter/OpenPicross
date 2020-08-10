using System;

namespace Picross
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new OpenPicross()) 
            {
                game.Run();
            }
        }
    }
}
