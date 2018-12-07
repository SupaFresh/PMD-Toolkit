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

using OpenTK;
using OpenTK.Graphics;
using PMDToolkit.Graphics;
using PMDToolkit.Maps;

namespace PMDToolkit.Logic.Display
{
    internal class OverlayMoveAnimation : ISpellSprite
    {
        #region Constructors

        public OverlayMoveAnimation(int animIndex, RenderTime animTime, int loops, byte transparency)
        {
            AnimationIndex = animIndex;
            FrameLength = animTime;
            TotalLoops = loops;
            Alpha = (byte)(255 - transparency);
        }

        #endregion Constructors

        #region Properties

        public int AnimationIndex
        {
            get;
            set;
        }

        public RenderTime FrameTime
        {
            get;
            set;
        }

        public RenderTime FrameLength
        {
            get;
            set;
        }

        public int Frame
        {
            get;
            set;
        }

        //total frames

        public int Loops
        {
            get;
            set;
        }

        public int TotalLoops
        {
            get;
            set;
        }

        public byte Alpha { get; set; }

        public MoveAnimationType AnimType => MoveAnimationType.Overlay;

        public Direction8 Direction => Direction8.None;

        public Loc2D StartLoc => new Loc2D();

        public Loc2D MapLoc => new Loc2D();
        public int MapHeight { get; set; }

        public RenderTime ActionTime { get; set; }
        public bool ActionDone { get; set; }

        #endregion Properties

        public virtual void Begin()
        {
        }

        public virtual void Process(RenderTime elapsedTime)
        {
            ActionTime += elapsedTime;
            FrameTime += elapsedTime;
            if (FrameTime >= FrameLength)
            {
                FrameTime = FrameTime - FrameLength;
                Frame++;
            }

            if (Frame >= TextureManager.GetSpellSheet(TextureManager.SpellAnimType.Spell, AnimationIndex).TotalFrames)
            {
                Loops++;
                Frame = 0;
            }

            if (Loops >= TotalLoops)
            {
                ActionDone = true;
            }
        }

        public virtual void Draw()
        {
            if (!ActionDone)
            {
                AnimSheet sheet = TextureManager.GetSpellSheet(TextureManager.SpellAnimType.Spell, AnimationIndex);
                TextureManager.TextureProgram.PushModelView();
                TextureManager.TextureProgram.SetTextureColor(new Color4(255, 255, 255, Alpha));
                for (int y = 0; y < TextureManager.SCREEN_HEIGHT; y += sheet.TileHeight)
                {
                    for (int x = 0; x < TextureManager.SCREEN_WIDTH; x += sheet.TileWidth)
                    {
                        TextureManager.TextureProgram.SetModelView(Matrix4.Identity);
                        TextureManager.TextureProgram.LeftMultModelView(Matrix4.CreateTranslation(x, y, 0));
                        TextureManager.TextureProgram.UpdateModelView();
                        sheet.RenderAnim(Frame, 0, 0);
                    }
                }
                TextureManager.TextureProgram.SetTextureColor(Color4.White);
                TextureManager.TextureProgram.PopModelView();
            }
        }

        public Loc2D GetStart()
        {
            return new Loc2D();
        }

        public Loc2D GetEnd()
        {
            return new Loc2D();
        }
    }
}