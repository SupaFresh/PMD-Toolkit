﻿/*The MIT License (MIT)

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

        public bool this[InputType i]
        {
            get
            {
                return inputStates[(int)i];
            }
        }

        public int TotalInputs { get { return inputStates.Length; } }

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

        public Input(KeyboardDevice Key, MouseDevice mouse)
        {
            Loc2D dirLoc = new Loc2D();

            if (Key[Key.Down])
            {
                Operations.MoveInDirection8(ref dirLoc, Direction8.Down, 1);
            }
            if (Key[Key.Left])
            {
                Operations.MoveInDirection8(ref dirLoc, Direction8.Left, 1);
            }
            if (Key[Key.Up])
            {
                Operations.MoveInDirection8(ref dirLoc, Direction8.Up, 1);
            }
            if (Key[Key.Right])
            {
                Operations.MoveInDirection8(ref dirLoc, Direction8.Right, 1);
            }

            Direction = Operations.GetDirection8(new Loc2D(), dirLoc);

            inputStates[(int)InputType.X] = Key[Key.X];
            inputStates[(int)InputType.Z] = Key[Key.Z];
            inputStates[(int)InputType.C] = Key[Key.C];
            inputStates[(int)InputType.A] = Key[Key.A];
            inputStates[(int)InputType.S] = Key[Key.S];
            inputStates[(int)InputType.D] = Key[Key.D];
            inputStates[(int)InputType.Q] = Key[Key.Q];
            inputStates[(int)InputType.W] = Key[Key.W];

            inputStates[(int)InputType.Enter] = (Key[Key.Enter]);

            LeftMouse = mouse[MouseButton.Left];
            RightMouse = mouse[MouseButton.Right];

            MouseWheel = mouse.Wheel;

            MouseLoc = new Loc2D(mouse.X, mouse.Y);

            Shift = Key[Key.ShiftLeft] || Key[Key.ShiftRight];

            ShowDebug = Key[Key.F1];
            SpeedDown = Key[Key.F2];
            SpeedUp = Key[Key.F3];
#if GAME_MODE
            Intangible = keyboard[Key.F4];
            Print = keyboard[Key.F5];
            Restart = keyboard[Key.F12];
#endif
        }

        public static bool operator ==(Input input1, Input input2)
        {
            if (input1.Direction != input2.Direction) return false;

            for (int i = 0; i < 9; i++)
            {
                if (input1[(InputType)i] != input2[(InputType)i]) return false;
            }

            return true;
        }

        public static bool operator !=(Input input1, Input input2)
        {
            return !(input1 == input2);
        }
    }
}