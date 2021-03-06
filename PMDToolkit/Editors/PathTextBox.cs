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
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PMDToolkit.Editors
{
    public class PathTextBox : TextBox
    {
        [DllImport("user32.dll")]
        private static extern bool HideCaret(IntPtr hWnd);

        public PathTextBox()
        {
            ReadOnly = true;
            BackColor = Color.White;
            Click += TextBox_Click;
            Enter += TextBox_Enter;
            //this.GotFocus += TextBox_GotFocus;
            Cursor = Cursors.Arrow; // mouse cursor like in other controls
        }

        private void TextBox_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbdPath = new FolderBrowserDialog
            {
                SelectedPath = Path.GetFullPath(Text)
            };
            if (fbdPath.ShowDialog() == DialogResult.OK)
            {
                Text = fbdPath.SelectedPath.EndsWith("\\") ? fbdPath.SelectedPath : fbdPath.SelectedPath + "\\";
            }
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            Enabled = false;
            Enabled = true;
        }

        //private void TextBox_GotFocus(object sender, EventArgs e)
        //{
        //    HideCaret(this.Handle);
        //}
    }
}