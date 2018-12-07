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

using DragonOgg.MediaPlayer;
using PMDToolkit.Data;
using PMDToolkit.Graphics;

namespace PMDToolkit.Logic.Results
{
    public class BGM : IResult
    {
        //public ResultType Type { get { return ResultType.BGM; } }
        public RenderTime Delay => RenderTime.Zero;

        private readonly string newBGM;
        private readonly bool fade;

        public BGM(string newBGM, bool fade)
        {
            this.newBGM = Paths.MusicPath + newBGM;
            this.fade = fade;
        }

        public void Execute()
        {
            if (Display.Screen.Song != newBGM)
            {
                if (Display.Screen.NextSong != null || Display.Screen.Song == "")
                {
                    //do nothing, and watch it tick down
                }
                else if (!string.IsNullOrEmpty(AudioManager.BGM.CurrentFile) || !fade)
                {
                    //otherwise, set up the tick-down
                    Display.Screen.MusicFadeTime = Display.Screen.MUSIC_FADE_TOTAL;
                }
                Display.Screen.NextSong = newBGM;
            }
        }
    }
}