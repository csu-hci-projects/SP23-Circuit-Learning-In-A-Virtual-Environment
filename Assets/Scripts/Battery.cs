using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CompleteUI;

public class Battery : MonoBehaviour
{
    public bool isComplete;
    public List <GameObject> touchedObjects = new List <GameObject> ();
    public List<Wire> touchedWires = new List<Wire>();

    // Start is called before the first frame update
    void Start()
    {
        isComplete = false;
    }

    // Update is called once per frame
    void Update()
    {
        isComplete = false;
        foreach (GameObject item in touchedObjects)
        {
            if (item.name == "WireEnd1" || item.name == "WireEnd2")
            {
                if (item.GetComponentsInParent<Wire>()[0].positiveConnected && item.GetComponentsInParent<Wire>()[0].negativeConnected)
                {
                    isComplete = true;
                }
            }
        }

        if(isComplete == true){
            Debug.Log("Circuit Complete!");
            CompletionUI.Instance.Show();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        touchedObjects.Add(other.gameObject);

        if (other.gameObject.name == "WireEnd1" || other.gameObject.name == "WireEnd2")
        {
            touchedWires.Add(other.gameObject.GetComponent<WireEnd>().ownerWire);
            other.gameObject.GetComponent<WireEnd>().ownerWire.connectedBattery = this;
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
            //other.gameObject.GetComponent<WireEnd>().ownerWire.touchedWires.Remove(this);
        }
    }
}
