using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    public bool positiveConnected;
    public bool negativeConnected;

    public List<WireEnd> ends = new List<WireEnd>();

    public List<GameObject> touchedObjects = new List<GameObject>();

    public List<Wire> touchedWires = new List<Wire>();

    public Battery connectedBattery;
    bool batterypositive = false;
    

    // Start is called before the first frame update
    void Start()
    {
        positiveConnected = false;
        negativeConnected = false;

        ends = new List<WireEnd>(gameObject.GetComponentsInChildren<WireEnd>());
        touchedWires = new List<Wire>();

        foreach (WireEnd addedend in ends)
        {
            addedend.ownerWire = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        positiveConnected = false;
        negativeConnected = false;

        if (connectedBattery != null)
        {
            if (batterypositive)
            {
                positiveConnected = true;
            }
            else
            {
                negativeConnected = true;
            }
        }

        foreach (GameObject item in touchedObjects)
        {
            if (item.name == "PositiveEnd")
            {
                positiveConnected = true;
            }

            else if (item.name == "NegativeEnd")
            {
                negativeConnected = true;
            }

            else if (item.name == "WireEnd1" || item.name == "WireEnd2")
            {
                Wire otherWire = item.gameObject.GetComponent<WireEnd>().ownerWire;
                
                if (otherWire.positiveConnected == true)
                {
                    positiveConnected = true;
                    //Debug.Log("Connected to positive end of wire");
                }
                if (otherWire.negativeConnected == true)
                {

                    //Debug.Log("Connected to negative end of wire");
                    negativeConnected = true;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        touchedObjects.Add(other.gameObject);

        if (other.gameObject.name == "WireEnd1" || other.gameObject.name == "WireEnd2")
        {
            touchedWires.Add(other.gameObject.GetComponent<WireEnd>().ownerWire);
            other.gameObject.GetComponent<WireEnd>().ownerWire.touchedWires.Add(this);
        }

        else if (other.gameObject.name == "PositiveEnd" || other.gameObject.name == "NegativeEnd")
        {
            connectedBattery = other.gameObject.GetComponent<WireEnd>().ownerBattery;
            other.gameObject.GetComponent<WireEnd>().ownerBattery.touchedWires.Add(this);

            if (other.gameObject.name == "PositiveEnd")
            {
                batterypositive = true;
            }
        }


        Debug.Log("Object added to colliders list");
    }

    private void OnTriggerExit(Collider other)
    {
        touchedObjects.Remove(other.gameObject);

        Debug.Log("Object removed from colliders list");

        if (other.gameObject.name == "WireEnd1" || other.gameObject.name == "WireEnd2")
        {
            touchedWires.Remove(other.gameObject.GetComponent<WireEnd>().ownerWire);
            other.gameObject.GetComponent<WireEnd>().ownerWire.touchedWires.Remove(this);
        }
    }
}
