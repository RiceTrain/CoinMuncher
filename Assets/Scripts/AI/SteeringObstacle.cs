using UnityEngine;
using System.Collections;

public class SteeringObstacle : MonoBehaviour {

    [SerializeField]
    private float _radius = 1f;
    public float Radius
    {
        get { return _radius; }
    }
     
    private Transform _attachedTransform;
    public Vector3 Position
    {
        get { return _attachedTransform.position; }
    }

    private bool _taggedForAvoidance = false;
    public bool TaggedForAvoidance
    {
        get
        {
            return _taggedForAvoidance;
        }
        set
        {
            _taggedForAvoidance = value;
        }
    }

    private void Awake()
    {
        _attachedTransform = GetComponent<Transform>();
    }
}
