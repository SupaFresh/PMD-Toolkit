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

namespace PMDToolkit.Graphics
{
    public class AnimSheet : TileSheet
    {
        public int TotalFrames { get; private set; }
        public int TotalDirs { get; private set; }
        public int FramesPerDir { get; private set; }

        public override void GenerateDataBuffer(int dirs, int framesPerDir)
        {
            TotalDirs = dirs;
            FramesPerDir = framesPerDir;
            TotalFrames = ImageWidth / (ImageHeight / dirs / framesPerDir);
            base.GenerateDataBuffer(ImageHeight / dirs / framesPerDir, ImageHeight / dirs / framesPerDir);
        }

        public void RenderAnim(int frame, int dir, int frameOfDir)
        {
            if (frame >= TotalFrames || dir >= TotalDirs || frameOfDir >= FramesPerDir)
            {
                TextureManager.ErrorTexture.RenderAnim(0, 0, 0);
                return;
            }

            RenderTile(frame, dir * FramesPerDir + frameOfDir);
        }
    }
}