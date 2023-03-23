using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    public bool positiveConnected;
    public bool negativeConnected;
    public List<GameObject> touchedObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        positiveConnected = false;
        negativeConnected = false;
    }

    // Update is called once per frame
    void Update()
    {
        positiveConnected = false;
        negativeConnected = false;

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
                Wire otherWire = item.gameObject.GetComponentsInParent<Wire>()[0];
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
        Debug.Log("Object added to colliders list");

        /*
        if (other.gameObject.name == "PositiveEnd"){
            positiveConnected = true;
            Debug.Log("Connected to positive end of battery");
        }else if (other.gameObject.name == "NegativeEnd"){
            negativeConnected = true;
            Debug.Log("Connected to negative end of battery");
        }
        else if (other.gameObject.name == "WireEnd1" || other.gameObject.name == "WireEnd2"){
             Wire otherWire = other.gameObject.GetComponentsInParent<Wire>()[0];
             if (otherWire.positiveConnected == true){
                positiveConnected = true;
                Debug.Log("Connected to positive end of wire");
            }
             if (otherWire.negativeConnected == true){

                Debug.Log("Connected to negative end of wire");
                negativeConnected = true;
             }
        }
        */
    }

    private void OnTriggerExit(Collider other)
    {
        touchedObjects.Remove(other.gameObject);

        Debug.Log("Object removed from colliders list");

        /*
        if (other.gameObject.name == "PositiveEnd"){
            positiveConnected = false;
            // Debug.Log("positive connected: " + positiveConnected);
        }else if (other.gameObject.name == "NegativeEnd"){
            negativeConnected = false;
            // Debug.Log("negative connected: " + negativeConnected);
        }else if (other.gameObject.name == "WireEnd1" || other.gameObject.name == "WireEnd2"){
             Wire otherWire = other.gameObject.GetComponentsInParent<Wire>()[0];
             if (otherWire.positiveConnected == true){
                positiveConnected = false;
             }
             if (otherWire.negativeConnected == true){
                negativeConnected = false;
             }
        }
        */
    }
}
