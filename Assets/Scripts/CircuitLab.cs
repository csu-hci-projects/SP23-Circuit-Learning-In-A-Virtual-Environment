using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using SpiceSharp;
//using SpiceSharp.Components;
//using SpiceSharp.Simulations;
//using UnityEngine.XR.Interaction.Toolkit;

public class Circuit
{
    public float resistance = 0f;
    public float current = 0f;

    public List<CircuitComponent> ownComponents = new List<CircuitComponent>();
}

public class CircuitLab : MonoBehaviour
{
    public static bool isWorldFixed;
    public GameObject screenFixed = null;
    public int levelNumber = 1;
    public DataManager dataManager;


    public Board board;
    [SerializeField] private GameObject pegTemplate = null;
    [SerializeField] private int size = 9;

    private CircuitComponent[] allComponents;
    private List<Circuit> allCircuits = new List<Circuit>();

    //List<IDynamic> dynamicComponents = new List<IDynamic>();
    public int numCircuits = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(levelNumber ==1){
            UIMainMenu.participantData.level01time = Time.time;
        }
        screenFixed.SetActive(isWorldFixed);

        allComponents = FindObjectsOfType<CircuitComponent>();

        board = new Board(size, pegTemplate);

        foreach (CircuitComponent V in allComponents)
        {
            V.setScale(board.scaleAdjust);
        }

       dataManager.Save();
    }

    public void constructCircuits()
    {
        Debug.Log("ConstructCircuits");

        numCircuits = 0;
        allCircuits = new List<Circuit>();

        List<CircuitComponent> unvisited = new List<CircuitComponent>(allComponents);

        while (unvisited.Count != 0)
        {
            //Debug.Log("loop " + unvisited.Count);
            CircuitComponent thisItem = unvisited[0];
            unvisited.Remove(thisItem);

            Circuit thisCircuit = new Circuit { ownComponents = new List<CircuitComponent>() };
            numCircuits += 1;

            thisCircuit.ownComponents.Add(thisItem);

            allCircuits.Add(thisCircuit);

            DepthFirstSearch(thisItem, thisCircuit, unvisited);

            Debug.Log("Circuit created of size " + thisCircuit.ownComponents.Count);
        }

    }

    private void DepthFirstSearch(CircuitComponent thisItem, Circuit thisCircuit, List<CircuitComponent> unvisited)
    {
        //Recursively accesses all of the unvisited adjacent components to thisItem, adding them to the circuit and removing
        //them from unvisited
        
        foreach (CircuitComponent V in thisItem.pegConnections())
        {
            if (unvisited.Contains(V))
            {
                unvisited.Remove(V);
                thisCircuit.ownComponents.Add(V);
                DepthFirstSearch(V, thisCircuit, unvisited);
            }
        }

    }

}
