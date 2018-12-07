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

using PMDToolkit.Graphics;
using PMDToolkit.Logic.Display;
using PMDToolkit.Maps;
using System.Collections.Generic;

namespace PMDToolkit.Logic.Results
{
    public class SetTile : IResult
    {
        //public ResultType Type { get { return ResultType.SetTile; } }
        public RenderTime Delay => RenderTime.Zero;

        private Loc2D loc;
        private readonly Tile tile;
        public List<TileAnim> groundLayers;
        public List<TileAnim> propBackLayers;
        public List<TileAnim> propFrontLayers;
        public List<TileAnim> fringeLayers;

        public SetTile(BasicMap map, Loc2D loc)
        {
            this.loc = loc;
            tile = new Tile(map.MapArray[loc.X, loc.Y]);
            groundLayers = new List<TileAnim>();
            for (int i = 0; i < map.GroundLayers.Count; i++)
            {
                groundLayers.Add(map.GroundLayers[i].Tiles[loc.X, loc.Y]);
            }
            propBackLayers = new List<TileAnim>();
            for (int i = 0; i < map.PropBackLayers.Count; i++)
            {
                propBackLayers.Add(map.PropBackLayers[i].Tiles[loc.X, loc.Y]);
            }
            propFrontLayers = new List<TileAnim>();
            for (int i = 0; i < map.PropFrontLayers.Count; i++)
            {
                propFrontLayers.Add(map.PropFrontLayers[i].Tiles[loc.X, loc.Y]);
            }
            fringeLayers = new List<TileAnim>();
            for (int i = 0; i < map.FringeLayers.Count; i++)
            {
                fringeLayers.Add(map.FringeLayers[i].Tiles[loc.X, loc.Y]);
            }
        }

        public void Execute()
        {
            Screen.Map.MapArray[loc.X, loc.Y] = tile;
            for (int i = 0; i < groundLayers.Count; i++)
            {
                Screen.Map.GroundLayers[i][loc.X, loc.Y] = groundLayers[i];
            }
            for (int i = 0; i < propBackLayers.Count; i++)
            {
                Screen.Map.PropBackLayers[i][loc.X, loc.Y] = propBackLayers[i];
            }
            for (int i = 0; i < propFrontLayers.Count; i++)
            {
                Screen.Map.PropFrontLayers[i][loc.X, loc.Y] = propFrontLayers[i];
            }
            for (int i = 0; i < fringeLayers.Count; i++)
            {
                Screen.Map.FringeLayers[i][loc.X, loc.Y] = fringeLayers[i];
            }
        }
    }
}