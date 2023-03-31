using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using SpiceSharp;
//using SpiceSharp.Components;
//using SpiceSharp.Simulations;
//using UnityEngine.XR.Interaction.Toolkit;

struct Circuit
{
    public float resistance;
    public float current;

    public List<Wire> ownWires;
}

public class CircuitLab : MonoBehaviour
{
    // public members found in Unity inspector
    public GameObject pegTemplate = null;
    public float pegInterval = 0.1f;
    public float pegHeight = 0.45f;
    public Vector3 pegScale;
    public static bool isWorldFixed;
    public GameObject screenFixed = null;


    //public List<PegSnap> _pegs = new List<PegSnap>();
    
    Board board;
    const int numRows = 9;
    const int numCols = 9;


    public PegSnap[,] _allPegs;
    public List<PegSnap> _listPegs;


    //List<IDynamic> dynamicComponents = new List<IDynamic>();
    int numActiveCircuits = 0;

    // Start is called before the first frame update
    void Start()
    {
        screenFixed.SetActive(isWorldFixed);
        _allPegs = new PegSnap[numRows, numCols];
        _listPegs = new List<PegSnap>();

        board = new Board(numRows,numCols);
        CreatePegs();
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreatePegs(){

        // Creates a matrix of pegs
        for(int i = 0; i < numRows; i++)
        {
            for(int j = 0; j < numCols; j++){
                _allPegs[i,j] = CreatePeg(i,j);
                _listPegs.Add(_allPegs[i, j]);
            }
        }
    }

    private PegSnap CreatePeg(int row, int col){

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
        var position = new Vector3(-(boardWidth / 2.0f) + ((col + 1) * pegInterval)+(boardWidth/2)-0.5f, pegHeight, -(boardHeight / 2.0f) + ((row + 1) * pegInterval)+(boardHeight/2)-0.5f);
        var rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        var peg = Instantiate(pegTemplate, position, rotation) as GameObject;
        peg.transform.parent = boardObject.transform;
        peg.transform.localPosition = position;
        peg.transform.localRotation = rotation;
        peg.transform.localScale = pegScale;

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

        // Run a new circuit simulation so that any circuits we've just broken get deactivated
        //SimulateCircuit();
        //REPLACE THIS WITH CONSTRUCTCIRCUITS() WHEN DONE
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

}
