using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board {
    private PegSnap[,] pegs;

    public Board(int rows, int cols)
    {
        pegs = new PegSnap[rows, cols];
    }
}
