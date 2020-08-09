using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System;

namespace Picross 
{
    public static class PuzzleLoader 
    {
        public static PuzzleMap LoadPuzzleFromPNG(string file) 
        {
            Bitmap image = new Bitmap(file);

            // Create a 2D-Array of booleans, with each element corresponding to a pixel in the PNG. 
            // True means the pixel is filled in, false means it's empty
            var pixel_map = new bool[image.Width, image.Height];

            for (int x = 0; x < image.Width; x++) 
            {                
                for (int y = 0; y < image.Height; y++)
                {
                    Color p_color = image.GetPixel(x, y);
                    if (p_color.R == 255 && p_color.B == 255 && p_color.G == 255) 
                    {
                        pixel_map[x, y] = false;
                    }
                        
                    else
                    {
                        pixel_map[x, y] = true;
                    }
                }
            }
            
            var hints = PuzzleMap.GetPuzzleHints(pixel_map);

            return new PuzzleMap(pixel_map, hints);
        }
    }
}