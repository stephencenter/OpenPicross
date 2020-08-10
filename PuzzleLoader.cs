using System.Drawing;
using Microsoft.Xna.Framework;

namespace Picross 
{
    public static class PuzzleLoader 
    {
        public static readonly Vector2 board_origin = new Vector2(776, 300);
        public static readonly Vector2 colguide_origin = new Vector2(776, 300);
        public const int internal_width = 1920;
        public const int internal_height = 1080;
        private const int board_max = 734;
        public static int board_width_px;
        public static int board_height_px;
        public static int pixel_size;

        public static PuzzleMap LoadPuzzleFromPNG(string file) 
        {
            Bitmap image = new Bitmap(file);

            // Create a 2D-Array of Pixels, with each element corresponding to a pixel in the PNG. 
            // The Pixel class is defined in the PuzzleMap.cs file
            var pixel_map = new Pixel[image.Width, image.Height];
            if (image.Width > image.Height) 
            {
                board_width_px = board_max;
                pixel_size = board_width_px/image.Width;
                board_height_px = pixel_size*image.Height;
            }

            else 
            {
                board_height_px = board_max;
                pixel_size = board_height_px/image.Height;
                board_width_px = pixel_size*image.Width;
            }

            for (int x = 0; x < image.Width; x++) 
            {   
                for (int y = 0; y < image.Height; y++)
                {
                    System.Drawing.Color p_color = image.GetPixel(x, y);
                    pixel_map[x, y] = new Pixel();
                    pixel_map[x, y].Position = new Vector2(board_origin.X + x*pixel_size, board_origin.Y + y*pixel_size);

                    if (p_color.R == 255 && p_color.B == 255 && p_color.G == 255) 
                    {
                        pixel_map[x, y].State = PixelState.Off;
                    }
                        
                    else
                    {
                        pixel_map[x, y].State = PixelState.On;
                    }
                }
            }

            return new PuzzleMap(pixel_map);
        }
    }
}