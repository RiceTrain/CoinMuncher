using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Vehicle : MonoBehaviour {

    private Transform _vehicleTransform;
    private Rigidbody _attachedRigidbody;
    private SteeringBehaviours _steeringBehaviours;

    public SteeringEntityManager ObstacleManagerReference
    {
        get { return SteeringEntityManager.Instance; }
    }

    public Vector3 Position
    {
        get { return _vehicleTransform.position; }
    }

    public Vector3 Velocity
    {
        get { return _attachedRigidbody.velocity; }
        set { _attachedRigidbody.velocity = value; }
    }

    public float Speed
    {
        get { return _attachedRigidbody.velocity.magnitude; }
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
        get { return _attachedRigidbody.mass; }
    }

    private float _maxTurnRate
    {
        get { return _attachedRigidbody.maxAngularVelocity; }
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

    [SerializeField]
    private float radius = 1f;
    public float Radius
    {
        get { return radius; }
    }

    [SerializeField]
    private SteeringBehaviours.UpdateTypes _steeringUpdateMethod = SteeringBehaviours.UpdateTypes.WeightedTruncatedSum;

    [SerializeField]
    private SteeringBehaviours.BehaviourModifiers _steeringBehaviourWeights;

    [SerializeField]
    private float _neighbourhoodRadius = 2f;

    private bool _taggedForGroupBehaviours;
    public bool TaggedForGroupBehaviours
    {
        get { return _taggedForGroupBehaviours; }
        set { _taggedForGroupBehaviours = value; }
    }

    private void Awake()
    {
        _vehicleTransform = GetComponent<Transform>();
        _attachedRigidbody = GetComponent<Rigidbody>();

        InitialiseSteeringBehaviours();
    }

    protected virtual void InitialiseSteeringBehaviours()
    {
        _steeringBehaviours = new SteeringBehaviours(this, _steeringUpdateMethod, _steeringBehaviourWeights, _neighbourhoodRadius);
    }

    private Vector3 _steeringForce = Vector3.zero;
    private Vector3 _acceleration = Vector3.zero;
    private void Update()
    {
        _steeringForce = _steeringBehaviours.Calculate();

        _acceleration = _steeringForce / _mass;

        Velocity += _acceleration * Time.deltaTime;
        Velocity = Vector3.ClampMagnitude(Velocity, MaxSpeed);

        if(Velocity.sqrMagnitude > 0.00000001f)
        {
            _heading = _attachedRigidbody.velocity.normalized;
            _side = Vector3.Cross(_attachedRigidbody.velocity.normalized, _vehicleTransform.up);
        }
    }
}
