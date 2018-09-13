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

using System.Drawing;
using System.Windows.Forms;

namespace PMDToolkit.Editors
{
    public class MapLayerListBox : CheckedListBox
    {
        private const int WM_LBUTTONDOWN = 0x0201;

        private const int WM_LBUTTONUP = 0x0202;

        private const int WM_LBUTTONDBLCLK = 0x0203;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_LBUTTONDOWN:
                    {
                        Point point = PointToClient(Cursor.Position);

                        for (int i = 0; i < Items.Count; i++)
                        {
                            Rectangle checkboxRect = GetItemRectangle(i);
                            checkboxRect.Width = checkboxRect.Height;
                            if (checkboxRect.Contains(point))
                            {
                                SetItemChecked(i, !GetItemChecked(i));
                                break;
                            }
                            else if (GetItemRectangle(i).Contains(point))
                            {
                                SelectedIndex = i;
                                break;
                            }
                        }

                        return;
                    }
                case WM_LBUTTONDBLCLK:
                    {
                        Point point = PointToClient(Cursor.Position);

                        for (int i = 0; i < Items.Count; i++)
                        {
                            Rectangle checkboxRect = GetItemRectangle(i);
                            checkboxRect.Width = checkboxRect.Height;
                            if (checkboxRect.Contains(point))
                            {
                                break;
                            }
                            else if (GetItemRectangle(i).Contains(point))
                            {
                                SelectedIndex = i;
                                break;
                            }
                        }

                        return;
                    }
            }

            base.WndProc(ref m);
        }
    }
}