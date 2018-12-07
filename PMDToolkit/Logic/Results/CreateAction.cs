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

using PMDToolkit.Graphics;
using PMDToolkit.Logic.Display;
using PMDToolkit.Logic.Gameplay;

namespace PMDToolkit.Logic.Results
{
    public class CreateAction : IResult
    {
        //an animation performed by a character as though in a spell
        //differs from hurt or defeat in that it can contain those animations, but is not limited to them
        //may require attack, throw, and jump to be removed (they are solely used in battle context)
        //public ResultType Type { get { return ResultType.CreateAction; } }

        public RenderTime Delay => InstantPass ? RenderTime.Zero : CharSprite.GetPassTime(charData, dir, Action);

        private FormData charData;
        private readonly Maps.Direction8 dir;
        public CharSprite.ActionType Action { get; set; }
        public bool Looping { get; set; }
        public bool InPlace { get; set; }
        public bool InstantPass { get; set; }

        public int CharIndex { get; }

        public CreateAction(int charIndex, ActiveChar character, CharSprite.ActionType action)
        {
            CharIndex = charIndex;
            Action = action;
            charData = character.CharData;
            dir = character.CharDir;
        }

        public CreateAction(int charIndex, ActiveChar character, CharSprite.ActionType action, bool looping, bool inPlace)
        {
            CharIndex = charIndex;
            Action = action;
            charData = character.CharData;
            dir = character.CharDir;
            Looping = looping;
            InPlace = inPlace;
        }

        public void Execute()
        {
            CharSprite sprite;
            if (CharIndex < 0)
            {
                sprite = Screen.Players[CharIndex + Processor.MAX_TEAM_SLOTS];
            }
            else
            {
                sprite = Screen.Npcs[CharIndex];
            }
            if (sprite.CurrentAction == Action)
            {
                sprite.PrevActionTime += sprite.ActionTime;
            }
            else
            {
                sprite.PrevActionTime = RenderTime.Zero;
            }

            sprite.CurrentAction = Action;
            sprite.ActionTime = RenderTime.Zero;
            sprite.ActionDone = false;
            sprite.ActionLoop = Looping;
            sprite.MoveInPlace = InPlace;
        }
    }
}