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

namespace PMDToolkit.Maps
{
    public class Tile
    {
        public TileData Data;

        public Tile()
        {
            Data = new TileData(Enums.TileType.Void, 0, 0, 0, "", "", "");
        }

        public Tile(Tile tile)
        {
            Data = tile.Data;
        }

        public Tile(Enums.TileType type, int data1, int data2, int data3)
        {
            Data = new TileData(type, data1, data2, data3, "", "", "");
        }

        public Tile(Enums.TileType type, int data1, int data2, int data3, string tileString1, string tileString2, string tileString3)
        {
            Data = new TileData(type, data1, data2, data3, tileString1, tileString2, tileString3);
        }
    }
}