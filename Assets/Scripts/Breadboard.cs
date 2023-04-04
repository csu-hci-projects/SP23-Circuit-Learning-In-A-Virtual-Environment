using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    private GameObject pegTemplate;
    [SerializeField] private float pegInterval;
    [SerializeField] private float pegHeight = 0.45f;
    [SerializeField] Vector3 pegScale = new Vector3(.02f, .2f, .02f);
    public float scaleAdjust { get; set; }

    private PegSnap[,] pegs;

    public Board(int size, GameObject pegTemplate)
    {
        this.pegTemplate = pegTemplate;
        pegInterval = 1.0f / (size + 1);
        scaleAdjust = 9.0f / size;
        pegs = new PegSnap[size, size];

        CreatePegs();
    }
    public void CreatePegs()
    {

        // Creates a matrix of pegs
        for (int i = 0; i < pegs.GetLength(0); i++)
        {
            for (int j = 0; j < pegs.GetLength(1); j++)
            {
                pegs[i, j] = CreatePeg(i, j);
            }
        }
    }
    private PegSnap CreatePeg(int row, int col)
    {
        // find bounds of breadboard
        GameObject boardObject = GameObject.Find("Breadboard").gameObject;
        Mesh mesh = boardObject.GetComponent<MeshFilter>().mesh;
        Vector3 size = mesh.bounds.size;
        float boardWidth = size.x * boardObject.transform.localScale.x;
        float boardHeight = size.z * boardObject.transform.localScale.z;

        // Create a new peg
        Vector3 pegPosition = new Vector3(-(boardWidth / 2.0f) + ((col + 1) * pegInterval) + (boardWidth / 2) - 0.5f, pegHeight, -(boardHeight / 2.0f) + ((row + 1) * pegInterval) + (boardHeight / 2) - 0.5f);
        Quaternion pegOrientation = Quaternion.Euler(new Vector3(0, 0, 0));

        GameObject peg = GameObject.Instantiate(pegTemplate, pegPosition, pegOrientation) as GameObject;
        string name = "Peg_" + row.ToString() + "_" + col.ToString();
        peg.name = name;
        peg.transform.parent = boardObject.transform;
        peg.transform.localPosition = pegPosition;
        peg.transform.localRotation = pegOrientation;
        peg.transform.localScale = pegScale * scaleAdjust;

        PegSnap pegComponent = peg.AddComponent<PegSnap>();
        pegComponent.init(row, col);

        return pegComponent;
    }

    public PegSnap getPeg(int row, int col)
    {
        return pegs[row, col];
    }

    public PegSnap[,] getAllPegs()
    {
        return pegs;
    }
}
