using UnityEngine;

public class TrayItem : MonoBehaviour
{
    public GameObject dupedComponent;
    public static int numComponentsDuped = 0;

    public void duplicateComponent()
    {
        dupedComponent = GameObject.Instantiate(gameObject, gameObject.transform.localPosition, gameObject.transform.localRotation);
        dupedComponent.transform.localPosition = gameObject.transform.position;
        dupedComponent.transform.localRotation = gameObject.transform.rotation;

        string componentName = dupedComponent.name.Split("_")[0].Split("(")[0];
        dupedComponent.name = componentName + "_" + dupedComponent.GetComponent<CircuitComponent>().direction + " (" + numComponentsDuped++ + ")";
        dupedComponent.GetComponent<CircuitComponent>().setScale(Board.scaleAdjust);

        Component.Destroy(dupedComponent.GetComponent<TrayItem>());

    }
}
