using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Picross 
{
    public static class InputManager
    {
        public static MouseState LeftButtonCurrentState;
        public static MouseState RightButtonCurrentState;

        private static Microsoft.Xna.Framework.Input.MouseState mouse_state;

        // Dictionary that determines which keys correspond to which actions
        public static Dictionary<Actions, List<Keys>> keyboard_map = new Dictionary<Actions, List<Keys>>()
        {
            { Actions.Special, new List<Keys>() { Keys.Space  } }
        };        

        // Check to see if a specific key is pressed
        public static bool IsButtonPressed(Actions action)
        {
            return keyboard_map[action].Any(x => Keyboard.GetState().IsKeyDown(x));
        }

        public static bool IsMousePointing(Vector2 topleft, Vector2 botright) 
        {
            var cursor = GetCursorPosition();
            return topleft.X < cursor.X && cursor.X < botright.X && topleft.Y < cursor.Y && cursor.Y < botright.Y;
        }

        private static Vector2 GetCursorPosition() 
        {
            return new Vector2
            (
                mouse_state.Position.X/OpenPicross.scaling_factor,
                mouse_state.Position.Y/OpenPicross.scaling_factor
            );
        }

        // UpdateMouseState should ONLY be called ONCE per frame!
        public static void UpdateMouseState() 
        {
            mouse_state = Mouse.GetState();

            // Update the state of the Left Mouse Button
            if (mouse_state.LeftButton == ButtonState.Pressed) 
            {
                LeftButtonCurrentState = MouseState.Held;
            }

            else if (LeftButtonCurrentState == MouseState.Held) 
            {
                LeftButtonCurrentState = MouseState.Clicked;
            }

            else if (LeftButtonCurrentState == MouseState.Clicked)
            {
                LeftButtonCurrentState = MouseState.Released;
            }

            // Update the state of the Right Mouse Button
            if (mouse_state.RightButton == ButtonState.Pressed) 
            {
                RightButtonCurrentState = MouseState.Held;
            }

            else if (RightButtonCurrentState == MouseState.Held) 
            {
                RightButtonCurrentState = MouseState.Clicked;
            }

            else if (RightButtonCurrentState == MouseState.Clicked)
            {
                RightButtonCurrentState = MouseState.Released;
            }
        }
    }

    public enum MouseState 
    {
        // Released means that the mouse button was not in the 'Held' state the previous frame and is not being pressed
        Released = 0,

        // Held means that the mouse button is currently being pressed down
        Held, 

        // Clicked means that the mouse button was in the 'Held' state the previous frame, but was released on the current frame
        Clicked
    };


    // List of valid actions, these can inputs assigned to them via InputManager.keyboard_map
    public enum Actions
    {
        Special
    }
}