using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    public bool positiveConnected;
    public bool negativeConnected;
    List <GameObject> touchedObjects = new List <GameObject> ();

    // Start is called before the first frame update
    void Start()
    {
        positiveConnected = false;
        negativeConnected = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        touchedObjects.Add(other.gameObject);

        if (other.gameObject.name == "PositiveEnd"){
            positiveConnected = true;
            // Debug.Log("positive connected: " + positiveConnected);
        }else if (other.gameObject.name == "NegativeEnd"){
            negativeConnected = true;
            // Debug.Log("negative connected: " + negativeConnected);
        }else if (other.gameObject.name == "WireEnd1" || other.gameObject.name == "WireEnd2"){
             Wire otherWire = other.gameObject.GetComponentsInParent<Wire>()[0];
             if (otherWire.positiveConnected == true){
                positiveConnected = true;
             }
             if (otherWire.negativeConnected == true){
                negativeConnected = true;
             }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        touchedObjects.Remove(other.gameObject);
        
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
    }
}
