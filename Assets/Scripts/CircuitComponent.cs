using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LabelAlignment { Top, Bottom, Center };

public abstract class CircuitComponent : MonoBehaviour
{
    [SerializeField]
    private CircuitLab Lab;

    [SerializeField]
    protected List<ComponentEnd> ends;

    [SerializeField]
    protected List<CircuitComponent> touchingComponents;

    public bool IsPlaced { get; protected set; }
    public bool IsHeld { get; protected set; }

    protected Point StartingPeg { get; set; }
    protected Direction Direction { get; set; }
    protected bool IsActive { get; set; }
    protected bool IsForward { get; set; }

    const double SignificantCurrent = 0.0000001;
    const float LabelOffset = 0.022f;

    protected CircuitComponent()
    {
        IsPlaced = false;
        IsHeld = false;
        IsActive = false;
        IsForward = true;
    }

    protected virtual void Start()
    {
        touchingComponents = new List<CircuitComponent>();

        ends = new List<ComponentEnd>(gameObject.GetComponentsInChildren<ComponentEnd>());
        foreach (ComponentEnd addedend in ends)
        {
            addedend.owner = this;
        }
    }

    protected abstract void Update();

    public void Place(Point start, Direction dir)
    {
        IsPlaced = true;
        StartingPeg = start;
        Direction = dir;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with " + other.gameObject.name + " !");
        if (other.gameObject.name.Contains("End") && !touchingComponents.Contains(other.gameObject.GetComponent<ComponentEnd>().owner))
        {
            touchingComponents.Add(other.gameObject.GetComponent<ComponentEnd>().owner);
            other.gameObject.GetComponent<ComponentEnd>().owner.touchingComponents.Add(this);

            Debug.Log("Object added to colliders list");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains("End") && touchingComponents.Contains(other.gameObject.GetComponent<ComponentEnd>().owner))
        {
            touchingComponents.Remove(other.gameObject.GetComponent<ComponentEnd>().owner);
            other.gameObject.GetComponent<ComponentEnd>().owner.touchingComponents.Remove(this);

            Debug.Log("Object removed from colliders list");
        }
    }

public virtual void SetActive(bool isActive, bool isForward)
    {
        IsActive = isActive;
        IsForward = isForward;
    }

    // Reset any component-specific state that might need resetting
    protected virtual void Reset()
    {
    }

    public virtual void SelectEntered()
    {
        IsHeld = true;

        // Enable box and sphere colliders so this piece can be placed somewhere else on the board.
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<SphereCollider>().enabled = true;

        if (IsPlaced)
        {
            Lab.RemoveComponent(this.gameObject, StartingPeg);

            IsPlaced = false;
        }

        // Reinitialize component state
        Reset();
    }

    public virtual void SelectExited()
    {
        IsHeld = false;

        // Make sure gravity is enabled any time we release the object
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
    }

    protected IEnumerator PlaySound(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);

        source.Stop();
        source.Play();
    }

    protected void RotateLabel(GameObject label, LabelAlignment alignment)
    {
        var rotation = label.transform.localEulerAngles;
        var position = label.transform.localPosition;

        switch (Direction)
        {
            case Direction.North:
            case Direction.East:
                rotation.z = -90f;
                position.x = alignment switch
                {
                    LabelAlignment.Top => -LabelOffset,
                    LabelAlignment.Bottom => LabelOffset,
                    _ => 0
                };
                break;
            case Direction.South:
            case Direction.West:
                rotation.z = 90f;
                position.x = alignment switch
                {
                    LabelAlignment.Top => LabelOffset,
                    LabelAlignment.Bottom => -LabelOffset,
                    _ => 0
                }; break;
            default:
                Debug.Log("Unrecognized direction!");
                break;
        }

        // Apply label positioning
        label.transform.localEulerAngles = rotation;
        label.transform.localPosition = position;
    }
}

public enum Direction { North, South, East, West };


