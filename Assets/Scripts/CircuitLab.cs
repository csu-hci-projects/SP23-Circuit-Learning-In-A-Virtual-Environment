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
    // public members found in Unity inspector
    public GameObject pegTemplate = null;
    public float pegInterval;
    public float pegHeight = 0.45f;
    public Vector3 pegScale;
    public bool isWorldFixed;


    Board board;
    int numRows;
    int numCols;

    public int numSquare;
    public float scaleAdjust = 1f;


    public PegSnap[,] _allPegs;
    public List<PegSnap> _listPegs;

    public CircuitComponent[] allComponents;

    protected List<Circuit> allCircuits = new List<Circuit>();

    //List<IDynamic> dynamicComponents = new List<IDynamic>();
    public int numCircuits = 0;

    // Start is called before the first frame update
    void Start()
    {
        numRows = numCols = numSquare;

        pegInterval = 1.0f / (float)(numSquare + 1);
        scaleAdjust = 9.0f / (float)numSquare;

        _allPegs = new PegSnap[numSquare, numSquare];
        _listPegs = new List<PegSnap>();

        allComponents = FindObjectsOfType<CircuitComponent>();

        board = new Board(numSquare, numSquare);
        CreatePegs();

        foreach (CircuitComponent V in allComponents)
        {
            V.setScale();
        }
    }



    // Update is called once per frame
    void Update()
    {

    }

    public void CreatePegs() {

        // Creates a matrix of pegs
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++) {
                _allPegs[i, j] = CreatePeg(i, j);
                _listPegs.Add(_allPegs[i, j]);
            }
        }
    }

    private PegSnap CreatePeg(int row, int col) {

        // create peg name
        string name = "Peg_" + row.ToString() + "_" + col.ToString();

        // find bounds of breadboard
        var boardObject = GameObject.Find("Breadboard").gameObject;
        var mesh = boardObject.GetComponent<MeshFilter>().mesh;
        var size = mesh.bounds.size;
        var boardWidth = size.x * boardObject.transform.localScale.x;
        var boardHeight = size.z * boardObject.transform.localScale.z;
        var boardXPos = boardObject.transform.position.x;
        var boardZPos = boardObject.transform.position.z;

        // Create a new peg
        var position = new Vector3(-(boardWidth / 2.0f) + ((col + 1) * pegInterval) + (boardWidth / 2) - 0.5f, pegHeight, -(boardHeight / 2.0f) + ((row + 1) * pegInterval) + (boardHeight / 2) - 0.5f);
        var rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        var peg = Instantiate(pegTemplate, position, rotation) as GameObject;
        peg.transform.parent = boardObject.transform;
        peg.transform.localPosition = position;
        peg.transform.localRotation = rotation;
        peg.transform.localScale = pegScale * scaleAdjust;

        peg.name = name;



        Point coords = new Point(row, col);
        board.SetPegGameObject(coords, peg);

        PegSnap pegComponent = peg.AddComponent<PegSnap>();

        pegComponent.row = row;
        pegComponent.col = col;

        return pegComponent;
    }

    public void RemoveComponent(GameObject component, Point start)
    {
        Peg pegA = board.GetPeg(start);
        if (pegA != null)
        {
            PlacedComponent found = pegA.Components.Find(x => x.GameObject == component);
            if (found != null)
            {
                Peg pegB = board.GetPeg(found.End);
                if (pegB != null)
                {
                    // Remove it from each of the pegs it's attached to
                    if (!pegA.Components.Remove(found))
                        Debug.Log("Failed to remove component from Peg A!");
                    if (!pegB.Components.Remove(found))
                        Debug.Log("Failed to remove component from Peg B!");

                    // Remove it from the master list as well
                    board.Components.Remove(found);

                    // Unblock/unhide intermediate pegs
                    BlockPegs(found.Start, found.End, false);
                }
            }
        }

        // Deactivate the component
        var script = component.GetComponent<CircuitComponent>();
        if (script != null)
        {
            script.SetActive(false, false);
        }

        constructCircuits();
    }

    public void BlockPegs(Point start, Point end, bool block)
    {
        // Hide all pegs between start and end
        List<Point> points = new List<Point>();
        if (start.x != end.x)
        {
            int xStart = (start.x < end.x ? start.x : end.x);
            int xEnd = (start.x < end.x ? end.x : start.x);
            for (int x = xStart + 1; x < xEnd; x++)
            {
                Point coords = new Point(x, start.y);
                board.BlockPeg(coords, block);
            }
        }
        if (start.y != end.y)
        {
            int yStart = (start.y < end.y ? start.y : end.y);
            int yEnd = (start.y < end.y ? end.y : start.y);
            for (int y = yStart + 1; y < yEnd; y++)
            {
                Point coords = new Point(start.x, y);
                board.BlockPeg(coords, block);
            }
        }
    }

    public void constructCircuits()
    {
        Debug.Log("ConstructCircuits");

        numCircuits = 0;
        allCircuits = new List<Circuit>();
        int circuitIndex = 0;

        List<CircuitComponent> unvisited = new List<CircuitComponent>(allComponents);

        while (unvisited.Count != 0)
        {
            //Debug.Log("loop " + unvisited.Count);
            CircuitComponent thisItem = unvisited[0];
            unvisited.Remove(thisItem);

            Circuit thisCircuit = new Circuit { ownComponents = new List<CircuitComponent>(), index = circuitIndex };
            numCircuits += 1;
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

            Debug.Log("Circuit : size " + thisCircuit.ownComponents.Count + ", resistance " + thisCircuit.resistance +
                ", voltage " + thisCircuit.voltage + ", current " + thisCircuit.current);
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

        if (thisItem.touchingComponents.Count != 2)
        {
            return false;
        }

        if (visited.Contains(thisItem.touchingComponents[0]) && visited.Contains(thisItem.touchingComponents[1]))
        {
            //A component was found that had 2 neighbors, both already visited - so the circuit is a loop!
            Debug.Log("Loop found!");
            return true;
            
        }

        else
        {
            return (DFS_Loop(thisItem.touchingComponents[0], visited) || DFS_Loop(thisItem.touchingComponents[1], visited));
        }

    }

    private void Construct_DepthFirstSearch(CircuitComponent thisItem, Circuit thisCircuit, List<CircuitComponent> unvisited)
    {
        //Recursively accesses all of the unvisited adjacent components to thisItem, adding them to the circuit and removing
        //them from unvisited
        
        foreach (CircuitComponent V in thisItem.touchingComponents)
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
        if (givenIndex < numCircuits)
        {
            return allCircuits[givenIndex].current;
        }

        else
        {
            return 0.0f;
        }
    }
}
