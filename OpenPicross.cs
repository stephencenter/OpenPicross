using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
        private PuzzleMap loaded_puzzle;

        // This is a list of all sprites in the game. They are loaded when the game launches
        public static Dictionary<string, Texture2D> SpriteMap;
        public static float scaling_factor = 1.0f;

        public OpenPicross()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

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

            SpriteMap = new Dictionary<string, Texture2D>() 
            {
                { "1", Content.Load<Texture2D>("Sprites/number1") },
                { "2", Content.Load<Texture2D>("Sprites/number2") },
                { "3", Content.Load<Texture2D>("Sprites/number3") },
                { "4", Content.Load<Texture2D>("Sprites/number4") },
                { "5", Content.Load<Texture2D>("Sprites/number5") },
                { "6", Content.Load<Texture2D>("Sprites/number6") },
                { "7", Content.Load<Texture2D>("Sprites/number7") },
                { "8", Content.Load<Texture2D>("Sprites/number8") },
                { "9", Content.Load<Texture2D>("Sprites/number9") },
                { "0", Content.Load<Texture2D>("Sprites/number0") },
                { "pixel_off", Content.Load<Texture2D>("Sprites/pixel_off") },
                { "pixel_on", Content.Load<Texture2D>("Sprites/pixel_on") },
                { "pixel_ignored", Content.Load<Texture2D>("Sprites/pixel_ignored") }
            };

            loaded_puzzle = PuzzleLoader.LoadPuzzleFromPNG("TestPuzzles/test9.png");
        }

        protected override void Update(GameTime gameTime)
        {
            scaling_factor = (float)Window.ClientBounds.Height/PuzzleLoader.internal_height;

            InputManager.UpdateMouseState();

            // Check to see if any of the tiles have been clicked
            for (int x = 0; x < loaded_puzzle.PlayerMap.GetLength(0); x++)
            {   
                for (int y = 0; y < loaded_puzzle.PlayerMap.GetLength(1); y++) 
                {
                    var pixel = loaded_puzzle.PlayerMap[x, y];
                    var botright = new Vector2(pixel.Position.X + PuzzleLoader.tile_size, pixel.Position.Y + PuzzleLoader.tile_size);

                    if (InputManager.IsMousePointing(pixel.Position, botright))
                    {
                        if (InputManager.LeftButtonCurrentState == MouseState.Clicked) 
                        {
                            pixel.ToggleOnOff();
                        }

                        else if (InputManager.RightButtonCurrentState == MouseState.Clicked)  
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
                        (float)PuzzleLoader.tile_size/pixel.Sprite.Height, SpriteEffects.None, 0f);
                }
            }

            var puzzle_guide = loaded_puzzle.GetSolutionGuide();
            var current_position = PuzzleLoader.colguide_origin;

            // This counter tells us index we're at in the column list
            int col_index = 0;
            foreach (List<int> column in puzzle_guide.Columns)
            {
                // This is the height in pixels of the number "Zero" in our sprite map
                // We will assume that all numbers in our font have ths same height, and are square.
                // This will make it easier for us to do some positioning math
                var num_height = SpriteMap["0"].Height;

                // col_scale is how much to shrink the column so that it will fit in the width of the tile
                var col_scale = ((float)PuzzleLoader.tile_size/num_height);

                // margin is how much of the vertical space above the board is being taken up by this column of the guide
                var margin = (column.Count*col_scale*num_height)/(PuzzleLoader.colguide_origin.Y);
                
                // If the margin is too small for the guide, then we have to shrink the column even further to make it fit
                col_scale = margin > 1 ? col_scale/margin : col_scale;

                // We push the column over to the right a distance equal to half the width of one tile,
                // minus half the apparent width of a number sprite. This will center the column horizontally
                // relative to the tile
                current_position.X += (PuzzleLoader.tile_size/2 - num_height*col_scale/2);

                // We push the column upwards a distance equal to its apparent height. This will align the bottom of 
                // the column with the top of the board
                current_position.Y -= num_height*col_scale;
                        
                Console.WriteLine($"{current_position}, {col_scale}, {margin}");

                float initial_x = current_position.X;
                foreach (int number in Enumerable.Reverse(column))
                {
                    var str = number.ToString();

                    // If the number has multiple digits, we'll have to shrink it even more
                    float num_scale = col_scale/str.Length;

                    foreach (char c in str) 
                    {
                        text_batch.Draw(SpriteMap[c.ToString()], current_position, null, Color.White, 0f, Vector2.Zero, num_scale, SpriteEffects.None, 0f);
                        current_position.X += num_height*num_scale*0.8f;
                    }

                    current_position.Y -= num_height*col_scale;
                    current_position.X = initial_x;
                }

                current_position.Y = PuzzleLoader.colguide_origin.Y;
                current_position.X = PuzzleLoader.colguide_origin.X + PuzzleLoader.tile_size*(col_index + 1);
                col_index++;
            }
            
            sprite_batch.End();
            text_batch.End(); 

            base.Draw(gameTime);
        }
    }
}