using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Input;

namespace Win2DEngine
{
    public enum PressState
    {
        Down,
        Up,
        Pressed,
        Released
    }

    public enum MouseButton
    {
        Left = 0,
        Right = 1,
        Middle = 2,
        X1 = 3,
        X2 = 4
    }

    public class InputState
    {
        public bool[] KeyStates { get; } = new bool[219];
        public bool[] MouseStates { get; } = new bool[5];
        public Vector2 MousePosition { get; set; }

        public InputState Clone()
        {
            var newState = new InputState() { MousePosition = MousePosition };
            Array.Copy(KeyStates, 0, newState.KeyStates, 0, KeyStates.Length);
            Array.Copy(MouseStates, 0, newState.MouseStates, 0, MouseStates.Length);
            return newState;
        }
    }
    public class InputManager
    {
        private InputState actualState = new InputState();
        public InputState CurrentState { get; private set; } = new InputState();
        public InputState PreviousState { get; private set; } = new InputState();

        public void KeyDown_GameLoopThread(VirtualKey virtualKey)
        {
            actualState.KeyStates[(int)virtualKey] = true;
        }

        public void KeyUp_GameLoopThread(VirtualKey virtualKey)
        {
            actualState.KeyStates[(int)virtualKey] = false;
        }

        public void PointerEvent_GameLoopThread(bool[] mouseStates, Vector2 position)
        {
            Array.Copy(mouseStates, 0, actualState.MouseStates, 0, actualState.MouseStates.Length);
            actualState.MousePosition = position;
        }

        public void Update()
        {
            CurrentState = actualState.Clone();
        }

        public void AfterUpdate()
        {
            PreviousState = CurrentState;
        }

        public bool IsKey(VirtualKey key, PressState pressState)
        {
            switch (pressState)
            {
                case PressState.Down:
                    return CurrentState.KeyStates[(int)key];
                case PressState.Up:
                    return !CurrentState.KeyStates[(int)key];
                case PressState.Pressed:
                    return CurrentState.KeyStates[(int)key] && !PreviousState.KeyStates[(int)key];
                case PressState.Released:
                    return !CurrentState.KeyStates[(int)key] && PreviousState.KeyStates[(int)key];
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public bool IsMouseButton(MouseButton mouseButton, PressState pressState)
        {
            switch (pressState)
            {
                case PressState.Down:
                    return CurrentState.MouseStates[(int)mouseButton];
                case PressState.Up:
                    return !CurrentState.MouseStates[(int)mouseButton];
                case PressState.Pressed:
                    return CurrentState.MouseStates[(int)mouseButton] && !PreviousState.MouseStates[(int)mouseButton];
                case PressState.Released:
                    return !CurrentState.MouseStates[(int)mouseButton] && PreviousState.MouseStates[(int)mouseButton];
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public Vector2 MousePosition => CurrentState.MousePosition;
        public Vector2 PreviousMousePosition => PreviousState.MousePosition;
    }
}
