using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LabelAlignment { Top, Bottom, Center };

public abstract class CircuitComponent : MonoBehaviour
{
    [SerializeField]
    private CircuitLab Lab;

    [SerializeField]
    protected List<ComponentEnd> ends;

    [SerializeField] protected List<PegSnap> connectedPegs = new List<PegSnap>();

    const double SignificantCurrent = 0.0000001;
    const float LabelOffset = 0.022f;

    public float transformAdjust = 1f;

    protected virtual void Start()
    {
        ends = new List<ComponentEnd>(gameObject.GetComponentsInChildren<ComponentEnd>());
        foreach (ComponentEnd addedend in ends)
        {
            addedend.owner = this;
        }

        Lab = (CircuitLab)FindObjectOfType(typeof(CircuitLab));
    }

    protected abstract void Update();

    public void setScale()
    {
        transformAdjust = (float)(Lab.scaleAdjust);
        gameObject.transform.localScale = gameObject.transform.localScale * transformAdjust;
    }

    public void connect(List<PegSnap> pegs)
    {
        foreach (PegSnap peg in pegs)
        {
            Debug.Log("a");
            peg.attachedComponents.Add(this);
        }
        connectedPegs = pegs;

    }
    public void disconnect()
    {
        foreach(PegSnap peg in connectedPegs)
        {
            peg.attachedComponents.Remove(this);
        }
        connectedPegs.Clear();
    }

    public bool isSnapped()
    {
        return !(connectedPegs.Count == 0);
    }

    public List<CircuitComponent> pegConnections()
    {
        List<CircuitComponent> connectedComponents =  new List<CircuitComponent>();
        foreach(PegSnap peg in connectedPegs)
        {
            foreach(CircuitComponent component in peg.attachedComponents)
            {
                if(!component.Equals(this))
                connectedComponents.Add(component);
            }
        }

        return connectedComponents;
    }
}


