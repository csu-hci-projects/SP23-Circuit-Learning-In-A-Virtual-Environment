using System.Collections;

using System.Collections.Generic;

using UnityEngine;


public class MouseDrag : MonoBehaviour

{

    private Vector3 mOffset;
    private float mZCoord;

    public CircuitLab c_lab;
    public Wire this_wire;

    public bool vertical;
    public bool snapped = false;

    public PegSnap[,] _pegsArray;
    //public List<PegSnap> _pegs = new List<PegSnap>();


    void Start()
    {

        //This needs to copy the _listPegs attribute over from the 
        //CircuitLab script, but if it copies it right at Start(), 
        //that list will be empty. So I have to do it like this.
        //I am sorry.

        StartCoroutine(PegListCoroutine());
    }

    IEnumerator PegListCoroutine()
    {
        //Print the time of when the function is first called.
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 1 seconds.
        yield return new WaitForSeconds(1);


        c_lab = GameObject.Find("CircuitLab").GetComponent<CircuitLab>();

        //_pegs = c_lab_component._listPegs;
        _pegsArray = c_lab._allPegs;

        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    void OnMouseDown(){

        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        // Store offset = gameobject world pos - mouse world pos

        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();

        if (snapped)
        {
            _chosenpeg.disconnect(this_wire);

            if (vertical)
            {
                int newRow = _chosenpeg.row - 1;
                if (newRow >= 0)
                {
                    PegSnap otherPeg = c_lab._allPegs[newRow, _chosenpeg.col];
                    otherPeg.disconnect(this_wire);
                }
            }
            else
            {
                int newCol = _chosenpeg.col - 1;
                if (newCol >= 0)
                {
                    PegSnap otherPeg = c_lab._allPegs[_chosenpeg.row, newCol];
                    otherPeg.disconnect(this_wire);
                }
            }

        }

        snapped = false;
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
        foreach(PegSnap _peg in _pegsArray)
        {
            if(Vector3.Distance(gameObject.transform.position, _peg.transform.position) < 0.5 && !_peg.blocked){
                _chosenpeg = _peg;
                gameObject.transform.position = _peg.transform.position;
                _peg.connect(this_wire);

                //The other peg will be 1 down if the component is vertical, 1 over if the component is horizontal.
                if (vertical)
                {
                    int newRow = _peg.row - 1;
                    if(newRow >= 0)
                    {
                        PegSnap otherPeg = c_lab._allPegs[newRow, _peg.col];
                        otherPeg.connect(this_wire);
                    }
                }
                else
                {
                    int newCol = _peg.col - 1;
                    if (newCol >= 0)
                    {
                        PegSnap otherPeg = c_lab._allPegs[_peg.row, newCol];
                        otherPeg.connect(this_wire);
                    }
                }
                snapped = true;
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