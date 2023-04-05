using System.Collections;

using System.Collections.Generic;

using UnityEngine;

public class MouseDrag : MonoBehaviour
{
    private Camera mainCamera;
    private float ZPlane;
    private float ZLiftDistance = -0.45f; // change to 0 to disable lift
    private float ZBase = 12.45f;
    private Vector3 mouseOffset;

    public CircuitLab c_lab;
    public CircuitComponent this_component;

    public bool vertical;
    public bool snapped = false;
    private float snapDistance = 0.5f;

    public PegSnap[,] _pegsArray;

    private PegSnap _peg;
    public PegSnap _chosenpeg;


    private PegSnap nextPegOver(PegSnap given)
    {
        PegSnap otherPeg = null;
        if (vertical)
        {
            int newRow = given.row - 1;
            if (newRow >= 0)
            {
                 otherPeg = c_lab._allPegs[newRow, given.col];
            }
        }
        else
        {
            int newCol = given.col - 1;
            if (newCol >= 0)
            {
                 otherPeg = c_lab._allPegs[given.row, newCol];
            }
        }

        return otherPeg;
    }


    void Start()
    {
        //This needs to copy the _listPegs attribute over from the 
        //CircuitLab script, but if it copies it right at Start(), 
        //that list will be empty. So I have to do it like this.
        //I am sorry.
        StartCoroutine(PegListCoroutine());

        mainCamera = Camera.main;
        // CameraZDistance = mainCamera.WorldToScreenPoint(transform.position).z;
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

        snapDistance = snapDistance * c_lab.scaleAdjust;

        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    void OnMouseDrag(){
        // get new mouse position and add offset, then move object to new location
        Vector3 ScreenPosition = new Vector3(Input.mousePosition.x + mouseOffset.x, Input.mousePosition.y + mouseOffset.y, ZPlane);
        Vector3 NewWorldPosition = mainCamera.ScreenToWorldPoint(ScreenPosition);
        transform.position = NewWorldPosition;

    }

    void OnMouseDown(){
        // set z plane to be above the board on click
        ZPlane = ZBase + ZLiftDistance;

        // calculate offest of mouse from object
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = ZPlane;
        mouseOffset = mainCamera.WorldToScreenPoint(gameObject.transform.position) - mousePoint;
        
        if (snapped){
            _chosenpeg.disconnect(this_component);

            nextPegOver(_chosenpeg).disconnect(this_component);
        }

        snapped = false;
    }


    void OnMouseUp(){
        // set z plane to be on the board on un-click
        ZPlane = ZBase;

        // get final mouse position and add offset, then move object to final location
        Vector3 ScreenPosition = new Vector3(Input.mousePosition.x + mouseOffset.x, Input.mousePosition.y + mouseOffset.y, ZPlane);
        Vector3 NewWorldPosition = mainCamera.ScreenToWorldPoint(ScreenPosition);
        transform.position = NewWorldPosition;

        foreach(PegSnap _peg in _pegsArray){

            if(Vector3.Distance(gameObject.transform.position, _peg.transform.position) < snapDistance){

                if (_peg.blocked || nextPegOver(_peg).blocked)
                {
                    Debug.Log("Can't connect to blocked peg");
                    break;
                }

                _chosenpeg = _peg;
                gameObject.transform.position = _peg.transform.position;
                _peg.connect(this_component);
                this_component.startPeg = _peg;

                nextPegOver(_peg).connect(this_component);
                this_component.endPeg = nextPegOver(_peg);

                snapped = true;

                break;
            }
        }

        c_lab.constructCircuits();
    }

}