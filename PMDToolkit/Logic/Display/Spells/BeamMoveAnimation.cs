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
using PMDToolkit.Graphics;
using PMDToolkit.Maps;
using System;

namespace PMDToolkit.Logic.Display
{
    internal class BeamMoveAnimation : ISpellSprite
    {
        #region Constructors

        public BeamMoveAnimation(Loc2D startLoc, int animIndex, RenderTime animTime, Direction8 dir, int distance, RenderTime lastingTime)
        {
            StartLoc = startLoc;
            AnimationIndex = animIndex;
            FrameLength = animTime;
            Direction = dir;
            TotalDistance = distance;
            LastingTime = lastingTime;
        }

        #endregion Constructors

        #region Properties

        public RenderTime TotalTime => FrameLength * TextureManager.GetSpellSheet(TextureManager.SpellAnimType.Beam, AnimationIndex).TotalFrames * TotalDistance;

        public Loc2D StartLoc
        {
            get;
            set;
        }

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

        public RenderTime LastingTime
        {
            get;
            set;
        }

        public MoveAnimationType AnimType => MoveAnimationType.Beam;

        public int Distance
        {
            get;
            set;
        }

        public int TotalDistance
        {
            get;
            set;
        }

        public Maps.Direction8 Direction
        {
            get;
            set;
        }

        public RenderTime TimeSinceArrival { get; set; }

        public Loc2D MapLoc => StartLoc * TextureManager.TILE_SIZE;
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
            if (Distance >= TotalDistance)
            {
                TimeSinceArrival += elapsedTime;
            }
            if (FrameTime >= FrameLength)
            {
                FrameTime = FrameTime - FrameLength;
                Frame++;
            }

            if (Frame >= TextureManager.GetSpellSheet(TextureManager.SpellAnimType.Beam, AnimationIndex).TotalFrames)
            {
                Frame = 0;
                if (Distance < TotalDistance)
                {
                    Distance++;
                }
            }

            if (TimeSinceArrival >= LastingTime)
            {
                ActionDone = true;
            }
        }

        public virtual void Draw()
        {
            if (!ActionDone)
            {
                TextureManager.TextureProgram.PushModelView();
                Loc2D drawLoc = new Loc2D(StartLoc.X * TextureManager.TILE_SIZE + TextureManager.TILE_SIZE / 2 - Graphics.TextureManager.GetSpellSheet(TextureManager.SpellAnimType.Beam, AnimationIndex).TileWidth / 2,
                    StartLoc.Y * TextureManager.TILE_SIZE + TextureManager.TILE_SIZE / 2 - TextureManager.GetSpellSheet(TextureManager.SpellAnimType.Beam, AnimationIndex).TileHeight / 2 - MapHeight);

                TextureManager.TextureProgram.LeftMultModelView(Matrix4.CreateTranslation(drawLoc.X, drawLoc.Y, 0));
                TextureManager.TextureProgram.UpdateModelView();

                AnimSheet sheet = TextureManager.GetSpellSheet(TextureManager.SpellAnimType.Beam, AnimationIndex);
                Loc2D diff = new Loc2D();
                Operations.MoveInDirection8(ref diff, Direction, TextureManager.TILE_SIZE);
                for (int i = 0; i <= Distance; i++)
                {
                    if (i == 0)
                    {
                        sheet.RenderAnim(Frame, (int)Direction, 0);
                    }
                    else if (i == Distance)
                    {
                        if (Distance >= TotalDistance)
                        {
                            sheet.RenderAnim(Frame, (int)Direction, 3);
                        }
                        else
                        {
                            sheet.RenderAnim(Frame, (int)Direction, 2);
                        }
                    }
                    else
                    {
                        sheet.RenderAnim(Frame, (int)Direction, 1);
                    }
                    TextureManager.TextureProgram.LeftMultModelView(Matrix4.CreateTranslation(diff.X, diff.Y, 0));
                    TextureManager.TextureProgram.UpdateModelView();
                }

                TextureManager.TextureProgram.PopModelView();
            }
        }

        public Loc2D GetStart()
        {
            Loc2D endLoc = StartLoc;
            Operations.MoveInDirection8(ref endLoc, Direction, Distance);
            Loc2D topLeftLoc = new Loc2D(Math.Min(StartLoc.X, endLoc.X), Math.Min(StartLoc.Y, endLoc.Y));

            return new Loc2D(topLeftLoc.X * TextureManager.TILE_SIZE + TextureManager.TILE_SIZE / 2 - TextureManager.GetSpellSheet(TextureManager.SpellAnimType.Beam, AnimationIndex).TileWidth / 2,
                    topLeftLoc.Y * TextureManager.TILE_SIZE + TextureManager.TILE_SIZE / 2 - TextureManager.GetSpellSheet(TextureManager.SpellAnimType.Beam, AnimationIndex).TileHeight / 2 - MapHeight);
        }

        public Loc2D GetEnd()
        {
            Loc2D endLoc = StartLoc;
            Operations.MoveInDirection8(ref endLoc, Direction, Distance);
            Loc2D bottomRightLoc = new Loc2D(Math.Max(StartLoc.X, endLoc.X), Math.Max(StartLoc.Y, endLoc.Y));

            return new Loc2D(bottomRightLoc.X * TextureManager.TILE_SIZE + TextureManager.TILE_SIZE / 2 + TextureManager.GetSpellSheet(TextureManager.SpellAnimType.Beam, AnimationIndex).TileWidth / 2,
                    bottomRightLoc.Y * TextureManager.TILE_SIZE + TextureManager.TILE_SIZE / 2 + TextureManager.GetSpellSheet(TextureManager.SpellAnimType.Beam, AnimationIndex).TileHeight / 2 - MapHeight);
        }
    }
}