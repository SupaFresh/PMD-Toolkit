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

namespace PMDToolkit.Enums
{
    public enum GrowthGroup
    {
        None = 0,
        Erratic = 1,
        Fast = 2,
        MediumFast = 3,
        MediumSlow = 4,
        Slow = 5,
        Fluctuating = 6
    }

    public enum EggGroup
    {
        Undiscovered = 0,
        Ditto = 1,
        Monster = 2,
        Water1 = 3,
        Bug = 4,
        Flying = 5,
        Field = 6,
        Fairy = 7,
        Grass = 8,
        Humanlike = 9,
        Water3 = 10,
        Mineral = 11,
        Amorphous = 12,
        Water2 = 13,
        Dragon = 14
    }

    public enum BodyShape
    {
        Head,
        Serpentine,
        Fins,
        HeadAndArms,
        HeadAndBase,
        BipedWithTail,
        HeadAndLegs,
        Quadruped,
        Wings,
        Tentacles,
        MultiBody,
        Biped,
        MultiWings,
        Insectoid
    }

    public enum ItemType
    {
        None = 0,
        Use = 1,
        Hold = 2,
        Party = 3,
        Bag = 4
    };

    public enum Element
    {
        None,
        Bug,
        Dark,
        Dragon,
        Electric,
        Fairy,
        Fighting,
        Fire,
        Flying,
        Ghost,
        Grass,
        Ground,
        Ice,
        Normal,
        Poison,
        Psychic,
        Rock,
        Steel,
        Water
    };

    public enum MoveCategory
    {
        Physical,
        Special,
        Status
    };

    public enum Weather
    {
        Ambiguous = -1,//for server weather.  counts as "none" in every other case.
        None = 0,
        Raining = 1,
        Snowing = 2,
        Rainstorm = 3,
        Hail = 4,
        DiamondDust = 5,
        Cloudy = 6,
        Fog = 7,
        Sunny = 8,
        Sandstorm = 9,
        Snowstorm = 10,
        Ashfall = 11
    };

    public enum StatusAilment
    {
        OK,
        Burn,
        Freeze,
        Paralyze,
        Poison,
        Sleep
    }

    public enum RangeType
    {
        None = 0,
        Front = 1,
        FrontUntil = 2,
        Room = 3,
        FrontAndSides = 4,
        FlyInArc = 5,
        TileInFrontUntil = 6,
        ThreeDirections = 7,
        EightDirections = 8
    };

    public enum Alignment
    {
        None = 0,
        Self = 1,
        Friend = 2,
        Foe = 3
    };

    public enum TileType
    {
        Void = -1,
        Walkable = 0,
        Blocked = 3,
        Water = 4,
        Ledge = 7,
        Slippery = 8,
        Warp = 9,
        Crack = 10,
        Pit = 11,
        ChangeFloor = 12,
        Trap = 13
    };

    public enum WalkMode
    {
        Normal = 0,
        Air = 1,
        All = 2
    }

    public enum Gender
    {
        Unknown = -1,
        Genderless = 0,
        Male = 1,
        Female = 2
    }

    public enum Coloration
    {
        Unknown = -1,
        Normal = 0,
        Shiny = 1
    }
}