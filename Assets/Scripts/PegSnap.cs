using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PegSnap : MonoBehaviour
{
    public bool blocked = false;
    public List<CircuitComponent> attachedComponents = new List<CircuitComponent>();


    public int row;
    public int col;

    public void connect(CircuitComponent connected)
    {
        attachedComponents.Add(connected);
        if (attachedComponents.Count >= 2)
        {
            blocked = true;
        }
    }

    public void disconnect(CircuitComponent connected)
    {
        attachedComponents.Remove(connected);
        if (attachedComponents.Count < 2)
        {
            blocked = false;
        }
    }
}
