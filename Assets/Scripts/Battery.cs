using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CompleteUI;

public class Battery : CircuitComponent
{
    // Update is called once per frame
    protected override void Update()
    {
        foreach (CircuitComponent item in touchingComponents)
        {
            if (item.name == "WireEnd1" || item.name == "WireEnd2")
            {
                Debug.Log("Circuit Complete!");
                CompletionUI.Instance.Show();
            }
        }
    }
}
