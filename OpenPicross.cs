using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Picross
{
    public class OpenPicross : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch sprite_batch;
        private SpriteBatch text_batch;
        PuzzleMap loaded_puzzle;
        bool left_down = false;
        bool right_down = false;
        float scaling_factor = 1.0f;

        public OpenPicross()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Logic.UpdateContent(Content);
            
            // Let the user resize their window if they want to, and also make their mouse cursor visable
            Window.AllowUserResizing = false;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Set the window to its default resolution
            graphics.PreferredBackBufferWidth = 1366;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            sprite_batch = new SpriteBatch(GraphicsDevice);
            text_batch = new SpriteBatch(GraphicsDevice);
            loaded_puzzle = PuzzleLoader.LoadPuzzleFromPNG("TestPuzzles/test_a.png");
        }

        protected override void Update(GameTime gameTime)
        {
            // Update the value of Logic.Content so we can use it elsewhere
            Logic.UpdateContent(Content);

            scaling_factor = (float)Window.ClientBounds.Height/PuzzleLoader.internal_height;

            // Check if the left or right mouse buttons have been clicked
            // A click has not actually occured until the button has been released
            var mouse_state = Mouse.GetState();
            var left_click = false;
            var right_click = false;

            if (left_down && mouse_state.LeftButton != ButtonState.Pressed) 
            {
                left_click = true;
            }

            left_down = (mouse_state.LeftButton == ButtonState.Pressed);

            if (right_down && mouse_state.RightButton != ButtonState.Pressed) 
            {
                right_click = true;
            }

            right_down = (mouse_state.RightButton == ButtonState.Pressed);

            // Check to see if any of the tiles have been clicked
            for (int x = 0; x < loaded_puzzle.PlayerMap.GetLength(0); x++)
            {   
                for (int y = 0; y < loaded_puzzle.PlayerMap.GetLength(1); y++) 
                {
                    var pixel = loaded_puzzle.PlayerMap[x, y];
                    var botright = new Vector2(pixel.Position.X + PuzzleLoader.pixel_size, pixel.Position.Y + PuzzleLoader.pixel_size);
                    var mouse_pos = new Vector2(mouse_state.Position.X/scaling_factor, mouse_state.Position.Y/scaling_factor);

                    if (Logic.IsMousePointing(pixel.Position, botright, mouse_pos))
                    {
                        if (left_click) 
                        {
                            pixel.ToggleOnOff();
                        }

                        else if (right_click) 
                        {
                            pixel.ToggleIgnored();
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            sprite_batch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Matrix.CreateScale(scaling_factor));
            text_batch.Begin(SpriteSortMode.Immediate, null, SamplerState.AnisotropicWrap, null, null, null, Matrix.CreateScale(scaling_factor));

            for (int x = 0; x < loaded_puzzle.PlayerMap.GetLength(0); x++)
            {   
                for (int y = 0; y < loaded_puzzle.PlayerMap.GetLength(1); y++) 
                {
                    var pixel = loaded_puzzle.PlayerMap[x, y];
                    sprite_batch.Draw(pixel.Sprite, pixel.Position, null, Color.White, 0f, Vector2.Zero,
                        (float)PuzzleLoader.pixel_size/pixel.Sprite.Height, SpriteEffects.None, 0f);
                }
            }

            var NumberMap = new Dictionary<int, Texture2D>() 
            {
                { 1, Content.Load<Texture2D>("Sprites/number1") },
                { 2, Content.Load<Texture2D>("Sprites/number2") },
                { 3, Content.Load<Texture2D>("Sprites/number3") },
                { 4, Content.Load<Texture2D>("Sprites/number4") },
                { 5, Content.Load<Texture2D>("Sprites/number5") },
                { 6, Content.Load<Texture2D>("Sprites/number6") },
                { 7, Content.Load<Texture2D>("Sprites/number7") },
                { 8, Content.Load<Texture2D>("Sprites/number8") },
                { 9, Content.Load<Texture2D>("Sprites/number9") },
                { 0, Content.Load<Texture2D>("Sprites/number0") },
            };

            var puzzle_guide = loaded_puzzle.PlayerMap.GetPuzzleGuide();
            var current_position = PuzzleLoader.colguide_origin;

            foreach (List<int> column in puzzle_guide.Columns)
            {
                var to_scale = (((float)PuzzleLoader.pixel_size/NumberMap[0].Height)/column.Count)/3;

                current_position = new Vector2
                (
                    current_position.X + PuzzleLoader.pixel_size/2 - NumberMap[0].Height*to_scale/2,
                    current_position.Y - NumberMap[0].Height*to_scale
                );
                        
                Console.WriteLine($"{current_position}, {to_scale}");

                foreach (int number in Enumerable.Reverse(column))
                {
                    if (number < 10) 
                    {
                        Console.WriteLine($"{current_position}, {to_scale}");
                        text_batch.Draw(NumberMap[0], current_position, null, Color.White, 
                            0f, Vector2.Zero, to_scale, SpriteEffects.None, 0f);
                    }

                    else 
                    {
                        
                    }
                }
            }
            
            sprite_batch.End();
            text_batch.End();

            base.Draw(gameTime);
        }
    }

    public static class Logic
    {
        public static ContentManager Content;

        // List of valid actions, these can have multiple keys assigned to them
        public enum Actions
        {

        }

        // Dictionary that determines which keys correspond to which actions
        public static Dictionary<Actions, List<Keys>> control_map = new Dictionary<Actions, List<Keys>>()
        {

        };

        // Check to see if a specific button is pressed
        public static bool IsButtonPressed(Actions action)
        {
            return control_map[action].Any(x => Keyboard.GetState().IsKeyDown(x));
        }

        public static bool IsMousePointing(Vector2 topleft, Vector2 botright, Vector2 mouse_pos) 
        {
            return topleft.X < mouse_pos.X && mouse_pos.X < botright.X && topleft.Y < mouse_pos.Y && mouse_pos.Y < botright.Y;
        }

        public static PuzzleGuide GetPuzzleGuide(this Pixel[,] pixel_map) 
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
   
        public static void UpdateContent(ContentManager content)
        {
            Content = content;
        }
    }
}