using System.Collections.Generic;
using UnityEngine;

public class CircuitLab : MonoBehaviour
{
    public static bool isWorldFixed;
    public GameObject screenFixed = null;
    public DataManager dataManager;
    

    public GameLevel currentLevel = GameLevel.One;

    public Board board;
    public ItemTray tray;
    [SerializeField] private int size = 9;

    [SerializeField] private GameObject pegTemplate = null;
    [SerializeField] private GameObject batteryTemplate = null;
    [SerializeField] private GameObject bulbTemplate = null;
    [SerializeField] private GameObject wireTemplate = null;

    [SerializeField] private List<CircuitComponent> allComponents;
    private List<Circuit> allCircuits = new List<Circuit>();


    public bool maxCurrent;
    public List<System.Type> requirements;
    public float goalcurrent;

    void Start()
    {

        // string m_Path = Application.dataPath;

        // //Output the Game data path to the console
        // Debug.Log("dataPath : " + m_Path);
        setLevel(currentLevel);

        screenFixed.SetActive(isWorldFixed);

        List<GameObject> trayTemplates = new List<GameObject>() { batteryTemplate, bulbTemplate, wireTemplate };
        board = new Board(size, pegTemplate);
        tray = new ItemTray(trayTemplates, currentLevel);

        allComponents = new List<CircuitComponent>();
    }

    public void setLevel(GameLevel level)
    {
        switch (level)
        {
            case GameLevel.One:
                UIMainMenu.participantData.level01startTime = Time.time;
                requirements = new List<System.Type>() { typeof(Battery), typeof(Wire), typeof(Wire), typeof(Wire) };
                maxCurrent = false;
                goalcurrent = 0.0f;
                break;
            case GameLevel.Two:
                UIMainMenu.participantData.level02startTime = Time.time;
                requirements = new List<System.Type>() { typeof(Battery), typeof(Bulb)};
                maxCurrent = false;
                goalcurrent = 0.5f;
                break;
            case GameLevel.Three:
                UIMainMenu.participantData.level03startTime = Time.time;
                requirements = new List<System.Type>() { typeof(Battery), typeof(Bulb), typeof(Bulb) };
                maxCurrent = false;
                goalcurrent = 0.0f;
                break;
        }
    }

    public bool checkRequirements()
    {

        Debug.Log("Check requirements on " + allCircuits.Count + " circuits");

        foreach (Circuit C in allCircuits)
        {

            Debug.Log("Checking circuit " + C.index);

            if (maxCurrent)
            {
                if (C.current > goalcurrent)
                {
                    Debug.Log(C.index + ": Max current exceeded");
                    continue;
                }
            }
            else if (C.current < goalcurrent)
            {
                Debug.Log(C.index + ": Min current not met");
                continue;
            }

            if (!circuitIsLoop(C))
            {
                Debug.Log("Circuit is not a loop.");
                continue;
            }

            List<System.Type> componentTypes = new List<System.Type>();

            foreach (CircuitComponent copyType in C.ownComponents)
            {
                componentTypes.Add(copyType.GetType());
            }

            bool allcomponentsfound = true;
            foreach (System.Type T in requirements)
            {
                bool found = false;
                foreach (System.Type singleType in componentTypes)
                {
                    if (singleType == T)
                    {
                        Debug.Log(C.index + ": Found component type: " + T.Name);
                        found = true;
                        componentTypes.Remove(T);
                       
                        break; //Stop checking for this one requirement - break out of the components foreach loop
                    }
                }

                if (found) continue; //continue to next component requirement
                
                else
                {
                    Debug.Log(C.index + ": Did not find necessary component: " + T.Name);
                    allcomponentsfound = false;
                    break;
                    //No, it shouldn't return false, it should go on to check the next circuit.
                }
            }
            if (allcomponentsfound) 
            {
                return true; }
            else continue;
        }
        //Done checking all circuits

        return false;
    }

    public void addCircuitComponent(CircuitComponent component)
    {
        allComponents.Add(component);
    }

    public void removeCircuitComponent(CircuitComponent component)
    {
        allComponents.Remove(component);
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

        Debug.Log("Constructed " + allCircuits.Count + " circuits");

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
        List<CircuitComponent> connectedComponents = thisItem.connectedViaPegs();
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
        
        foreach (CircuitComponent V in thisItem.connectedViaPegs())
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
        Two,
        Three
    }
}

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