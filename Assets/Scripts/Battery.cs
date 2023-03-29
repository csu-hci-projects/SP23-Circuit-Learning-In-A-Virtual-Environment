using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CompleteUI;

public class Battery : MonoBehaviour
{
    public bool isComplete;
    List <GameObject> touchedObjects = new List <GameObject> ();

    // Start is called before the first frame update
    void Start()
    {
        isComplete = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        foreach (GameObject item in touchedObjects)
        {
            if(item.GetComponentsInParent<Wire>()[0].positiveConnected && item.GetComponentsInParent<Wire>()[0].negativeConnected){
                isComplete = true;
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
    }
}
