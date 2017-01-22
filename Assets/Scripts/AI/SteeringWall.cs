using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class SteeringWall : MonoBehaviour {

    private Transform _attachedTransform;
    public Vector3 WallStartPosition
    {
        get { return _attachedTransform.position + (_attachedTransform.right * (_attachedBoxCollider.size.x * 0.5f)); }
    }
    public Vector3 WallEndPosition
    {
        get { return _attachedTransform.position - (_attachedTransform.right * (_attachedBoxCollider.size.x * 0.5f)); }
    }

    private BoxCollider _attachedBoxCollider;
    public BoxCollider AttachedCollider
    {
        get { return _attachedBoxCollider; }
    }
    public float WallHeight
    {
        get { return _attachedBoxCollider.size.y; }
    }

    private void Awake()
    {
        _attachedTransform = GetComponent<Transform>();
        _attachedBoxCollider = GetComponent<BoxCollider>();
    }
}
