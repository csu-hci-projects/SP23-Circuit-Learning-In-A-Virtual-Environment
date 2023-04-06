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
    public float voltage = 0f;

    public int index;

    public List<CircuitComponent> ownComponents = new List<CircuitComponent>();

    public void addComponent(CircuitComponent newItem)
    {
        ownComponents.Add(newItem);
        newItem.circuitIndex = index;
        newItem.ownCircuit = this;
    }
}

public class CircuitLab : MonoBehaviour
{
    public static bool isWorldFixed;
    public GameObject screenFixed = null;
    public DataManager dataManager;

    public GameLevel currentLevel = GameLevel.One;

    public Board board;
    [SerializeField] private GameObject pegTemplate = null;
    [SerializeField] private int size = 9;

    private CircuitComponent[] allComponents;
    private List<Circuit> allCircuits = new List<Circuit>();


    public bool maxCurrent;
    public List<System.Type> requirements;
    public float goalcurrent;

    void Start()
    {
        setLevel(currentLevel);

        screenFixed.SetActive(isWorldFixed);

        allComponents = FindObjectsOfType<CircuitComponent>();

        board = new Board(size, pegTemplate);

        foreach (CircuitComponent V in allComponents)
        {
            V.setScale(board.scaleAdjust);
        }

       dataManager.Save();
    }

    public void setLevel(GameLevel level)
    {
        switch (level)
        {
            case GameLevel.One:
                UIMainMenu.participantData.level01time = Time.time;

                requirements = new List<System.Type>() { typeof(Wire), typeof(Wire), typeof(Wire), typeof(Wire) };
                maxCurrent = false;
                goalcurrent = 0.0f;
                break;

        }
    }

    public bool checkRequirements()
    {

        Debug.Log("Check requirements");

        foreach (Circuit C in allCircuits)
        {
            if (maxCurrent)
            {
                if (C.current > goalcurrent) continue;
            }
            else if (C.current < goalcurrent) continue;

            List<CircuitComponent> copylist = new List<CircuitComponent>(C.ownComponents);

            foreach (System.Type T in requirements)
            {
                bool found = false;
                foreach (CircuitComponent singleComponent in C.ownComponents)
                {
                    if (singleComponent.GetType() == T)
                    {
                        Debug.Log("Found component type");
                        found = true;
                        break;
                    }
                }
                if (found) continue;
                else return false;
            }
        }

        return true;
    }

    public void constructCircuits()
    {
        //Debug.Log("ConstructCircuits");

        allCircuits = new List<Circuit>();
        int circuitIndex = 0;

        List<CircuitComponent> unvisited = new List<CircuitComponent>(allComponents);

        while (unvisited.Count != 0)
        {
            //Debug.Log("loop " + unvisited.Count);
            CircuitComponent thisItem = unvisited[0];
            unvisited.Remove(thisItem);

            Circuit thisCircuit = new Circuit { ownComponents = new List<CircuitComponent>(), index = circuitIndex };
            circuitIndex += 1;

            thisCircuit.addComponent(thisItem);
            

            allCircuits.Add(thisCircuit);

            Construct_DepthFirstSearch(thisItem, thisCircuit, unvisited);

        }

        foreach (Circuit thisCircuit in allCircuits)
        {
            
            foreach (CircuitComponent thisComponent in thisCircuit.ownComponents)
            {
                thisCircuit.resistance += thisComponent.resistance;
                if (thisComponent is Battery)
                {
                    thisCircuit.voltage += ((Battery)thisComponent).voltage;
                }
            }

            if (circuitIsLoop(thisCircuit))
            {
                thisCircuit.current = thisCircuit.voltage / thisCircuit.resistance;
            }

            if (thisCircuit.ownComponents.Count > 1)
            {
                Debug.Log("Circuit : size " + thisCircuit.ownComponents.Count + ", resistance " + thisCircuit.resistance +
                    ", voltage " + thisCircuit.voltage + ", current " + thisCircuit.current);
            }
        }

    }

    private bool circuitIsLoop(Circuit testCircuit)
    {
        if (testCircuit.ownComponents.Count < 4) //There's no way to have a loop with less than 4, so don't even check.
        {
            return false;
        }

        List<CircuitComponent> visited = new List<CircuitComponent>();

        return DFS_Loop(testCircuit.ownComponents[0], visited);
    }

    private bool DFS_Loop(CircuitComponent thisItem, List<CircuitComponent> visited)
    {
        if (visited.Contains(thisItem)) {
            return false;
        }

        visited.Add(thisItem);
        List<CircuitComponent> connectedComponents = thisItem.pegConnections();
        if (connectedComponents.Count != 2)
        {
            return false;
        }

        if (visited.Contains(connectedComponents[0]) && visited.Contains(connectedComponents[1]))
        {
            //A component was found that had 2 neighbors, both already visited - so the circuit is a loop!
            Debug.Log("Loop found!");
            return true;
            
        }

        else
        {
            return (DFS_Loop(connectedComponents[0], visited) || DFS_Loop(connectedComponents[1], visited));
        }

    }

    private void Construct_DepthFirstSearch(CircuitComponent thisItem, Circuit thisCircuit, List<CircuitComponent> unvisited)
    {
        //Recursively accesses all of the unvisited adjacent components to thisItem, adding them to the circuit and removing
        //them from unvisited
        
        foreach (CircuitComponent V in thisItem.pegConnections())
        {
            if (unvisited.Contains(V))
            {
                unvisited.Remove(V);
                thisCircuit.addComponent(V);
                Construct_DepthFirstSearch(V, thisCircuit, unvisited);
            }
        }

    }

    public float getCurrent(int givenIndex)
    {
        if (givenIndex < allCircuits.Count)
        {
            return allCircuits[givenIndex].current;
        }

        else
        {
            return 0.0f;
        }
    }

    public enum GameLevel
    {
        One,
        Two
    }
}