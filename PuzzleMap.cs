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

        public bool CheckForVictory() 
        {
            // We calculate the puzzle guide that would have been generated
            // if the player's current map was in the game. If their guide matches
            // the intended solution guide, then that means it's a valid solution.
            // Since there isn't a 1-to-1 correspondence between puzzle guides and
            // pixel maps, that means it's possible for the player to finish some
            // puzzles without using the intended solution!
            var solution_guide = SolutionMap.GetPuzzleGuide();
            var player_guide = PlayerMap.GetPuzzleGuide();

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
                    PlayerMap[x, y] = new Pixel();
                    PlayerMap[x, y].Position = SolutionMap[x, y].Position;
                }
            }
        }
    }


    public class Pixel 
    {
        public PixelState State { get; set; }
        public Texture2D Sprite { get; set; }
        public Vector2 Position { get; set; }

        public void ToggleOnOff() 
        {
            // Pixels with the "Ignored" state cannot be toggled on or off, they must be unignored first
            if (State == PixelState.Off) 
            {
                Sprite = Logic.Content.Load<Texture2D>("Sprites/pixel_on");
                State = PixelState.On;
            }

            else if (State == PixelState.On) 
            {
                Sprite = Logic.Content.Load<Texture2D>("Sprites/pixel_off");
                State = PixelState.Off;
            }
        }

        public void ToggleIgnored() 
        {
            // Pixels with the "On" state cannot be ignored, they must be toggled off first
            if (State == PixelState.Off) 
            {
                Sprite = Logic.Content.Load<Texture2D>("Sprites/pixel_ignored");
                State = PixelState.Ignored;
            }

            else if (State == PixelState.Ignored) 
            {
                Sprite = Logic.Content.Load<Texture2D>("Sprites/pixel_off");
                State = PixelState.Off;
            }
        }

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