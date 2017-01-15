using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
abstract public class Vehicle : MonoBehaviour {

    private Transform VehicleTransform;
    private Rigidbody AttachedRigidbody;
    internal SteeringBehaviours _steeringBehaviours;

    public Vector3 Position
    {
        get { return VehicleTransform.position; }
    }

    public Vector3 Velocity
    {
        get { return AttachedRigidbody.velocity; }
        set { AttachedRigidbody.velocity = value; }
    }

    public float Speed
    {
        get { return AttachedRigidbody.velocity.magnitude; }
    }

    private Vector3 _heading;
    public Vector3 Heading
    {
        get { return _heading; }
    }

    private Vector3 _side;
    public Vector3 Side
    {
        get { return _side; }
    }

    private float _mass
    {
        get { return AttachedRigidbody.mass; }
    }

    private float _maxTurnRate
    {
        get { return AttachedRigidbody.maxAngularVelocity; }
    }
    
    [SerializeField]
    private float maxSpeed = 8f;
    public float MaxSpeed
    {
        get { return maxSpeed; }
    }

    [SerializeField]
    private float maxForce = 4f;
    public float MaxForce
    {
        get { return maxForce; }
    }

    private void Awake()
    {
        VehicleTransform = GetComponent<Transform>();
        AttachedRigidbody = GetComponent<Rigidbody>();

        _steeringBehaviours.Init(this);
    }

    private Vector3 _steeringForce = Vector3.zero;
    private Vector3 _acceleration = Vector3.zero;
    private void Update()
    {
        _steeringForce = UpdateSteering();

        _acceleration = _steeringForce / _mass;

        Velocity += _acceleration * Time.deltaTime;
        Velocity = Vector3.ClampMagnitude(Velocity, MaxSpeed);

        if(Velocity.sqrMagnitude > 0.00000001f)
        {
            _heading = AttachedRigidbody.velocity.normalized;
            _side = Vector3.Cross(AttachedRigidbody.velocity.normalized, VehicleTransform.up);
        }
    }

    abstract protected Vector3 UpdateSteering();
}
