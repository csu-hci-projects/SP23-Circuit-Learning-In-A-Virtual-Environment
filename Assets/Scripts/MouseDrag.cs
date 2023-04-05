using System.Collections;

using System.Collections.Generic;

using UnityEngine;

public class MouseDrag : MonoBehaviour
{
    [SerializeField] private CircuitLab _lab;
    [SerializeField] private CircuitComponent _thisComponent;
    [SerializeField] private bool _vertical;

    private const float LIFT_DISTANCE = -0.45f;
    private const float DEFAULT_HEIGHT = 12.45f;
    private const float SNAP_DISTANCE = 0.5f;

    private Camera mainCamera;
    private Vector3 _mouseOffset;
    private float _height;

    void Start()
    {
        _lab = (CircuitLab)FindObjectOfType(typeof(CircuitLab));
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

        foreach(PegSnap peg in _lab.board.getAllPegs())
        {
            if(Vector3.Distance(gameObject.transform.position, peg.transform.position) < SNAP_DISTANCE && !peg.blocked){
                gameObject.transform.position = peg.transform.position;

                if (peg.blocked || nextPegOver(peg).blocked)
                {
                    Debug.Log("Can't connect to blocked peg");
                    break;
                }

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
            int newRow = given.row - 1;
            if (newRow >= 0)
            {
                otherPeg = _lab.board.getPeg(newRow, given.col);
            }
        }
        else
        {
            int newCol = given.col - 1;
            if (newCol >= 0)
            {
                otherPeg = _lab.board.getPeg(given.row, newCol);
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