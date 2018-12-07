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

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PMDToolkit.Graphics
{
    public class FontSheet : AtlasSheet
    {
        private int mCellW;
        private int mCellH;

        private int mRows;
        private int mCols;

        private int mCharOffset;

        public FontSheet()
        {
            mCellW = 0;
            mCellH = 0;
            mRows = 0;
            mCols = 0;

            mCharOffset = 0;
        }

        public void LoadFont(string path, int startChar, ref List<int> charList, ref int maxLineHeight, ref int space)
        {
            Color4 cellPixel = new Color4(0, 0, 0, 255);
            Color4 borderPixel = new Color4(255, 0, 0, 255);
            //Get rid of the font if it exists
            FreeFont();
            //Image pixels loaded
            if (LoadPixelsFromFile32(path))
            {
                //Get cell dimensions
                mCellW = 0;
                for (int x = 1; x < ImageWidth; x++)
                {
                    if (GetPixel(x, 1) == cellPixel)
                    {
                        break;
                    }

                    mCellW++;
                }
                mCellW += 2;
                mCols = ImageWidth / mCellW;

                mCellH = 0;
                for (int y = 1; y < ImageHeight; y++)
                {
                    if (GetPixel(y, 1) == cellPixel)
                    {
                        break;
                    }

                    mCellH++;
                }
                mCellH += 2;
                mRows = ImageHeight / mCellH;

                //Begin parsing bitmap font
                mCharOffset = startChar;
                int currentChar = startChar;

                //Go through cell rows
                for (int rows = 0; rows < mRows; rows++)
                {
                    //Go through each cell column in the row
                    for (int cols = 0; cols < mCols; cols++)
                    {
                        //Begin cell parsing
                        //Set base offsets
                        int currentX = mCellW * cols;
                        int currentY = mCellH * rows;

                        if (GetPixel(currentX + 1, currentY + 1) == borderPixel)
                        {
                            if (currentChar == 32)
                            {
                                int width = 0;
                                for (int x = 1; x < ImageWidth; x++)
                                {
                                    if (GetPixel(currentX + x, currentY + 1) != borderPixel)
                                    {
                                        break;
                                    }

                                    width++;
                                }
                                width -= 2;

                                if (width > space)
                                {
                                    space = width;
                                }
                            }
                            else
                            {
                                int width = 0;
                                for (int x = 1; x < ImageWidth; x++)
                                {
                                    if (GetPixel(currentX + x, currentY + 1) != borderPixel)
                                    {
                                        break;
                                    }

                                    width++;
                                }
                                width -= 2;

                                int height = 0;
                                for (int y = 1; y < ImageHeight; y++)
                                {
                                    if (GetPixel(currentX + 1, currentY + y) != borderPixel)
                                    {
                                        break;
                                    }

                                    height++;
                                }
                                height -= 2;

                                //Initialize clip
                                Rectangle nextClip = new Rectangle(mCellW * cols + 2, mCellH * rows + 2, width, height);

                                if (maxLineHeight < nextClip.Height)
                                {
                                    maxLineHeight = nextClip.Height;
                                }

                                charList.Add(currentChar);
                                mClips.Add(nextClip);
                            }
                        }

                        //Go to the next character
                        currentChar++;
                    }
                }

                LoadTextureFromPixels32();
                GenerateDataBuffer(SpriteVOrigin.Top, SpriteHOrigin.Left);
            }
            else
            {
                throw new Exception("Could not load bitmap font image: " + path + "!");
            }
        }

        private void FreeFont()
        {
            //Get rid of sprite sheet
            FreeTexture();
        }

        public void BeginRender()
        {
            //Set texture
            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            //Enable vertex and texture coordinate arrays
            TextureManager.TextureProgram.EnableVertexPointer();
            TextureManager.TextureProgram.EnableTexCoordPointer();
            //Bind vertex data
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVertexDataBuffer);
            //Set texture coordinate data
            TextureManager.TextureProgram.SetTexCoordPointer(VertexData.SizeInBytes, VertexData.TexCoordOffset);
            //Set vertex data
            TextureManager.TextureProgram.SetVertexPointer(VertexData.SizeInBytes, VertexData.PositionOffset);
        }

        public void EndRender()
        {
            //Disable vertex and texture coordinate arrays
            TextureManager.TextureProgram.DisableVertexPointer();
            TextureManager.TextureProgram.DisableTexCoordPointer();
        }

        public void RenderFontSprite(int sprite)
        {
            //If there is a texture to render from

            if (TextureID != 0)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, mIndexBuffers[sprite]);
                GL.DrawElements(BeginMode.Quads, 4, DrawElementsType.UnsignedInt, 0);
            }
        }
    }
}