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

    //[SerializeField]
    public List<CircuitComponent> touchingComponents;

    const double SignificantCurrent = 0.0000001;
    const float LabelOffset = 0.022f;

    public float transformAdjust = 1f;

    protected virtual void Start()
    {
        touchingComponents = new List<CircuitComponent>();

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


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("End") && !touchingComponents.Contains(other.gameObject.GetComponent<ComponentEnd>().owner))
        {
            touchingComponents.Add(other.gameObject.GetComponent<ComponentEnd>().owner);
            other.gameObject.GetComponent<ComponentEnd>().owner.touchingComponents.Add(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains("End") && touchingComponents.Contains(other.gameObject.GetComponent<ComponentEnd>().owner))
        {
            touchingComponents.Remove(other.gameObject.GetComponent<ComponentEnd>().owner);
            other.gameObject.GetComponent<ComponentEnd>().owner.touchingComponents.Remove(this);
        }
    }

    {

    }

    {
        {
        }

    }
}


