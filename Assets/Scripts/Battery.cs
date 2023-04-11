using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CompleteUI;

public class Battery : CircuitComponent
{

    public float voltage = 5f;

    // Update is called once per frame
    protected override void Update()
    {
        foreach (CircuitComponent item in connectedViaPegs())
        {
            if (item.name == "WireEnd1" || item.name == "WireEnd2")
            {
                Debug.Log("Circuit Complete!");
                CompletionUI.Instance.Show();
            }
        }
    }
}
