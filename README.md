# PegSolitaireSolver
Solves any 7x7 peg solitaire board.
The playable field mask can be modified in State.playableField

Example games and ending patterns work up to game7 with their respective endPattern(s).

The maximum number of states that can be discovered is 2 146 233 066, after this index arrays throw an exception.
This is enough to cover 16Gb of game states stored in long (64 bits) using each bit to represent an empty or full peg.

Possible memory optimizations include replacing the BFS with a bidirectional version to cut memory costs.

Can use up to 4 cores - expands 4 nodes in parallel but waits for their expansion to end until starting a new iteration because of duplicate checking and saving which is performed after each expansion.
