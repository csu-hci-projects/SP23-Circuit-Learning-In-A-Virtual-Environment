using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Simulations;
//using UnityEngine.XR.Interaction.Toolkit;

public class CircuitLab : MonoBehaviour
{
    // public members found in Unity inspector
    public GameObject pegTemplate = null;
    public float pegInterval = 0.1f;
    public float pegHeight = 0.45f;
    public Vector3 pegScale;
    public bool isWorldFixed;

    Board board;
    const int numRows = 9;
    const int numCols = 9;
    // Start is called before the first frame update
    void Start()
    {
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
                CreatePeg(i,j);
            }
        }
    }

    private void CreatePeg(int row, int col){

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

        Point coords = new Point(col, row);
        board.SetPegGameObject(coords, peg);
    }
}
