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


    void Start()
    {
        IEnumerator PegListCoroutine()
        {
            //yield on a new YieldInstruction that waits for 1 seconds.
            yield return new WaitForSeconds(1);

            _lab = GameObject.Find("CircuitLab").GetComponent<CircuitLab>();

            //_pegs = c_lab_component._listPegs;
            _pegsArray = _lab._allPegs;
        }
        StartCoroutine(PegListCoroutine());
        mainCamera = Camera.main;
        // CameraZDistance = mainCamera.WorldToScreenPoint(transform.position).z;
    }

    void OnMouseDrag()
    {
        moveToMouse();
    }

    void OnMouseDown()
    {
        _height = DEFAULT_HEIGHT + LIFT_DISTANCE;

        // calculate offest of mouse from object
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = _height;
        _mouseOffset = mainCamera.WorldToScreenPoint(gameObject.transform.position) - mousePoint;
        
        if (_thisComponent.isSnapped())
        {
            _thisComponent.disconnect();
        }
    }


    void OnMouseUp()
    {
        _height = DEFAULT_HEIGHT;
        moveToMouse();

        Debug.Log(_lab._allPegs.ToString());
        Debug.Log(_pegsArray.ToString());
        foreach(PegSnap peg in _pegsArray)
        {
            if(Vector3.Distance(gameObject.transform.position, peg.transform.position) < SNAP_DISTANCE && !peg.blocked){
                gameObject.transform.position = peg.transform.position;

                List<PegSnap> pegsToConnect = new List<PegSnap>() { peg, nextPegOver(peg) };
                _thisComponent.connect(pegsToConnect);
            }
        }

        _lab.constructCircuits();
    }

    private PegSnap nextPegOver(PegSnap given)
    {
        PegSnap otherPeg = null;
        if (_vertical)
        {
            int newRow = --given.row;
            if (newRow >= 0)
            {
                otherPeg = _lab._allPegs[newRow, given.col];
            }
        }
        else
        {
            int newCol = --given.col;
            if (newCol >= 0)
            {
                otherPeg = _lab._allPegs[given.row, newCol];
            }
        }

        return otherPeg;
    }

    private void moveToMouse()
    {
        // get new mouse position and add offset, then move object to new location
        Vector3 mousePos = new Vector3(Input.mousePosition.x + _mouseOffset.x, Input.mousePosition.y + _mouseOffset.y, _height);
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        transform.position = worldPos;
    }

}