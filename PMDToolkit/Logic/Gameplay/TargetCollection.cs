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

using System.Collections.Generic;

namespace PMDToolkit.Logic.Gameplay
{
    public class TargetCollection
    {
        public int Self { get; private set; }
        public int Friends { get; private set; }
        public int Foes { get; private set; }

        public List<Target> Targets { get; }
        public List<TileTarget> Tiles { get; }

        public TargetCollection()
        {
            Targets = new List<Target>();
            Tiles = new List<TileTarget>();
        }

        public void Add(Target target)
        {
            if (target.TargetAlignment == Enums.Alignment.Self)
            {
                Self++;
            }
            else if (target.TargetAlignment == Enums.Alignment.Friend)
            {
                Friends++;
            }
            else if (target.TargetAlignment == Enums.Alignment.Foe)
            {
                Foes++;
            }
            Targets.Add(target);
        }

        public void Add(TileTarget tile)
        {
            Tiles.Add(tile);
        }
    }
}