using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct Point {
    public int x;
    public int y;

    public Point(int xVal, int yVal){
        x = xVal;
        y = yVal;
    }

    public override string ToString()
    {
        return "(" + x + "," + y + ")";
    }
}
public class PlacedComponent{
    public GameObject GameObject { get; set; }
   // public CircuitComponent Component { get; set; }
    public Point Start { get; set; }
    public Point End { get; set; }
}

public class Peg{
    public GameObject GameObject { get; set; }
    public List<PlacedComponent> Components { get; set; }
    public bool IsBlocked { get; set; }

    public Peg(){
        Components = new List<PlacedComponent>();
        IsBlocked = false;
    }
}

public class Board {
    public List<PlacedComponent> Components { get; set; }
    private Peg[,] Pegs { get; set; }
    private int Rows { get; set; }
    private int Cols { get; set; }
    public int Generation { get; set; }

    public Board(int rows, int cols)
    {
        Components = new List<PlacedComponent>();
        Generation = 0;

        Rows = rows;
        Cols = cols;

        Pegs = new Peg[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Pegs[i, j] = new Peg();
            }
        }
    }

    public void Reset()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                Pegs[i, j].GameObject.SetActive(true);
                Pegs[i, j].IsBlocked = false;
                Pegs[i, j].Components.Clear();
            }
        }

        Components.Clear();
    }

    public Peg GetPeg(Point coords)
    {
        if (coords.x < 0 || coords.x >= Cols || coords.y < 0 || coords.y >= Rows)
        {
            return null;
        }
        return Pegs[coords.y, coords.x];
    }

    public void SetPegGameObject(Point coords, GameObject gameObject)
    {
        if (coords.x < 0 || coords.x >= Cols || coords.y < 0 || coords.y >= Rows)
        {
            return;
        }
        Pegs[coords.y, coords.x].GameObject = gameObject;
    }
}
