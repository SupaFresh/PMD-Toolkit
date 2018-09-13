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

using PMDToolkit.Data;
using System;
using System.Collections.Generic;

namespace PMDToolkit.Maps.Dungeons
{
    public class FloorByFloor : BasicDungeon
    {
        public override void Generate(int seed, RDungeonEntry entry)
        {
            this.seed = seed;
            this.entry = entry;

            rand = new Random(seed);

            for (int i = 0; i < entry.Floors.Count - 1; i++)
            {
                floorLinks.Add(new FloorLink(i, 1), new FloorLink(i + 1, 0));
            }
            floorLinks.Add(new FloorLink(entry.Floors.Count - 1, 1), new FloorLink(-1, 0));

            Start = new Loc3D(this[0].BorderPoints[0].X, this[0].BorderPoints[0].Y, 0);
        }

        //re-call-able floor getting methods, caching where needed
        protected override void generateFloor(int floor)
        {
            //borders are orders to the floor generator for entrance/exit data: requirements of where they need to be, and which direction they are
            List<FloorBorder> borders = new List<FloorBorder>();
            borders.Add(new FloorBorder(null, Direction3D.Forth));
            borders.Add(new FloorBorder(null, Direction3D.Back));

            //borderLinks are orders to the floor generator for entrance/exit data: requirements of which nodes must connect to each other
            Dictionary<int, List<int>> borderLinks = new Dictionary<int, List<int>>();
            List<int> ends = new List<int>();
            ends.Add(1);
            borderLinks.Add(0, ends);

            RandomMap map = GameData.FloorAlgorithmDex[entry.Floors[floor].Algorithm].CreateFloor();
            map.Generate(rand.Next(), entry.Floors[floor], borders, borderLinks);
            maps.Add(floor, map);
        }
    }
}