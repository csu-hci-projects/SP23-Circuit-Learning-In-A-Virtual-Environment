using System.Collections.Generic;

using UnityEngine;

public class MouseDrag : MonoBehaviour
{
    [SerializeField] private TrayItem _trayItem;
    [SerializeField] private CircuitLab _lab;
    [SerializeField] private CircuitComponent _thisComponent;

    private const float LIFT_DISTANCE = -0.45f;
    private const float DEFAULT_HEIGHT = 12.45f;
    private const float SNAP_DISTANCE = 0.5f;

    private Camera mainCamera;
    private Vector3 _mouseOffset;
    private float _height;

    public static int numComponentsCreated = 0;

    void Start()
    {
        _lab = (CircuitLab)FindObjectOfType(typeof(CircuitLab));
        _trayItem = gameObject.GetComponent<TrayItem>();
        mainCamera = Camera.main;
    }

    void OnMouseDrag()
    {
        if (_trayItem != null)
        {
            moveToMouse(_trayItem.dupedComponent);
        }
        else
        {
            moveToMouse(gameObject);
        }
    }

    void OnMouseDown()
    {
        _height = DEFAULT_HEIGHT + LIFT_DISTANCE;

        // calculate offest of mouse from object
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = _height;
        _mouseOffset = mainCamera.WorldToScreenPoint(gameObject.transform.position) - mousePoint;

        if (_trayItem != null)
        {
            _trayItem.duplicateComponent();
            _lab.addCircuitComponent(_trayItem.dupedComponent.GetComponent<CircuitComponent>());
        }
        else
        {

            if (_thisComponent.isSnapped())
            {
                _thisComponent.disconnect();
            }
        }
    }


    void OnMouseUp()
    {
        if (_trayItem != null)
        {
            _trayItem.dupedComponent.GetComponent<MouseDrag>().trySnapping();
            _trayItem.dupedComponent = null;
        }
        else
        {
            trySnapping();
        }

    }

    private PegSnap nextPegOver(PegSnap given)
    {
        PegSnap otherPeg = null;
        if (_thisComponent.direction==CircuitComponent.Direction.Vertical)
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

    private void moveToMouse(GameObject objectToMove)
    {
        // get new mouse position and add offset, then move object to new location
        Vector3 mousePos = new Vector3(Input.mousePosition.x + _mouseOffset.x, Input.mousePosition.y + _mouseOffset.y, _height);
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        objectToMove.transform.position = worldPos;
    }

    public void trySnapping()
    {
        _height = DEFAULT_HEIGHT;

        moveToMouse(gameObject);

        foreach (PegSnap peg in _lab.board.getAllPegs())
        {
            if (Vector3.Distance(transform.position, peg.transform.position) < SNAP_DISTANCE && !peg.blocked)
            {
                transform.position = peg.transform.position;

                if (peg.blocked)
                {
                    Debug.Log("Can't connect to blocked peg");
                    break;
                }

                List<PegSnap> pegsToConnect = new List<PegSnap>() { peg, nextPegOver(peg) };
                _thisComponent.connect(pegsToConnect);
                _lab.constructCircuits();

                return;
            }
        }

        Destroy(gameObject);
    }

}