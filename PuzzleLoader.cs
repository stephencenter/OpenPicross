using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Picross 
{
    public static class PuzzleLoader 
    {
        public static PuzzleTemplate LoadPuzzleFromPNG() 
        {
            Bitmap image = new Bitmap("TestPuzzles/test7.png");

            var row_list = new List<List<bool>>() {};

            // Convert the file to a list of lists, where each nested list corresponds to a row
            // of pixels in the file
            for (int row = 0; row < image.Height; row++) 
            {
                var current_row = new List<bool>() {};
                
                for (int pixel = 0; pixel < image.Width; pixel++)
                {
                    Color p_color = image.GetPixel(pixel, row);
                    if (p_color.R == 255 && p_color.B == 255 && p_color.G == 255) 
                    {
                        current_row.Add(false);
                    }
                        
                    else
                    {
                        current_row.Add(true);
                    }
                }
                        
                row_list.Add(current_row);
            }

            // Use the row list to create a column list, where each nested list corresponds to a column
            // of pixels in the file
            var column_list = new List<List<bool>>() {};
            for (int i = 0; i < image.Width; i++) 
            {
                column_list.Add(new List<bool>());
            }

            foreach (List<bool> row in row_list) 
            {
                int column = 0;
                foreach (bool pixel in row) 
                {
                    column_list[column].Add(pixel);
                    column++;
                }

            }

            // Use the rows and columns to create the picross hints
            var row_hints = new List<List<int>>() {};
            var column_hints = new List<List<int>>() {};

            foreach (List<bool> row in row_list) 
            {
                var current_row = new List<int>() { 0 };
                var last_pixel = false;
                
                foreach (bool pixel in row)
                {
                    if (pixel) 
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

            foreach (List<bool> column in column_list) 
            {
                var current_column = new List<int>() { 0 };
                var last_pixel = false;
                
                foreach (bool pixel in column)
                {
                    if (pixel) 
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

            return new PuzzleTemplate(row_list, row_hints, column_hints);
        }
    }

    public class PuzzleTemplate 
    {
        public List<List<bool>> PixelMap  { get; set; }
        public List<List<int>> RowHints { get; set; }
        public List<List<int>> ColumnHints { get; set; }

        public PuzzleTemplate(List<List<bool>> pixel_map, List<List<int>> r_hints, List<List<int>> c_hints)
        {
            PixelMap = pixel_map;
            RowHints = r_hints;
            ColumnHints = c_hints;
        }
    }
}