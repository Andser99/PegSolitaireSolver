using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IQSoliterSolver
{
    class Program
    {
        private static bool DEBUG = false;

        //the end pattern for levels 1 to 6 are the same
        private static int[] endPattern1 = new int[]
        {
            0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 1, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0
        };
        //retarded end pattern for level 7, messing with my hard coded end patterns
        //and efficient end condition one liner check
        private static int[] endPattern2 = new int[]
        {
            0, 0, 1, 1, 1, 0, 0,
            0, 0, 1, 0, 1, 0, 0,
            1, 1, 1, 0, 1, 1, 1,
            1, 0, 0, 0, 0, 0, 1,
            1, 1, 1, 0, 1, 1, 1,
            0, 0, 1, 0, 1, 0, 0,
            0, 0, 1, 1, 1, 0, 0
        };
        //level 8
        private static int[] endPattern3 = new int[]
        {
            0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0,
            0, 0, 1, 1, 1, 0, 0,
            0, 0, 1, 0, 1, 0, 0,
            0, 0, 1, 1, 1, 0, 0,
            0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0,
        };
        private static int[] game1 = new int[] // field that can be played
        {
            0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 1, 0, 0, 0,
            0, 0, 1, 1, 1, 0, 0,
            0, 0, 0, 1, 0, 0, 0,
            0, 0, 0, 1, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0

        };
        private static int[] game3 = new int[]
        {
            0, 0, 1, 1, 1, 0, 0,
            0, 0, 1, 1, 1, 0, 0,
            0, 0, 1, 1, 1, 0, 0,
            0, 0, 1, 0, 1, 0, 0,
            0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0

        };
        private static int[] game4 = new int[]
        {
            0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 1, 0, 0, 0,
            0, 0, 1, 1, 1, 0, 0,
            0, 1, 1, 1, 1, 1, 0,
            1, 1, 1, 1, 1, 1, 1,
            0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0

        };
        private static int[] game5 = new int[]
        {
            0, 0, 0, 1, 0, 0, 0,
            0, 0, 1, 1, 1, 0, 0,
            0, 1, 1, 1, 1, 1, 0,
            0, 0, 0, 1, 0, 0, 0,
            0, 0, 0, 1, 0, 0, 0,
            0, 0, 1, 1, 1, 0, 0,
            0, 0, 1, 1, 1, 0, 0

        };
        private static int[] game6 = new int[]
        {
            0, 0, 0, 1, 0, 0, 0,
            0, 0, 1, 1, 1, 0, 0,
            0, 1, 1, 1, 1, 1, 0,
            1, 1, 1, 0, 1, 1, 1,
            0, 1, 1, 1, 1, 1, 0,
            0, 0, 1, 1, 1, 0, 0,
            0, 0, 0, 1, 0, 0, 0,

        };
        private static int[] game7 = new int[]
        {
            0, 0, 1, 1, 1, 0, 0,
            0, 0, 1, 1, 1, 0, 0,
            1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 0, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1,
            0, 0, 1, 1, 1, 0, 0,
            0, 0, 1, 1, 1, 0, 0,

        };
        //game 7 and 8 have the same layout
        private static int[] game8 = new int[]
        {
            0, 0, 1, 1, 1, 0, 0,
            0, 0, 1, 1, 1, 0, 0,
            1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 0, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1,
            0, 0, 1, 1, 1, 0, 0,
            0, 0, 1, 1, 1, 0, 0,

        };


        //replace gameX endPatternY with any from above.
        static void Main(string[] args)
        {
            var res = FindSolution(new State(game8), endPattern3).GetAwaiter().GetResult();
            State.PrintSolutionBackwardsFromArrays(res.Explored, res.Parents, res.StartIndex);
        }

        private static readonly object hashLock = new();
        private static readonly object exploredArrLock = new();
        async static Task<(int StartIndex, long[] Explored, uint[] Parents)> FindSolution(State start, int[] endPattern)
        {
            State.END_PATTERN = endPattern;
            var exploredList = new List<State>();
            var exploredArr = new long[2146233066];
            exploredArr[0] = start.RecalculateHash();
            uint[] parent = new uint[2146233066];
            exploredList.Add(start);
            var found = false;
            var index = 0;
            var appendIndex = 0;
            var hashDict = new HashSet<long>();
            var endPatternCountGreen = endPattern.Sum();

            // Seed once to create initial moves
            List<State> newFound = new State(exploredArr[index], index).GetAllMoves();
            foreach (var x in newFound)
            {
                if (DEBUG)
                {
                    Console.WriteLine(exploredList.Count - 1);
                    x.Print();
                }
                //Check if such game state wasn't already explored
                if (!hashDict.Contains(x.Hash) && endPatternCountGreen < x.CountGreen)
                {
                    //If no, then add it
                    hashDict.Add(x.Hash);
                    appendIndex++;
                    exploredArr[appendIndex] = x.Hash;
                    parent[appendIndex] = (uint)index;
                    //toAdd.Add(x);
                }

                if (x.IsFinished(endPattern)) return (appendIndex, exploredArr, parent);
            }

            while (!found)
            {
                // Old List implementation
                //List<State> newFound = exploredList[index].GetAllMoves();
                //List<State> toAdd = new List<State>();

                var t1 = new Task<State>(() => {
                    List<State> newFound = new State(exploredArr[index * 4 + 0], index).GetAllMoves();
                    foreach (var x in newFound)
                    {
                        lock (hashLock)
                        {
                            if (!hashDict.Contains(x.Hash) && endPatternCountGreen < x.CountGreen)
                            {
                                hashDict.Add(x.Hash);
                                lock (exploredArrLock)
                                {
                                    appendIndex++;
                                    exploredArr[appendIndex] = x.Hash;
                                    parent[appendIndex] = (uint)index * 4 + 0;
                                }
                            }
                        }

                        if (x.IsFinished(endPattern)) return x;
                    }
                    return State.NULL_STATE;
                });
                var t2 = new Task<State>(() => {
                    List<State> newFound = new State(exploredArr[index * 4 + 1], index).GetAllMoves();
                    foreach (var x in newFound)
                    {
                        lock (hashLock)
                        {
                            if (!hashDict.Contains(x.Hash) && endPatternCountGreen < x.CountGreen)
                            {
                                hashDict.Add(x.Hash);
                                lock (exploredArrLock)
                                {
                                    appendIndex++;
                                    exploredArr[appendIndex] = x.Hash;
                                    parent[appendIndex] = (uint)index * 4 + 1;
                                }
                            }
                        }

                        if (x.IsFinished(endPattern)) return x;
                    }
                    return State.NULL_STATE;
                });
                var t3 = new Task<State>(() => {
                    List<State> newFound = new State(exploredArr[index * 4 + 2], index).GetAllMoves();
                    foreach (var x in newFound)
                    {
                        lock (hashLock)
                        {
                            if (!hashDict.Contains(x.Hash) && endPatternCountGreen < x.CountGreen)
                            {
                                hashDict.Add(x.Hash);
                                lock (exploredArrLock)
                                {
                                    appendIndex++;
                                    exploredArr[appendIndex] = x.Hash;
                                    parent[appendIndex] = (uint)index * 4 + 2;
                                }
                            }
                        }

                        if (x.IsFinished(endPattern)) return x;
                    }
                    return State.NULL_STATE;
                });
                var t4 = new Task<State>(() => {
                    List<State> newFound = new State(exploredArr[index * 4 + 3], index).GetAllMoves();
                    foreach (var x in newFound)
                    {
                        lock (hashLock)
                        {
                            if (!hashDict.Contains(x.Hash) && endPatternCountGreen < x.CountGreen)
                            {
                                hashDict.Add(x.Hash);
                                lock (exploredArrLock)
                                {
                                    appendIndex++;
                                    exploredArr[appendIndex] = x.Hash;
                                    parent[appendIndex] = (uint)index * 4 + 3;
                                }
                            }
                        }

                        if (x.IsFinished(endPattern)) return x;
                    }
                    return State.NULL_STATE;
                });
                //Only new states are added to exploration list
                // Old List implementation
                //exploredList.AddRange(toAdd);
                t1.Start();
                t2.Start();
                t3.Start();
                t4.Start();
                await Task.WhenAll(t1, t2, t3, t4);
                if (State.SOLUTION != State.NULL_STATE) return (appendIndex, exploredArr, parent);
                index++;
            }
            return (appendIndex, exploredArr, parent);
        }
    }
}
