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

using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PMDToolkit.Graphics
{
    public class Font : IDisposable
    {
        //Spacing variables
        private int mSpace;

        private int mCharHeight;

        private List<FontSheet> mFontSheets;

        private Dictionary<int, Tuple<int, int>> mCharToVertIndexMap;

        public Font()
        {
            mSpace = 0;
            mCharHeight = 0;

            mFontSheets = new List<FontSheet>();
            mCharToVertIndexMap = new Dictionary<int, Tuple<int, int>>();
        }

        public void LoadFont(string path, int startChar)
        {
            List<int> listChars = new List<int>();
            FontSheet sheet = new FontSheet();
            sheet.LoadFont(path, startChar, ref listChars, ref mCharHeight, ref mSpace);
            for (int i = 0; i < listChars.Count; i++)
            {
                mCharToVertIndexMap.Add(listChars[i], new Tuple<int, int>(mFontSheets.Count, i));
            }
            mFontSheets.Add(sheet);
        }

        public virtual void Dispose(bool v)
        {
            //Clear sprite sheet data
            FreeFont();
            Dispose(true);
        }

        private void FreeFont()
        {
            //Get rid of sprite sheet

            for (int i = 0; i < mFontSheets.Count; i++)
            {
                mFontSheets[i].Dispose(true);
            }
            //Reinitialize spacing constants
            mSpace = 0;
            mCharHeight = 0;
        }

        public void RenderText(float x, float y, string text, Rectangle? area, AtlasSheet.SpriteVOrigin vOrigin, AtlasSheet.SpriteHOrigin hOrigin, int lineSpace, Color4 color)
        {
            TextureManager.TextureProgram.SetTextureColor(color);
            RenderText(x, y, text, area, vOrigin, hOrigin, lineSpace);
            TextureManager.TextureProgram.SetTextureColor(Color4.White);
        }

        public void RenderText(float x, float y, string text, Rectangle? area, AtlasSheet.SpriteVOrigin vOrigin, AtlasSheet.SpriteHOrigin hOrigin, int lineSpace)
        {
            //If there is a texture to render from
            TextureManager.TextureProgram.PushModelView();

            //Draw positions
            float dX = x;
            float dY = y;

            //If the text needs to be aligned
            if (area == null)
            {
                area = new Rectangle((int)x, (int)y, 0, 0);
            }

            //Set origin (vertical)
            switch (vOrigin)
            {
                case AtlasSheet.SpriteVOrigin.Top:
                    {
                        dY = area.Value.Top;
                    }
                    break;

                case AtlasSheet.SpriteVOrigin.Bottom:
                    {
                        dY = area.Value.Bottom - StringHeight(text, lineSpace);
                    }
                    break;

                default:
                    {
                        dY = (area.Value.Top + area.Value.Bottom - StringHeight(text, lineSpace)) / 2;
                    }
                    break;
            }
            //Set origin (horizontal)
            switch (hOrigin)
            {
                case AtlasSheet.SpriteHOrigin.Left:
                    {
                        dX = area.Value.Left;
                    }
                    break;

                case AtlasSheet.SpriteHOrigin.Right:
                    {
                        dX = area.Value.Right - SubstringWidth(text);
                    }
                    break;

                default:
                    {
                        dX = (area.Value.Left + area.Value.Right - SubstringWidth(text)) / 2;
                    }
                    break;
            }

            //Move to draw position
            TextureManager.TextureProgram.LeftMultModelView(Matrix4.CreateTranslation(dX, dY, 0));

            int currentTexture = -1;

            //Go through string
            for (int i = 0; i < text.Length; i++)
            {
                //Space
                if (text[i] == ' ')
                {
                    TextureManager.TextureProgram.LeftMultModelView(Matrix4.CreateTranslation(mSpace, 0, 0));
                    TextureManager.TextureProgram.UpdateModelView();
                    dX += mSpace;
                }
                //Newline
                else if (text[i] == '\n')
                {
                    //Handle horizontal alignment
                    float targetX = x;
                    switch (hOrigin)
                    {
                        case AtlasSheet.SpriteHOrigin.Left:
                            {
                                targetX = area.Value.Left;
                            }
                            break;

                        case AtlasSheet.SpriteHOrigin.Right:
                            {
                                targetX = area.Value.Right - SubstringWidth(text[i + 1].ToString());
                            }
                            break;

                        default:
                            {
                                targetX = (area.Value.Left + area.Value.Right - SubstringWidth(text.Substring(i + 1).ToString())) / 2;
                            }
                            break;
                    }
                    TextureManager.TextureProgram.LeftMultModelView(Matrix4.CreateTranslation(targetX - dX, lineSpace, 0));
                    TextureManager.TextureProgram.UpdateModelView();
                    dY += lineSpace;
                    dX += targetX - dX;
                }
                //Character
                else
                {
                    //Get ASCII
                    if (mCharToVertIndexMap.ContainsKey(text[i]))
                    {
                        //Update position matrix in program
                        TextureManager.TextureProgram.UpdateModelView();

                        //Get ASCII code's index in the font array
                        Tuple<int, int> texture_char = mCharToVertIndexMap[text[i]];

                        if (currentTexture != texture_char.Item1)
                        {
                            if (currentTexture != -1)
                                mFontSheets[currentTexture].EndRender();

                            mFontSheets[texture_char.Item1].BeginRender();
                            currentTexture = texture_char.Item1;
                        }

                        Rectangle char_rect = mFontSheets[texture_char.Item1].GetClip(texture_char.Item2);
                        mFontSheets[texture_char.Item1].RenderFontSprite(texture_char.Item2);

                        //Move over
                        TextureManager.TextureProgram.LeftMultModelView(Matrix4.CreateTranslation(char_rect.Width, 0, 0));
                        TextureManager.TextureProgram.UpdateModelView();

                        dX += char_rect.Width;
                    }
                    TextureManager.TextureProgram.UpdateModelView();
                }
            }

            if (currentTexture != -1)
                mFontSheets[currentTexture].EndRender();

            TextureManager.TextureProgram.PopModelView();
            TextureManager.TextureProgram.UpdateModelView();
        }

        private Rectangle GetStringArea(string text, int lineSpace)
        {
            //Initialize area
            float subWidth = 0;
            Rectangle area = new Rectangle(0, 0, (int)subWidth, mCharHeight);
            //Go through string
            for (int i = 0; i < text.Length; ++i)
            {
                //Space
                if (text[i] == ' ')
                {
                    subWidth += mSpace;
                }
                //Newline
                else if (text[i] == '\n')
                {
                    //Add another line
                    area.Height += mCharHeight;
                    //Check for max width
                    if (subWidth > area.Width)
                    {
                        area.Width = (int)subWidth;
                        subWidth = 0;
                    }
                } //Character
                else
                {
                    if (mCharToVertIndexMap.ContainsKey(text[i]))
                    {
                        //Get ASCII
                        Tuple<int, int> texture_char = mCharToVertIndexMap[text[i]];
                        Rectangle char_rect = mFontSheets[texture_char.Item1].GetClip(texture_char.Item2);
                        subWidth += char_rect.Width;
                    }
                }
            } //Check for max width
            if (subWidth > area.Width)
            {
                area.Width = (int)subWidth;
            }
            return area;
        }

        public int StringHeight(string thisString, int lineSpace)
        {
            int height = mCharHeight;
            //Go through string
            for (int i = 0; i < thisString.Length; ++i)
            {
                //Space
                if (thisString[i] == '\n')
                {
                    height += mCharHeight + lineSpace;
                }
            }
            return height;
        }

        public int SubstringWidth(string substring)
        {
            int subWidth = 0;
            //Go through string
            for (int i = 0; i < substring.Length && substring[i] != '\n'; i++)
            {
                //Space
                if (substring[i] == ' ')
                {
                    subWidth += mSpace;
                }
                //Character
                else
                {
                    if (mCharToVertIndexMap.ContainsKey(substring[i]))
                    {
                        //Get ASCII
                        Tuple<int, int> texture_char = mCharToVertIndexMap[substring[i]];
                        Rectangle char_rect = mFontSheets[texture_char.Item1].GetClip(texture_char.Item2);
                        subWidth += char_rect.Width;
                    }
                }
            }
            return subWidth;
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            return;
        }
    }
}