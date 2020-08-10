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
        RenderTarget2D OffScreenRenderTarget;
        Point OldWindowSize;
        private static float aspect_ratio = 16/9f;
        MouseState mouse_state;
        PuzzleMap loaded_puzzle;

        public OpenPicross()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Logic.UpdateContent(Content);
            
            // Let the user resize their window if they want to, and also make their mouse cursor visable
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            // This code is from https://stackoverflow.com/a/8396832
            // It is licensed under CC BY-SA 3.0: https://creativecommons.org/licenses/by-sa/3.0/
            Window.ClientSizeChanged -= new EventHandler<EventArgs>(Window_ClientSizeChanged);

            if (Window.ClientBounds.Width != OldWindowSize.X)
            {
                graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                graphics.PreferredBackBufferHeight = (int)(Window.ClientBounds.Width / aspect_ratio);

                graphics.ApplyChanges();
            }
            
            if (Window.ClientBounds.Height != OldWindowSize.Y)
            {
                graphics.PreferredBackBufferWidth = (int)(Window.ClientBounds.Height * aspect_ratio);
                graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;

                graphics.ApplyChanges();
            }

            OldWindowSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
            Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
        }

        protected override void Initialize()
        {
            // Set the window to its default resolution
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            sprite_batch = new SpriteBatch(GraphicsDevice);
            OldWindowSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
            OffScreenRenderTarget = new RenderTarget2D(GraphicsDevice, Window.ClientBounds.Width, Window.ClientBounds.Height);
            loaded_puzzle = PuzzleLoader.LoadPuzzleFromPNG("TestPuzzles/test5.png");
        }

        protected override void UnloadContent()
        {
            if (OffScreenRenderTarget != null)
                OffScreenRenderTarget.Dispose();

            if (sprite_batch != null)
                sprite_batch.Dispose();

            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            // Update the value of Logic.Content so we can use it elsewhere
            Logic.UpdateContent(Content);
            mouse_state = Mouse.GetState();

            base.Update(gameTime);
        }

        protected override bool BeginDraw()
        {
            GraphicsDevice.SetRenderTarget(OffScreenRenderTarget);
            return base.BeginDraw();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            sprite_batch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp);

            for (int x = 0; x < loaded_puzzle.SolutionMap.GetLength(0); x++)
            {   
                for (int y = 0; y < loaded_puzzle.SolutionMap.GetLength(1); y++) 
                {
                    var pixel = loaded_puzzle.SolutionMap[x, y];
                    sprite_batch.Draw(pixel.Sprite, pixel.Position, null, Color.White, 0f, Vector2.Zero,
                        (float)PuzzleLoader.pixel_size/pixel.Sprite.Height, SpriteEffects.None, 0f);
                }
            }
            
            sprite_batch.End();

            base.Draw(gameTime);
        }       
        
        protected override void EndDraw()
        {
            GraphicsDevice.SetRenderTarget(null);
            sprite_batch.Begin();
            sprite_batch.Draw(OffScreenRenderTarget, GraphicsDevice.Viewport.Bounds, Color.White);
            sprite_batch.End();
            base.EndDraw();
        }
    }

    public static class Logic
    {
        public static ContentManager Content;
        public static GraphicsDevice Graphics;

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

        public static void UpdateContent(ContentManager content)
        {
            Content = content;
        }
    }
}