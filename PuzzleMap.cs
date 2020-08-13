using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
using System;

namespace Picross
{
    public class PuzzleMap 
    {
        private Pixel[,] SolutionMap  { get; }
        public Pixel[,] PlayerMap { get; set; }

        private static PuzzleGuide get_puzzle_guide(Pixel[,] pixel_map) 
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
                    if (pixel_map[x, y].PixelState == PixelState.On) 
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
                    if (pixel_map[x, y].PixelState == PixelState.On) 
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

        public PuzzleGuide GetSolutionGuide() 
        {
            return get_puzzle_guide(SolutionMap);
        }

        public bool CheckForVictory() 
        {
            // We calculate the puzzle guide that would have been generated
            // if the player's current map was in the game. If their guide matches
            // the intended solution guide, then that means it's a valid solution.
            // Since there isn't a 1-to-1 correspondence between puzzle guides and
            // pixel maps, that means it's possible for the player to finish some
            // puzzles without using the intended solution!
            var solution_guide = GetSolutionGuide();
            var player_guide = get_puzzle_guide(PlayerMap);

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

        public PuzzleMap(Pixel[,] pixel_map)
        {
            SolutionMap = pixel_map;
            PlayerMap = new Pixel[pixel_map.GetLength(0), pixel_map.GetLength(1)];

            for (int x = 0; x < PlayerMap.GetLength(0); x++)
            {   
                for (int y = 0; y < PlayerMap.GetLength(1); y++) 
                {
                    PlayerMap[x, y] = new Pixel(SolutionMap[x, y].Position, SolutionMap[x, y].Height);
                }
            }
        }
    }


    public class Pixel : GameObject
    {
        public PixelState PixelState { get; set; }

        public void ToggleOnOff() 
        {
            // Pixels with the "Ignored" state cannot be toggled on or off, they must be unignored first
            if (PixelState == PixelState.Off) 
            {
                Sprite = OpenPicross.SpriteMap["pixel_on"];
                PixelState = PixelState.On;
            }

            else if (PixelState == PixelState.On) 
            {
                Sprite = OpenPicross.SpriteMap["pixel_off"];
                PixelState = PixelState.Off;
            }
        }

        public void ToggleIgnored() 
        {
            // Pixels with the "On" state cannot be ignored, they must be toggled off first
            if (PixelState == PixelState.Off) 
            {
                Sprite = OpenPicross.SpriteMap["pixel_ignored"];
                PixelState = PixelState.Ignored;
            }

            else if (PixelState == PixelState.Ignored) 
            {
                Sprite = OpenPicross.SpriteMap["pixel_off"];
                PixelState = PixelState.Off;
            }
        }

        public Pixel(Vector2 pos, int size) : base(pos, size, size, OpenPicross.SpriteMap["pixel_off"]) { }
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