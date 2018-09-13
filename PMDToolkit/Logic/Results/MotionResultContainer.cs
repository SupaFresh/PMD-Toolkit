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
using System.Collections.Generic;

namespace PMDToolkit.Logic.Results
{
    public class MotionResultContainer : IResultContainer
    {
        private Dictionary<int, ResultBranch> branches;
        private int currentId;

        public int BranchCount { get { return branches.Count; } }

        public bool Empty
        {
            get
            {
                foreach (KeyValuePair<int, ResultBranch> entry in branches)
                {
                    if (entry.Value.Results.Count > 0)
                        return false;
                }
                return true;
            }
        }

        public MotionResultContainer(int id)
        {
            branches = new Dictionary<int, ResultBranch>();
            OpenBranch(id);
        }

        public bool IsFinished()
        {
            foreach (KeyValuePair<int, ResultBranch> entry in branches)
            {
                if (entry.Value.Results.Count > 0 || entry.Value.Delay > RenderTime.Zero)
                    return false;
            }
            return true;
        }

        public void OpenBranch(int id)
        {
            if (!branches.ContainsKey(id))
                branches.Add(id, new ResultBranch());

            currentId = id;
        }

        public void AddResult(IResult result)
        {
            branches[currentId].Results.Enqueue(result);
        }

        public bool IsBranchEmpty()
        {
            return (branches[currentId].Results.Count == 0);
        }

        public void ProcessDelay(RenderTime time)
        {
            foreach (KeyValuePair<int, ResultBranch> entry in branches)
            {
                entry.Value.Delay -= time;
                if (entry.Value.Delay < RenderTime.Zero)
                    entry.Value.Delay = RenderTime.Zero;
            }
        }

        public IEnumerable<ResultBranch> GetAllBranches()
        {
            foreach (KeyValuePair<int, ResultBranch> entry in branches)
            {
                yield return entry.Value;
            }
        }
    }
}