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

using System;
using System.Collections.Generic;
using System.Drawing;

using System.IO;

namespace PMDToolkit.Graphics
{
    public enum FrameType
    {
        Idle = 0,
        Walk,
        Attack,
        AttackArm,
        AltAttack,
        SpAttack,
        SpAttackCharge,
        SpAttackShoot,
        Hurt,
        Sleep
    }

    public class FrameData
    {
        #region Properties

        public int FrameWidth { get; private set; }

        public int FrameHeight { get; private set; }

        private Dictionary<FrameType, Dictionary<Maps.Direction8, int>> frameCount;

        #endregion Properties

        public FrameData()
        {
            frameCount = new Dictionary<FrameType, Dictionary<Maps.Direction8, int>>();
        }

        #region Methods

        public void SetFrameSize(int animWidth, int animHeight, int frames)
        {
            FrameWidth = animWidth / frames;

            FrameHeight = animHeight;
        }

        public void SetFrameCount(FrameType type, Maps.Direction8 dir, int count)
        {
            if (frameCount.ContainsKey(type) == false)
            {
                frameCount.Add(type, new Dictionary<Maps.Direction8, int>());
            }
            if (frameCount[type].ContainsKey(dir) == false)
            {
                frameCount[type].Add(dir, count);
            }
            else
            {
                frameCount[type][dir] = count;
            }
        }

        public int GetFrameCount(FrameType type, Maps.Direction8 dir)
        {
            if (frameCount.TryGetValue(type, out Dictionary<Maps.Direction8, int> dirs))
            {
                if (dirs.TryGetValue(dir, out int value))
                {
                    return value;
                }
            }

            return 0;
        }

        #endregion Methods
    }

    public class SpriteSheet : IDisposable
    {
        #region Constructors

        public SpriteSheet()
        {
            FrameData = new FrameData();
            animations = new Dictionary<FrameType, Dictionary<Maps.Direction8, TileSheet>>();
        }

        #endregion Constructors

        #region Properties

        public int BytesUsed { get; private set; }

        public FrameData FrameData { get; }

        private Dictionary<FrameType, Dictionary<Maps.Direction8, TileSheet>> animations;

        #endregion Properties

        #region Methods

        public Rectangle GetFrameBounds(FrameType frameType, Maps.Direction8 direction, int frameNum)
        {
            Rectangle rec = new Rectangle
            {
                X = frameNum * FrameData.FrameWidth,
                Y = 0,
                Width = FrameData.FrameWidth,
                Height = FrameData.FrameHeight
            };

            return rec;
        }

        public void LoadFromData(BinaryReader reader, int totalByteSize)
        {
            foreach (FrameType frameType in Enum.GetValues(typeof(FrameType)))
            {
                if (IsFrameTypeDirectionless(frameType) == false)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Maps.Direction8 dir = (Maps.Direction8)i;
                        int frameCount = reader.ReadInt32();
                        FrameData.SetFrameCount(frameType, dir, frameCount);
                        int size = reader.ReadInt32();
                        if (size > 0)
                        {
                            byte[] imgData = reader.ReadBytes(size);
                            TileSheet sheetSurface = new TileSheet();
                            using (MemoryStream stream = new MemoryStream(imgData))
                            {
                                try
                                {
                                    sheetSurface.LoadPixelsFromBytes(stream);

                                    sheetSurface.LoadTextureFromPixels32();
                                    sheetSurface.GenerateDataBuffer(sheetSurface.ImageWidth / frameCount, sheetSurface.ImageHeight);
                                }
                                catch (Exception ex)
                                {
                                    sheetSurface.Dispose(true);
                                    throw new Exception("Error reading image data for " + frameType.ToString() + " " + dir.ToString() + "\n", ex);
                                }
                            }
                            AddSheet(frameType, dir, sheetSurface);

                            FrameData.SetFrameSize(sheetSurface.ImageWidth, sheetSurface.ImageHeight, frameCount);
                        }
                    }
                }
                else
                {
                    int frameCount = reader.ReadInt32();
                    FrameData.SetFrameCount(frameType, Maps.Direction8.Down, frameCount);
                    int size = reader.ReadInt32();
                    if (size > 0)
                    {
                        byte[] imgData = reader.ReadBytes(size);
                        TileSheet sheetSurface = new TileSheet();
                        using (MemoryStream stream = new MemoryStream(imgData))
                        {
                            try
                            {
                                sheetSurface.LoadPixelsFromBytes(stream);
                                sheetSurface.LoadTextureFromPixels32();
                            }
                            catch (Exception ex)
                            {
                                sheetSurface.Dispose(true);
                                throw new Exception("Error reading image data for " + frameType.ToString() + "\n", ex);
                            }
                        }
                        sheetSurface.GenerateDataBuffer(sheetSurface.ImageWidth / frameCount, sheetSurface.ImageHeight);
                        AddSheet(frameType, Maps.Direction8.Down, sheetSurface);

                        FrameData.SetFrameSize(sheetSurface.ImageWidth, sheetSurface.ImageHeight, frameCount);
                    }
                }
            }

            BytesUsed = totalByteSize;
        }

        public TileSheet GetSheet(FrameType type, Maps.Direction8 dir)
        {
            if (IsFrameTypeDirectionless(type))
            {
                dir = Maps.Direction8.Down;
            }
            if (animations.ContainsKey(type))
            {
                if (animations[type].ContainsKey(dir))
                {
                    return animations[type][dir];
                }
            }

            return TextureManager.ErrorTexture;
        }

        public void AddSheet(FrameType type, Maps.Direction8 dir, TileSheet surface)
        {
            if (!animations.ContainsKey(type))
            {
                animations.Add(type, new Dictionary<Maps.Direction8, TileSheet>());
            }
            if (animations[type].ContainsKey(dir) == false)
            {
                animations[type].Add(dir, surface);
            }
            else
            {
                animations[type][dir] = surface;
            }
        }

        public static bool IsFrameTypeDirectionless(FrameType frameType)
        {
            switch (frameType)
            {
                case FrameType.Sleep:
                    return true;
            }
            return false;
        }

        public void Dispose()
        {
            foreach (FrameType frameType in animations.Keys)
            {
                foreach (Maps.Direction8 dir in animations[frameType].Keys)
                {
                    if (animations[frameType][dir] != null)
                    {
                        animations[frameType][dir].Dispose(true);
                    }
                }
            }
        }

        #endregion Methods
    }
}