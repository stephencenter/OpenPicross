using System.Collections.Generic;
using System.Linq;
using System;

namespace Picross
{
    public class PuzzleMap 
    {
        public bool[,] SolutionMap  { get; }
        public PuzzleHints Hints { get; }
        public bool[,] PlayerMap { get; set; }

        /**********************
        *    STATIC METHODS   *
        ***********************/
        #region 

        public static PuzzleHints GetPuzzleHints(bool[,] pixel_map) 
        {
            // Use the pixel map to generate the picross hints
            var column_hints = new List<List<int>>() {};
            var row_hints = new List<List<int>>() {};

            for (int x = 0; x < pixel_map.GetLength(0); x++) 
            {
                var current_column = new List<int>() { 0 };
                var last_pixel = false;
                
                for (int y = 0; y < pixel_map.GetLength(1); y++)
                {
                    if (pixel_map[x, y]) 
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
                    
                column_hints.Add(current_column);
            }  

            for (int y = 0; y < pixel_map.GetLength(1); y++) 
            {
                var current_row = new List<int>() { 0 };
                var last_pixel = false;
                
                for (int x = 0; x < pixel_map.GetLength(0); x++)
                {
                    if (pixel_map[x, y]) 
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
                    
                row_hints.Add(current_row);
            }

            return new PuzzleHints(column_hints, row_hints);
        }

        public static void PrintPixelMap(bool[,] pixel_map) 
        {
            for (int y = 0; y < pixel_map.GetLength(1); y++) 
            {   
                string row = "";

                for (int x = 0; x < pixel_map.GetLength(0); x++)
                {
                    if (pixel_map[x, y]) 
                    {
                        row += "X";
                    }
                        
                    else
                    {
                        row += " ";
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

        public void PrintHints() 
        {
            Console.WriteLine("ROWS: ");
            foreach (List<int> row in Hints.Rows) 
            {
                Console.WriteLine(String.Join(" ", row));
            }

            Console.WriteLine();
            Console.WriteLine("COLUMNS: ");
            foreach (List<int> column in Hints.Columns) 
            {
                Console.WriteLine(String.Join(" ", column));
            }
        }

        #endregion

        /**********************
        *     CONSTRUCTOR     *
        ***********************/
        public PuzzleMap(bool[,] pixel_map, PuzzleHints hints)
        {
            SolutionMap = pixel_map;
            Hints = hints;
            PlayerMap = new bool[pixel_map.GetLength(0), pixel_map.GetLength(1)];
        }
    }

    public class PuzzleHints 
    {
        public List<List<int>> Columns { get; }
        public List<List<int>> Rows { get; }

        public PuzzleHints(List<List<int>> columns, List<List<int>> rows)
        {
            Columns = columns;
            Rows = rows;
        }
    }
}