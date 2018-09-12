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

namespace PMDToolkit.Data
{
    public static class GameData {
        public const int MAX_ITEMS = 30;
        public const int MAX_MOVES = 10;
        public const int MAX_DEX = 718;
        public const int MAX_ROOM_ALGORITHMS = 10;
        public const int MAX_FLOOR_ALGORITHMS = 10;
        public const int MAX_DUNGEON_ALGORITHMS = 10;
        public const int MAX_RDUNGEONS = 10;

        //items
        public static ItemEntry[] ItemDex { get; set; }

        //moves
        public static MoveEntry[] MoveDex { get; set; }

        //characters
        public static DexEntry[] Dex { get; set; }

        //algorithms
        public static RoomAlgorithm[] RoomAlgorithmDex { get; set; }
        public static FloorAlgorithm[] FloorAlgorithmDex { get; set; }
        public static AlgorithmEntry[] DungeonAlgorithmDex { get; set; }

        //random dungeons
        public static RDungeonEntry[] RDungeonDex { get; set; }


        public static void Init()
        {

#if GAME_MODE

            ItemDex = new ItemEntry[MAX_ITEMS];
            for (int i = 0; i < MAX_ITEMS; i++) {
                ItemDex[i] = new ItemEntry();
                ItemDex[i].Load(i);
            }
            
            MoveDex = new MoveEntry[MAX_MOVES];
            for (int i = 0; i < MAX_MOVES; i++) {
                MoveDex[i] = new MoveEntry();
                MoveDex[i].Load(i);
            }

#endif
            Dex = new DexEntry[MAX_DEX+1];
            for (int i = 0; i <= MAX_DEX; i++)
            {
                try
                {
                    Dex[i] = new DexEntry();
                    Dex[i].Load(i);
                }
                catch (Exception ex)
                {
                    Logs.Logger.LogError(ex);
                }
            }

#if GAME_MODE
            RoomAlgorithmDex = new RoomAlgorithm[MAX_ROOM_ALGORITHMS];
            for (int i = 0; i < MAX_ROOM_ALGORITHMS; i++) {
                RoomAlgorithmDex[i] = new RoomAlgorithm();
                RoomAlgorithmDex[i].Load(i);
            }

            FloorAlgorithmDex = new FloorAlgorithm[MAX_FLOOR_ALGORITHMS];
            for (int i = 0; i < MAX_FLOOR_ALGORITHMS; i++) {
                FloorAlgorithmDex[i] = new FloorAlgorithm();
                FloorAlgorithmDex[i].Load(i);
            }

            DungeonAlgorithmDex = new AlgorithmEntry[MAX_DUNGEON_ALGORITHMS];
            for (int i = 0; i < MAX_DUNGEON_ALGORITHMS; i++) {
                DungeonAlgorithmDex[i] = new AlgorithmEntry();
                DungeonAlgorithmDex[i].Load(i);
            }

            RDungeonDex = new RDungeonEntry[MAX_RDUNGEONS];
            for (int i = 0; i < MAX_RDUNGEONS; i++) {
                RDungeonDex[i] = new RDungeonEntry();
                RDungeonDex[i].Load(i);
            }
#endif
        }

        public static int GetMove(string moveName) {
#if GAME_MODE
            for (int i = 0; i < MoveDex.Length; i++) {
                if (MoveDex[i].Name == moveName) return i;
            }
#endif
            return -1;
        }

    }
}
