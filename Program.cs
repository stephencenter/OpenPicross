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

        private static void CLITest() 
        {
            var file = "TestPuzzles/test_a.png";
                
            Console.WriteLine($"Loading '{file}'...");
            var loaded_puzzle = PuzzleLoader.LoadPuzzleFromPNG(file);

            while (true) 
            {
                loaded_puzzle.PrintGuide(loaded_puzzle.SolutionMap);

                PuzzleMap.PrintPixelMap(loaded_puzzle.PlayerMap);

                if (loaded_puzzle.CheckForVictory()) 
                {
                    Console.WriteLine("You won!");
                    break;
                }    

                Console.Write("Enter command ([t]oggle or [i]gnore x y): ");
                
                while (true) 
                {
                    var input = Console.ReadLine().ToLower().Trim().Split();
                    
                    if (input.Length != 3) 
                    {
                        continue;
                    }

                    if (!int.TryParse(input[1], out int x)) 
                    {
                        continue;
                    }

                    if (!int.TryParse(input[2], out int y)) 
                    {
                        continue;
                    }

                    if (input[0].StartsWith("t")) 
                    {
                        loaded_puzzle.PixelToggleOnOff(x - 1, y - 1);
                        break;
                    }

                    else if (input[0].StartsWith("i")) 
                    {
                        loaded_puzzle.PixelToggleIgnored(x - 1, y - 1);
                        break;
                    }
                }            
            }
        }
    }
}
