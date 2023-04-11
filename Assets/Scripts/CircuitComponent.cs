using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LabelAlignment { Top, Bottom, Center };

public abstract class CircuitComponent : MonoBehaviour
{
    [SerializeField] protected CircuitLab _lab;

    [SerializeField] protected List<ComponentEnd> _ends;
    [SerializeField] protected List<PegSnap> _connectedPegs = new List<PegSnap>();

    //for some reason, wire in the item tray scales weird. DEFAULT_SCALE is quick fix because time crunch :)))
    private Vector3 DEFAULT_SCALE = new Vector3(5, 5, 5);
    public Direction direction;

    const double SIGNIFICANT_CURRENT = 0.0000001;
    const float LABEL_OFFSET = 0.022f;

    public float resistance = 0.01f;

    public int circuitIndex;
    public Circuit ownCircuit;

    protected virtual void Start()
    {
        _ends = new List<ComponentEnd>(gameObject.GetComponentsInChildren<ComponentEnd>());
        foreach (ComponentEnd ends in _ends)
        {
            ends.owner = this;
        }

        _lab = (CircuitLab)FindObjectOfType(typeof(CircuitLab));
    }

    protected abstract void Update();

    public void setScale(float scaleAdjust)
    {
        gameObject.transform.localScale = DEFAULT_SCALE * scaleAdjust;
    }

    public void connect(List<PegSnap> pegs)
    {
        //Debug.Log("Connecting to " + pegs.Count + " pegs");
        foreach (PegSnap peg in pegs)
        {
            Debug.Log("Peg " + peg.row + " " + peg.col);
            peg.attachedComponents.Add(this);
            if (peg.attachedComponents.Count >= 2)
            {
                peg.blocked = true;
            }
        }
        _connectedPegs = pegs;

        Debug.Log("Connected to " + connectedViaPegs().Count + " other components");
    }

    public void disconnect()
    {
        foreach(PegSnap peg in _connectedPegs)
        {
            peg.attachedComponents.Remove(this);
            peg.blocked = false;
        }
        _connectedPegs.Clear();
    }

    public bool isSnapped()
    {
        return !(_connectedPegs.Count == 0);
    }

    public List<CircuitComponent> connectedViaPegs()
    {
        List<CircuitComponent> connectedComponents =  new List<CircuitComponent>();
        foreach(PegSnap peg in _connectedPegs)
        {
            foreach(CircuitComponent component in peg.attachedComponents)
            {
                if(!component.Equals(this))
                connectedComponents.Add(component);
            }
        }
        return connectedComponents;
    }



    public enum Direction
    {
        Vertical,
        Horizontal
    }
}


