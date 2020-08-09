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
                var file = "TestPuzzles/test6.png";
                
                Console.WriteLine($"Loading '{file}'...");
                var loaded_puzzle = PuzzleLoader.LoadPuzzleFromPNG(file);

                loaded_puzzle.PrintHints();
                PuzzleMap.PrintPixelMap(loaded_puzzle.PlayerMap);

                while (true) 
                {
                    
                }

                // game.Run();
            }
        }
    }
}
