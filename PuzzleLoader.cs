using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace Picross 
{
    public static class GameStateLoader 
    {
        public static readonly Vector2 board_origin = new Vector2(480, 125);
        public const int internal_width = 1920;
        public const int internal_height = 1080;
        public static int board_width_px;
        public static int board_height_px;

        public static PuzzleMap LoadPuzzleFromPNG(string file) 
        {   
            Bitmap image = new Bitmap(file);
            int pixel_size;
            int board_max = (int)((internal_height - board_origin.Y)*0.99f);

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
                    pixel_map[x, y] = new Pixel(new Vector2(board_origin.X + x*pixel_size, board_origin.Y + y*pixel_size), pixel_size);

                    if (p_color.R == 255 && p_color.B == 255 && p_color.G == 255) 
                    {
                        pixel_map[x, y].PixelState = PixelState.Off;
                    }
                        
                    else
                    {
                        pixel_map[x, y].PixelState = PixelState.On;
                    }
                }
            }

            return new PuzzleMap(pixel_map);
        }

        public static void LoadLevelSelect(LoadDirection load_direction)
        {   
            OpenPicross.ObjectLayers[GameState.LevelSelect].Clear();

            if (load_direction == LoadDirection.Next)
            {
                OpenPicross.GameStateNext(GameState.LevelSelect);
            }

            else
            {
                OpenPicross.GameStateBack();
            }

            int counter = 1;
            foreach (string level in OpenPicross.PuzzleList)
            {
                var selector = new LevelSelector(Vector2.Zero, 256, 256, OpenPicross.SpriteMap["pixel_off"], level, counter);
                OpenPicross.ObjectLayers[GameState.LevelSelect].Add(selector);  
                counter++;              
            }
        }

        public static void LoadInGame(LoadDirection load_direction, string level_string)
        {   
            OpenPicross.ObjectLayers[GameState.InGame].Clear();

            if (load_direction == LoadDirection.Next)
            {
                OpenPicross.GameStateNext(GameState.InGame);
            }

            else
            {
                OpenPicross.GameStateBack();
            }

            var loaded_puzzle = GameStateLoader.LoadPuzzleFromPNG(level_string);

            for (int x = 0; x < loaded_puzzle.PlayerMap.GetLength(0); x++)
            {   
                for (int y = 0; y < loaded_puzzle.PlayerMap.GetLength(1); y++) 
                {
                    OpenPicross.ObjectLayers[GameState.InGame].Add(loaded_puzzle.PlayerMap[x, y]);
                }
            }

            OpenPicross.LoadedPuzzle = loaded_puzzle;
        }
    }

    public class LevelSelector : Interactable
    {
        public string LevelString { get; set; }
        public int LevelNumber { get; set; }

        public override void OnCursorHover()
        {
            if (InputManager.LeftButtonCurrentState == MouseState.Clicked) 
            {
                GameStateLoader.LoadInGame(LoadDirection.Next, LevelString);
            }
        }

        public LevelSelector(Vector2 pos, int width, int height, Texture2D sprite, string lvl, int num) : base(pos, width, height, sprite)
        {
            LevelString = lvl;
            LevelNumber = num;
        }
    }

    public enum LoadDirection
    {
        Next = 0,
        Back
    }
}