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

using System.Drawing;
using System.IO;

namespace PMDToolkit.Graphics
{
    public class TileMetadata
    {
        private long headerSize;
        private long[] tilePositions;

        public int[] TileSizes { get; set; }

        public int TotalTiles { get { return tilePositions.Length; } }

        public Size Size { get; set; }

        public long GetTilePosition(int index)
        {
            return tilePositions[index] + headerSize;
        }

        public void Load(string filePath)
        {
            // File format:
            // [tileset-width(4)][tileset-height(4)][tile-count(4)]
            // [tileposition-1(4)][tilesize-1(4)][tileposition-2(4)][tilesize-2(4)][tileposition-n(n*4)][tilesize-n(n*4)]
            // [tile-1(variable)][tile-2(variable)][tile-n(variable)]
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                using (BinaryReader reader = new BinaryReader(fileStream))
                {
                    // Read tileset width
                    Size = new Size (reader.ReadInt32(), Size.Height);
                    // Read tileset height
                    Size = new Size(Size.Height, reader.ReadInt32());

                    int tileCount = (Size.Width / TextureManager.TILE_SIZE) * (Size.Height / TextureManager.TILE_SIZE);

                    // Prepare tile information cache
                    tilePositions = new long[tileCount];
                    TileSizes = new int[tileCount];

                    // Load tile information
                    for (int i = 0; i < tileCount; i++)
                    {
                        // Read tile position data
                        tilePositions[i] = reader.ReadInt64();
                        // Read tile size data
                        TileSizes[i] = reader.ReadInt32();
                    }
                    headerSize = fileStream.Position;
                }
            }
        }
    }
}