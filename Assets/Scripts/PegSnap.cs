using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PegSnap : MonoBehaviour
{
    public bool blocked = false;
    public List<CircuitComponent> attachedComponents = new List<CircuitComponent>();


    public int row;
    public int col;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void connect(CircuitComponent connected)
    {
        foreach (CircuitComponent toAdd in attachedComponents)
        {
            connected.touchingComponents.Add(toAdd);
        }

        attachedComponents.Add(connected);
        if (attachedComponents.Count >= 2)
        {
            blocked = true;
        }

        
    }

    public void disconnect(CircuitComponent connected)
    {
        foreach (CircuitComponent toAdd in attachedComponents)
        {
            if (toAdd == connected) { continue;  }
            connected.touchingComponents.Add(toAdd);
        }

        attachedComponents.Remove(connected);
        if (attachedComponents.Count < 2)
        {
            blocked = false;
        }
    }
}
