using System.Collections;

using System.Collections.Generic;

using UnityEngine;


public class MouseDrag : MonoBehaviour

{

    private Vector3 mOffset;
    private float mZCoord;


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

    //public List<PegSnap> _pegs = new List<PegSnap>();

    public List<PegSnap> _pegs = GetComponentsInChildren<PegSnap>();

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