using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
using System;

namespace Picross
{
    public class PuzzleMap 
    {
        public Pixel[,] SolutionMap  { get; }
        public Pixel[,] PlayerMap { get; set; }

        /**********************
        *    STATIC METHODS   *
        ***********************/
        #region 

        public static PuzzleGuide GetPuzzleGuide(Pixel[,] pixel_map) 
        {
            // Use the pixel map to generate the picross guide
            var column_guide = new List<List<int>>() {};
            var row_guide = new List<List<int>>() {};

            for (int x = 0; x < pixel_map.GetLength(0); x++) 
            {
                var current_column = new List<int>() { 0 };
                var last_pixel = false;
                
                for (int y = 0; y < pixel_map.GetLength(1); y++)
                {
                    if (pixel_map[x, y].State == PixelState.On) 
                    {
                        current_column[current_column.Count - 1]++;
                        last_pixel = true;
                    }
                        
                    else
                    {
                        if (last_pixel)
                        {
                            current_column.Add(0);
                        }

                        last_pixel = false;
                    }
                }
                
                if (current_column.Count > 1 && current_column.Last() == 0)
                {
                    current_column.RemoveAt(current_column.Count - 1);
                }
                    
                column_guide.Add(current_column);
            }  

            for (int y = 0; y < pixel_map.GetLength(1); y++) 
            {
                var current_row = new List<int>() { 0 };
                var last_pixel = false;
                
                for (int x = 0; x < pixel_map.GetLength(0); x++)
                {
                    if (pixel_map[x, y].State == PixelState.On) 
                    {
                        current_row[current_row.Count - 1]++;
                        last_pixel = true;
                    }
                        
                    else
                    {
                        if (last_pixel)
                        {
                            current_row.Add(0);
                        }

                        last_pixel = false;
                    }
                }
                
                if (current_row.Count > 1 && current_row.Last() == 0)
                {
                    current_row.RemoveAt(current_row.Count - 1);
                }
                    
                row_guide.Add(current_row);
            }

            return new PuzzleGuide(column_guide, row_guide);
        }

        public static void PrintPixelMap(Pixel[,] pixel_map) 
        {
            for (int y = 0; y < pixel_map.GetLength(1); y++) 
            {   
                string row = "";

                for (int x = 0; x < pixel_map.GetLength(0); x++)
                {
                    if (pixel_map[x, y].State == PixelState.On) 
                    {
                        row += "X";
                    }
                    
                    else if (pixel_map[x, y].State == PixelState.Ignored) 
                    {
                        row += ".";
                    }

                    else
                    {
                        row += "·";
                    }
                }

                Console.WriteLine(row);
            }
        }

        #endregion

        /**********************
        *   INSTANCE METHODS  *
        ***********************/
        #region 

        public void PixelToggleOnOff(int x, int y) 
        {
            // Pixels with the "Ignored" state cannot be toggled on or off,
            // they must be unignored first

            if (PlayerMap[x, y].State == PixelState.Off) 
            {
                PlayerMap[x, y].State = PixelState.On;
            }

            else if (PlayerMap[x, y].State == PixelState.On) 
            {
                PlayerMap[x, y].State = PixelState.Off;
            }
        }

        public void PixelToggleIgnored(int x, int y) 
        {
            // Pixels with the "On" state cannot be ignored,
            // they must be toggled off first

            if (PlayerMap[x, y].State == PixelState.Off) 
            {
                PlayerMap[x, y].State = PixelState.Ignored;
            }

            else if (PlayerMap[x, y].State == PixelState.Ignored) 
            {
                PlayerMap[x, y].State = PixelState.Off;
            }
        }

        public bool CheckForVictory() 
        {
            // We calculate the puzzle guide that would have been generated
            // if the player's current map was in the game. If their guide matches
            // the intended solution guide, then that means it's a valid solution.
            // Since there isn't a 1-to-1 correspondence between puzzle guides and
            // pixel maps, that means it's possible for the player to finish some
            // puzzles without using the intended solution!
            var solution_guide = GetPuzzleGuide(SolutionMap);
            var player_guide = GetPuzzleGuide(PlayerMap);

            var s_col = solution_guide.Columns;
            var p_col = player_guide.Columns;
            var s_row = solution_guide.Rows;
            var p_row = player_guide.Rows;
            
            for (int column = 0; column < s_col.Count; column++) 
            {
                if (!s_col[column].SequenceEqual(p_col[column])) 
                {
                    return false;
                }
            }

            for (int row = 0; row < s_row.Count; row++) 
            {
                if (!s_row[row].SequenceEqual(p_row[row])) 
                {
                    return false;
                }
            }

            return true;
        }

        public void PrintGuide(Pixel[,] pixel_map) 
        {
            Console.WriteLine("ROWS: ");
            foreach (List<int> row in GetPuzzleGuide(pixel_map).Rows) 
            {
                Console.WriteLine(String.Join(" ", row));
            }

            Console.WriteLine();
            Console.WriteLine("COLUMNS: ");
            foreach (List<int> column in GetPuzzleGuide(pixel_map).Columns) 
            {
                Console.WriteLine(String.Join(" ", column));
            }
        }

        #endregion

        /**********************
        *     CONSTRUCTOR     *
        ***********************/
        public PuzzleMap(Pixel[,] pixel_map)
        {
            SolutionMap = pixel_map;
            PlayerMap = new Pixel[pixel_map.GetLength(0), pixel_map.GetLength(1)];
        }
    }

    public class Pixel 
    {
        public PixelState State { get; set; }
        public Texture2D Sprite { get; set; }
        public Vector2 Position { get; set; }

        public Pixel() 
        {
            Sprite = Logic.Content.Load<Texture2D>("Sprites/pixel_off");
        }
    }

    public enum PixelState
    {
        Off = 0,
        On,
        Ignored
    }

    public class PuzzleGuide
    {
        public List<List<int>> Columns { get; }
        public List<List<int>> Rows { get; }

        public PuzzleGuide(List<List<int>> columns, List<List<int>> rows)
        {
            Columns = columns;
            Rows = rows;
        }
    }
}