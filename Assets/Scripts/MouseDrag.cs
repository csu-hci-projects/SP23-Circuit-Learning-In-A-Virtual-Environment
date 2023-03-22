using System.Collections;

using System.Collections.Generic;

using UnityEngine;


public class MouseDrag : MonoBehaviour

{

    private Vector3 mOffset;
    private float mZCoord;

    public GameObject c_lab;

    public CircuitLab c_lab_component;

    public PegSnap[,] _pegsArray;
    public List<PegSnap> _pegs = new List<PegSnap>();


    void Start()
    {

        //This needs to copy the _listPegs attribute over from the 
        //CircuitLab script, but if it copies it right at Start(), 
        //that list will be empty. So I have to do it like this

        StartCoroutine(PegListCoroutine());

        //_pegsArray = c_lab_component._allPegs;

        //_pegs = c_lab_component._listPegs;
        //Creates list of pegs here
    }

    IEnumerator PegListCoroutine()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 1 seconds.
        yield return new WaitForSeconds(1);


        c_lab = GameObject.Find("CircuitLab");

        c_lab_component = c_lab.GetComponent<CircuitLab>();

        _pegs = c_lab_component._listPegs;

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    void OnMouseDown(){

        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        // Store offset = gameobject world pos - mouse world pos

        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }


    private Vector3 GetMouseAsWorldPoint(){

        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = mZCoord;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }


    void OnMouseDrag(){

        transform.position = GetMouseAsWorldPoint() + mOffset;

    }

    // public Vector3 targetPosition;
    // public float snapDistance = 0.1f;
    // public List<Transform> nodes = new List<Transform>();

    

    private PegSnap _peg;

    public PegSnap _chosenpeg;

    void OnMouseUp()
    {
        foreach(PegSnap _peg in _pegs){
            if(Vector3.Distance(gameObject.transform.position, _peg.transform.position) < 0.5){
                _chosenpeg = _peg;
                gameObject.transform.position = _peg.transform.position;
            }
        }

        // if(Vector3.Distance(transform.position, _peg.transform.position) < 0.5){
        //     transform.position = _peg.transform.position;
        // }


        // transform.position = GetMouseAsWorldPoint() + mOffset;
        // float smallestDistance = snapDistance;
        // foreach (Transform node in transform)
        // {
        //     Debug.Log("Node:"+ node.name);
            
        //     if (Vector3.Distance(node.position, targetPosition) < smallestDistance)
        //     {
                
        //         transform.position = node.position;
        //         smallestDistance = Vector3.Distance(node.position, targetPosition);
        //     } 
        //  }


    }

}