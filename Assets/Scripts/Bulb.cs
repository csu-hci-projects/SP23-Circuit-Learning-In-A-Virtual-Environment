using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bulb : CircuitComponent
{
    [SerializeField] Light bulbLight;

    [SerializeField] Material unlit;
    [SerializeField] Material lit;
    [SerializeField] GameObject filament;

    protected override void Update() {
        /*
        if(voltage > 0, and this component is connected on both ends)
            bulbLight.enabled = true;
            bulbLight.intensity = (circuit voltage/circuit resistance) * some scaling coefficient;
        */

        if (touchingComponents.Count == 2)
        {
            bulbLight.intensity = ownCircuit.current;
            if (bulbLight.intensity > 0.01) { filament.GetComponent<MeshRenderer>().material = lit; }
            else  {
                filament.GetComponent<MeshRenderer>().material = unlit; }
           
        }
        else
        {
            filament.GetComponent<MeshRenderer>().material = unlit;
            bulbLight.intensity = 0;
        }

        /*
        //If the Bulb is on the pegboard, and both ends are connected, turn the light on
        if((startPeg != null && endPeg != null) && 
            (startPeg.attachedComponents.Count == 2 && endPeg.attachedComponents.Count == 2)){
            bulbLight.intensity = 1;
            bulbLight.enabled = true; 
        } else {
            bulbLight.intensity = 0;
            bulbLight.enabled = false;
        }    
        */
    }
}