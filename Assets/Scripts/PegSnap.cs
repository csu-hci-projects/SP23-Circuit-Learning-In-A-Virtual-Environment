using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PegSnap : MonoBehaviour
{
    public int row;
    public int col;

    public List<CircuitComponent> attachedComponents = new List<CircuitComponent>();
    public bool blocked = false;

    public void init(int row, int col)
    {
        this.row = row;
        this.col = col;
    }
}
