/*The MIT License (MIT)

Copyright (c) 2014 PMU Staff

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using OpenTK.Input;
using PMDToolkit.Maps;

namespace PMDToolkit.Logic.Gameplay
{
    public class Input
    {
        public enum InputType
        {
            Z,
            X,
            C,
            A,
            S,
            D,
            Q,
            W,
            Enter
        }

        private bool[] inputStates = new bool[9];

        public bool this[InputType i] => inputStates[(int)i];

        public int TotalInputs => inputStates.Length;

        public Direction8 Direction { get; } = Direction8.None;

        public bool LeftMouse { get; set; }
        public bool RightMouse { get; set; }
        public Loc2D MouseLoc { get; set; }
        public int MouseWheel { get; set; }

        public bool Shift { get; set; }

        public bool ShowDebug { get; set; }
        public bool SpeedDown { get; set; }
        public bool SpeedUp { get; set; }
        public bool Intangible { get; set; }
        public bool Print { get; set; }
        public bool Restart { get; set; }

        public Input()
        {
            Direction = Direction8.None;
        }

        public Input(KeyboardDevice Keyboard, MouseDevice Mouse)
        {
            Loc2D dirLoc = new Loc2D();

            if (Keyboard[Key.Down])
            {
                Operations.MoveInDirection8(ref dirLoc, Direction8.Down, 1);
            }
            if (Keyboard[Key.Left])
            {
                Operations.MoveInDirection8(ref dirLoc, Direction8.Left, 1);
            }
            if (Keyboard[Key.Up])
            {
                Operations.MoveInDirection8(ref dirLoc, Direction8.Up, 1);
            }
            if (Keyboard[Key.Right])
            {
                Operations.MoveInDirection8(ref dirLoc, Direction8.Right, 1);
            }

            Direction = Operations.GetDirection8(new Loc2D(), dirLoc);

            inputStates[(int)InputType.X] = Keyboard[Key.X];
            inputStates[(int)InputType.Z] = Keyboard[Key.Z];
            inputStates[(int)InputType.C] = Keyboard[Key.C];
            inputStates[(int)InputType.A] = Keyboard[Key.A];
            inputStates[(int)InputType.S] = Keyboard[Key.S];
            inputStates[(int)InputType.D] = Keyboard[Key.D];
            inputStates[(int)InputType.Q] = Keyboard[Key.Q];
            inputStates[(int)InputType.W] = Keyboard[Key.W];

            inputStates[(int)InputType.Enter] = (Keyboard[Key.Enter]);

            LeftMouse = Mouse[MouseButton.Left];
            RightMouse = Mouse[MouseButton.Right];

            MouseWheel = Mouse.Wheel;

            MouseLoc = new Loc2D(Mouse.X, Mouse.Y);

            Shift = Keyboard[Key.ShiftLeft] || Keyboard[Key.ShiftRight];

            ShowDebug = Keyboard[Key.F1];
            SpeedDown = Keyboard[Key.F2];
            SpeedUp = Keyboard[Key.F3];
#if GAME_MODE
            Intangible = keyboard[Key.F4];
            Print = keyboard[Key.F5];
            Restart = keyboard[Key.F12];
#endif
        }

        public static bool operator ==(Input input1, Input input2)
        {
            if (input1.Direction != input2.Direction)
            {
                return false;
            }

            for (int i = 0; i < 9; i++)
            {
                if (input1[(InputType)i] != input2[(InputType)i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator !=(Input input1, Input input2)
        {
            return !(input1 == input2);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}