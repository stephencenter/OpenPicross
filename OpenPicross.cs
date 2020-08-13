using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace Picross
{
    public class OpenPicross : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch sprite_batch;
        private SpriteBatch text_batch;

        // This is a list of all sprites in the game. They are loaded when the game launches
        public static Dictionary<string, Texture2D> SpriteMap;
        public static Dictionary<char, Texture2D> FontMap;
        
        public static List<string> PuzzleList;
        public static Dictionary<GameState, List<GameObject>> ObjectLayers;
        public static PuzzleMap LoadedPuzzle;
        
        // This is the game's state path, or in other words the path taken to get to the current state.
        // This lets us implement a Back button simply returning to the previous state in the list.
        // The final item in this list state_path.Last() is the current game state
        private static List<GameState> state_path;

        // This number is used to scale up/down all the elements of the game so they take up the same percentage
        // of the window regardless of resolution
        public static float scaling_factor = 1.0f;

        #region
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

            state_path = new List<GameState> { };

            SpriteMap = new Dictionary<string, Texture2D>() 
            {
                { "pixel_off", Content.Load<Texture2D>("Sprites/pixel_off") },
                { "pixel_on", Content.Load<Texture2D>("Sprites/pixel_on") },
                { "pixel_ignored", Content.Load<Texture2D>("Sprites/pixel_ignored") }
            };

            FontMap = new Dictionary<char, Texture2D>() 
            {
                { ' ', Content.Load<Texture2D>("Sprites/Text/text_space") },
                { '1', Content.Load<Texture2D>("Sprites/Text/text_1") },
                { '2', Content.Load<Texture2D>("Sprites/Text/text_2") },
                { '3', Content.Load<Texture2D>("Sprites/Text/text_3") },
                { '4', Content.Load<Texture2D>("Sprites/Text/text_4") },
                { '5', Content.Load<Texture2D>("Sprites/Text/text_5") },
                { '6', Content.Load<Texture2D>("Sprites/Text/text_6") },
                { '7', Content.Load<Texture2D>("Sprites/Text/text_7") },
                { '8', Content.Load<Texture2D>("Sprites/Text/text_8") },
                { '9', Content.Load<Texture2D>("Sprites/Text/text_9") },
                { '0', Content.Load<Texture2D>("Sprites/Text/text_0") },
            };

            PuzzleList = new List<string>() 
            {
                "TestPuzzles/test_a.png",
                "TestPuzzles/test_b.png",
                "TestPuzzles/test_c.png",
                "TestPuzzles/test_d.png",
                "TestPuzzles/test_e.png",
                "TestPuzzles/test_f.png",
                "TestPuzzles/test_g.png",
                "TestPuzzles/test1.png",
                "TestPuzzles/test2.png",
                "TestPuzzles/test3.png",
                "TestPuzzles/test4.png",
                "TestPuzzles/test5.png",
                "TestPuzzles/test6.png",
                "TestPuzzles/test7.png",
                "TestPuzzles/test8.png",
                "TestPuzzles/test9.png",
            };

            ObjectLayers = new Dictionary<GameState, List<GameObject>>()
            {
                { GameState.TitleScreen, new List<GameObject>() },
                { GameState.LevelSelect, new List<GameObject>() },
                { GameState.InGame, new List<GameObject>() },
                { GameState.Options, new List<GameObject>() },
                { GameState.Victory, new List<GameObject>() },
            };

            GameStateLoader.LoadLevelSelect(LoadDirection.Next);

            VerifyPuzzleList();
        }

        protected override void Update(GameTime gameTime)
        {
            scaling_factor = (float)Window.ClientBounds.Height/GameStateLoader.internal_height;
            InputManager.UpdateMouseState();

            if (state_path.Last() == GameState.TitleScreen)
            {
                GameStateTitleScreenUpdate();
            }

            if (state_path.Last() == GameState.LevelSelect)
            {
                GameStateLevelSelectUpdate();
            }
            
            if (state_path.Last() == GameState.InGame)
            {
                GameStateInGameUpdate();
            }

            if (state_path.Last() == GameState.Options)
            {
                GameStateOptionsUpdate();
            }

            if (state_path.Last() == GameState.Victory)
            {
                GameStateVictoryUpdate();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            sprite_batch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Matrix.CreateScale(scaling_factor));
            text_batch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null, null, Matrix.CreateScale(scaling_factor));

            if (state_path.Last() == GameState.TitleScreen)
            {
                GameStateTitleScreenDraw();
            }

            if (state_path.Last() == GameState.LevelSelect)
            {
                GameStateLevelSelectDraw();
            }
            
            if (state_path.Last() == GameState.InGame)
            {
                GameStateInGameDraw();
            }

            if (state_path.Last() == GameState.Options)
            {
                GameStateOptionsDraw();
            }

            if (state_path.Last() == GameState.Victory)
            {
                GameStateVictoryDraw();
            }
            
            sprite_batch.End();
            text_batch.End(); 

            base.Draw(gameTime);
        }
        #endregion
    
        /*************************
         *   GAMESTATE METHODS   *
         *************************/
        #region
        private void GameStateTitleScreenUpdate()
        {

        }       
             
        private void GameStateTitleScreenDraw()
        {

        }
                
        private void GameStateLevelSelectUpdate()
        {
            foreach (GameObject obj in ObjectLayers[GameState.LevelSelect])
            {
                if (obj is Interactable interactable && InputManager.IsMousePointing(interactable))
                {
                    interactable.OnCursorHover();
                }
            }
        }        
        
        private void GameStateLevelSelectDraw()
        {
            foreach (GameObject obj in ObjectLayers[GameState.LevelSelect])
            {
                sprite_batch.Draw(obj.Sprite, obj.Position, null, Color.White);

                if (obj is LevelSelector level)
                {
                    level.DrawText(text_batch, level.LevelNumber.ToString(), 0.05f, 0.05f);
                }
            }
        }
        
        private void GameStateInGameUpdate() 
        {
            // Check to see if any of the tiles have been clicked
            foreach (GameObject obj in ObjectLayers[GameState.InGame])
            {
                if (obj is Interactable interactable && InputManager.IsMousePointing(interactable))
                {
                    interactable.OnCursorHover();
                }
            }

            if (LoadedPuzzle.CheckForVictory())
            {
                Console.WriteLine("You win!");
                System.Environment.Exit(0);
            }
        }

        private void GameStateInGameDraw() 
        {
            var puzzle_guide = LoadedPuzzle.GetSolutionGuide();
            DrawGameBoard();
            DrawColumnGuide(puzzle_guide);
            DrawRowGuide(puzzle_guide);
        }        
        
        private void GameStateOptionsUpdate()
        {

        }        
        
        private void GameStateOptionsDraw()
        {

        }        
        
        private void GameStateVictoryUpdate()
        {

        }        
        
        private void GameStateVictoryDraw()
        {

        }

        public static void GameStateNext(GameState state)
        {
            state_path.Add(state);
        }

        public static void GameStateBack()
        {
            state_path.RemoveAt(state_path.Count - 1);

            if (!state_path.Any())
            {
                state_path = new List<GameState>() { GameState.TitleScreen };
            }
        }
        #endregion

        /*************************
         *     OTHER METHODS     *
         *************************/
        #region
        private void DrawGameBoard() 
        {
            for (int x = 0; x < LoadedPuzzle.PlayerMap.GetLength(0); x++)
            {   
                for (int y = 0; y < LoadedPuzzle.PlayerMap.GetLength(1); y++) 
                {
                    var pixel = LoadedPuzzle.PlayerMap[x, y];
                    sprite_batch.Draw(pixel.Sprite, pixel.Position, null, Color.White, 0f, Vector2.Zero,
                        (float)pixel.Height/pixel.Sprite.Height, SpriteEffects.None, 0f);
                }
            }
        }

        private void DrawColumnGuide(PuzzleGuide puzzle_guide) 
        {
            var current_position = GameStateLoader.board_origin;
            var pixel_size = LoadedPuzzle.PlayerMap[0, 0].Height;
            var text_height = FontMap['0'].Height;
            var text_width = FontMap['0'].Width;

            // This counter tells us index we're at in the column list
            int col_index = 0;
            foreach (List<int> column in puzzle_guide.Columns)
            {
                // col_scale is how much to shrink the column so that it will fit in the width of the tile
                var col_scale = ((float)pixel_size/text_height);

                // margin is how much of the vertical space above the board is being taken up by this column of the guide
                var margin = (column.Count*col_scale*text_height)/(GameStateLoader.board_origin.Y);
                
                // If the margin is too small for the guide, then we have to shrink the column even further to make it fit
                col_scale = margin > 1 ? col_scale/margin : col_scale;

                // We push the column over to the right a distance equal to half the width of one tile,
                // minus half the apparent width of a number sprite. This will center the column horizontally
                // relative to the tile
                current_position.X += (pixel_size/2 - text_width*col_scale/2);

                // We iterate through the column in reverse, since we're drawing them bottom to top
                float initial_x = current_position.X;
                foreach (int number in Enumerable.Reverse(column))
                {
                    var str = number.ToString();

                    // If the number has multiple digits, we'll have to shrink it even more
                    float num_scale = col_scale/str.Length;

                    // We push the column upwards a distance equal to its apparent height. This will align the bottom of 
                    // the column with the top of the board
                    current_position.Y -= text_height*num_scale;

                    // Draw each of the digits of in the number to the screen
                    foreach (char c in str) 
                    {   
                        text_batch.Draw(FontMap[c], current_position, null, Color.White, 0f, Vector2.Zero, num_scale, SpriteEffects.None, 0f);
                        
                        // Each digit needs to be to the right of the previous digit.
                        current_position.X += text_width*num_scale;
                    }

                    // We return to our inital X value for the next number in the column
                    current_position.X = initial_x;
                }

                // Return the drawing position to the board origin, and then calculate from there where to begin
                // the next column
                current_position.Y = GameStateLoader.board_origin.Y;
                current_position.X = GameStateLoader.board_origin.X + pixel_size*(col_index + 1);
                col_index++;
            }
        }
    
        private void DrawRowGuide(PuzzleGuide puzzle_guide) 
        {
            var current_position = GameStateLoader.board_origin;
            var pixel_size = LoadedPuzzle.PlayerMap[0, 0].Height;
            var text_height = FontMap['0'].Height;
            var text_width = FontMap['0'].Width;

            // This counter tells us index we're at in the row list
            int row_index = 0;
            foreach (List<int> row in puzzle_guide.Rows)
            {
                // row_scale is how much to shrink the row so that it will fit in the height of the tile
                var row_scale = ((float)pixel_size/text_height);

                // margin is how much of the horizontal space to the left of the board is being taken up by this row of the guide
                var margin = (row.Count*pixel_size)/(GameStateLoader.board_origin.X);

                // If margin > 1, then the row is to big and we need to shrink it
                margin = margin > 1 ? margin : 1;

                float initial_y = current_position.Y;
                foreach (int number in Enumerable.Reverse(row))
                {
                    var str = number.ToString();

                    // If the number has multiple digits, we'll have to shrink it even more
                    float num_scale = row_scale/str.Length;

                    // Move the next number to the left, with the distance scaled down a bit if there's no margin
                    current_position.X -= pixel_size/margin;

                    // Center multi-digit numbers vertically
                    if (str.Length > 1) 
                    {
                        current_position.Y += text_height*num_scale/2;
                        current_position.X += pixel_size/2;
                    }

                    // Draw each of the digits of in the number to the screen
                    float initial_x = current_position.X;
                    foreach (char c in Enumerable.Reverse(str))
                    {   
                        text_batch.Draw(FontMap[c], current_position, null, Color.White, 0f, Vector2.Zero, num_scale, SpriteEffects.None, 0f);
                        current_position.X -= text_width*num_scale;
                    }

                    // We return to our inital X and Y value for the next number in the row
                    current_position.X = initial_x;
                    current_position.Y = initial_y;
                }

                // Return the drawing position to the board origin, and then calculate from there where to begin
                // the next row
                current_position.X = GameStateLoader.board_origin.X;
                current_position.Y = GameStateLoader.board_origin.Y + pixel_size*(row_index + 1);
                row_index++;
            }
        }

        private void VerifyPuzzleList()
        {
            foreach (string path in PuzzleList)
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine($"Could not find puzzle with path '{path}'");
                }
            }
        }
        #endregion
    }

    public enum GameState
    {
        TitleScreen = 0,
        LevelSelect,
        InGame,
        Options,
        Victory
    }

    public abstract class GameObject 
    {
        public Vector2 Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Texture2D Sprite { get; set; }

        // margin_x and margin_y should be >= 0f and < 0.5f
        public void DrawText(SpriteBatch text_batch, string text, float margin_x, float margin_y)
        {
            var text_height = OpenPicross.FontMap['0'].Height;
            var text_width = OpenPicross.FontMap['0'].Width;
            var string_width = text.Length*text_width;

            // We check how much each axis would need to be scaled up/down by to fit snuggly inside the textbox
            var x_scale = ((float)Width/string_width);
            var y_scale = ((float)Height/text_height);

            // Next we factor in the margins, and then choose the smaller scaling
            var text_scale = Math.Min(x_scale, y_scale)*Math.Min((1 - 2*margin_x), (1 - 2*margin_y));

            // draw_position is the position the current character in the string is being drawn to
            var draw_position = new Vector2
            (
                Position.X + (Width/2f) - (text_scale*string_width/2f),
                Position.Y + (Height/2f) - (text_scale*text_height/2f)
            );

            draw_position.X = Math.Max(draw_position.X, margin_x*Width);
            draw_position.Y = Math.Max(draw_position.Y, margin_y*Height);

            // Now we draw the chracters to the screen
            foreach (char c in text)
            {      
                text_batch.Draw(OpenPicross.FontMap[c], draw_position, null, Color.White, 0f, Vector2.Zero, text_scale, SpriteEffects.None, 0f);
                draw_position.X += text_width*text_scale;
            }
        }

        protected GameObject(Vector2 pos, int width, int height, Texture2D sprite)
        {
            Position = pos;
            Width = width;
            Height = height;
            Sprite = sprite;
        }
    }

    public abstract class Interactable : GameObject
    {   
        public abstract void OnCursorHover();

        public Interactable(Vector2 pos, int width, int height, Texture2D sprite) : base(pos, width, height, sprite) { }
    }
} 