using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQSoliterSolver
{
    class State
    {
        //0 == out of bounds, 1 = playable field
        private static int BOARD_WIDTH = 7;
        private static int BOARD_HEIGHT = 7;
        public static readonly State NULL_STATE = new State();
        private static List<int> playableField = new List<int>() // field that can be played
        {
            0, 0, 1, 1, 1, 0, 0,
            0, 0, 1, 1, 1, 0, 0,
            1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1,
            0, 0, 1, 1, 1, 0, 0,
            0, 0, 1, 1, 1, 0, 0

        };

        public static int[] END_PATTERN;
        public static State SOLUTION = NULL_STATE;


        private long _hash = 0;
        public long Hash
        {
            get
            {
                return _hash;
            }
        }
        private int _countGreen = 0;
        public int CountGreen
        {
            get
            {
                return _countGreen;
            }
        }
        private long _prevIndex = -1;
        public long PrevIndex
        {
            get
            {
                return _prevIndex;
            }
        }
        //0 == empty, 1 == green
        private List<int> _board = new List<int>(48);

        public State Previous = NULL_STATE;

        public State() { }
        public State(long bitmask, long prevIndex = -1)
        {
            _prevIndex = prevIndex;
            _countGreen = 0;
            for (int i = 0; i < 49; i++)
            {
                var nextVal = (int)((bitmask >> i) & 1);
                _board.Add(nextVal);
                _countGreen += nextVal;
            }
        }
        public State(int[] stateArray)
        {
            _board = new List<int>(stateArray);
            _countGreen = stateArray.Sum();
        }


        public List<State> GetAllMoves()
        {
            var newStates = new List<State>();
            for (var i = 0; i < _board.Count(); i++)
            {
                if (_board[i] == 1) // Found a green tile
                {
                    var states = CheckNeighbours(i);
                    newStates.AddRange(states);
                }
            }
            return newStates;
        }
        // LEFT MOVE means the green blob is being moved to the left
        // creating 2 empty squares to its right
        private List<State> CheckNeighbours(int i)
        {
            var states = new List<State>();
            //LEFT MOVE
            if ((i % BOARD_WIDTH) > 1)
            {
                //Possible move to the left
                // check the adjacent square for green
                // check the square over the green for a white
                // check if the "white" square is still within playable bounds
                // No need to check whether the neighbouring green is in bounds
                // since the game progression doesn't allow it unless you paint
                // out of bounds in the first place
                if (_board[i - 1] == 1 
                    && _board[i - 2] == 0 
                    && playableField[i - 2] == 1)
                {
                    var newState = new State(_board.ToArray());
                    newState._board[i] = 0;
                    newState._board[i - 1] = 0;
                    newState._board[i - 2] = 1;
                    newState.Previous = this;
                    newState.RecalculateHash();
                    newState._countGreen = _countGreen - 1;
                    //newState._prevIndex = _prevIndex;
                    states.Add(newState);
                    if (newState.IsFinished(END_PATTERN))
                    {
                        SOLUTION = newState;
                    }
                }
            }
            //RIGHT MOVE
            if ((i % BOARD_WIDTH) < BOARD_WIDTH - 2)
            {
                if (_board[i + 1] == 1 
                    && _board[i + 2] == 0 
                    && playableField[i + 2] == 1)
                {
                    var newState = new State(_board.ToArray());
                    newState._board[i] = 0;
                    newState._board[i + 1] = 0;
                    newState._board[i + 2] = 1;
                    newState.Previous = this;
                    newState.RecalculateHash();
                    newState._countGreen = _countGreen - 1;
                    //newState._prevIndex = _prevIndex;
                    states.Add(newState);
                    if (newState.IsFinished(END_PATTERN))
                    {
                        SOLUTION = newState;
                    }
                }
            }
            //TOP MOVE
            if ((i / BOARD_WIDTH) > 1)
            {
                if (_board[i - BOARD_WIDTH] == 1 
                    && _board[i - BOARD_WIDTH * 2] == 0 
                    && playableField[i - BOARD_WIDTH * 2] == 1)
                {
                    var newState = new State(_board.ToArray());
                    newState._board[i] = 0;
                    newState._board[i - BOARD_WIDTH] = 0;
                    newState._board[i - BOARD_WIDTH * 2] = 1;
                    newState.Previous = this;
                    newState.RecalculateHash();
                    newState._countGreen = _countGreen - 1;
                    //newState._prevIndex = _prevIndex;
                    states.Add(newState);
                    if (newState.IsFinished(END_PATTERN))
                    {
                        SOLUTION = newState;
                    }
                }
            }
            //BOTTOM MOVE
            if ((i / BOARD_WIDTH) < BOARD_HEIGHT - 2)
            {
                if (_board[i + BOARD_WIDTH] == 1
                    && _board[i + BOARD_WIDTH * 2] == 0
                    && playableField[i + BOARD_WIDTH * 2] == 1)
                {
                    var newState = new State(_board.ToArray());
                    newState._board[i] = 0;
                    newState._board[i + BOARD_WIDTH] = 0;
                    newState._board[i + BOARD_WIDTH * 2] = 1;
                    newState.Previous = this;
                    newState.RecalculateHash();
                    newState._countGreen = _countGreen - 1;
                    //newState._prevIndex = _prevIndex;
                    states.Add(newState);
                    if (newState.IsFinished(END_PATTERN))
                    {
                        SOLUTION = newState;
                    }
                }
            }
            return states;
        }

        public void Print()
        {
            for (int i = 0; i < _board.Count(); i++)
            {
                Console.ForegroundColor = _board[i] == 1 ? ConsoleColor.Green : ConsoleColor.White;
                Console.Write($"{(playableField[i] == 1 ? "[]" : "  ")}");
                if (i % BOARD_WIDTH == BOARD_WIDTH - 1) Console.WriteLine();
            }
        }

        public void PrintSolutionBackwards()
        {
            Print();
            var parent = Previous;
            while (parent != NULL_STATE)
            {
                Console.WriteLine();
                parent.Print();
                parent = parent.Previous;
            }
        }

        public static void PrintSolutionBackwardsFromArrays(long[] explored, uint[] parents, int startIndex)
        {
            Console.WriteLine(startIndex);
            State s = new State(explored[startIndex]);
            int nextIndex = (int)parents[startIndex];
            s.Print();
            while (nextIndex != 0)
            {
                Console.WriteLine(nextIndex);
                s = new State(explored[nextIndex]);
                s.Print();
                nextIndex = (int)parents[nextIndex];
            }
            Console.WriteLine(nextIndex);
            s = new State(explored[0]);
            s.Print();
            Console.WriteLine("------ DONE ------");
        }

        public bool IsFinished(int[] pattern)
        {
            return _board.SequenceEqual(pattern);
        }

        public long RecalculateHash()
        {
            _hash = 0;
            for (int i = 0; i < _board.Count; i++)
            {
                _hash += _board[i] * (1L << i);
            }
            return _hash;
        }
        public override bool Equals(object obj)
        {
            return ((State)obj).Hash == Hash;
        }

    }
}
