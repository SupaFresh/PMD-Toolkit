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

namespace PMDToolkit.Maps
{
    public class Item
    {
        public int ItemIndex { get; set; }
        public int Amount { get; set; }
        public string Tag { get; set; }
        public bool Disabled { get; set; }
        public Loc2D ItemLoc { get; set; }

        public Item()
        {
            ItemIndex = -1;
            Amount = 0;
            Tag = "";
            Disabled = false;
            ItemLoc = new Loc2D();
        }

        public Item(int itemIndex, int amount, string tag, bool disabled, Loc2D loc)
        {
            ItemIndex = itemIndex;
            Amount = amount;
            Tag = tag;
            Disabled = disabled;
            ItemLoc = loc;
        }

        public void Draw()
        {
            if (ItemIndex > -1)
            {
                AnimSheet sheet = TextureManager.GetItemSheet(Data.GameData.ItemDex[ItemIndex].Sprite);

                TextureManager.TextureProgram.PushModelView();
                TextureManager.TextureProgram.LeftMultModelView(Matrix4.CreateTranslation((TextureManager.TILE_SIZE - sheet.TileWidth) / 2, (TextureManager.TILE_SIZE - sheet.TileHeight) / 2, 0));
                TextureManager.TextureProgram.UpdateModelView();

                sheet.RenderTile(0, 0);

                TextureManager.TextureProgram.PopModelView();
            }
        }
    }
}