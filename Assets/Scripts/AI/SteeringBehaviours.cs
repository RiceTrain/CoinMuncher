using UnityEngine;

public class SteeringBehaviours {

    //[NOTE] Need to write methods of tagging obstacles, also may need to write world to local space functions based on velocities

    private Vehicle _vehicle;

    public void Init(Vehicle vehicleToControl)
    {
        _vehicle = vehicleToControl;
    }

    private Vector3 _desiredVelocity;
	public Vector3 Seek(Vector3 targetPos)
    {
        _desiredVelocity = Vector3.Normalize(targetPos - _vehicle.Position) * _vehicle.MaxSpeed;

        return (_desiredVelocity - _vehicle.Velocity);
    }

    public Vector3 Flee(Vector3 targetPos, float fleeRadius = 0f)
    {
        if((_vehicle.Position - targetPos).sqrMagnitude > fleeRadius * fleeRadius)
        {
            return Vector3.zero;
        }

        _desiredVelocity = Vector3.Normalize(_vehicle.Position - targetPos) * _vehicle.MaxSpeed;

        return (_desiredVelocity - _vehicle.Velocity);
    }

    public enum Deceleration { slow = 3, normal = 2, fast = 1 }
    private Vector3 _toTarget;
    private float _distance;
    private float _speed;
    public Vector3 Arrive(Vector3 targetPos, Deceleration deceleration, float decelerationTweaker = 0.3f)
    {
        _toTarget = targetPos - _vehicle.Position;
        _distance = _toTarget.magnitude;
        if(_distance > 0f)
        {
            _speed = _distance / ((float)deceleration * decelerationTweaker);
            _speed = Mathf.Min(_speed, _vehicle.MaxSpeed);

            _desiredVelocity = _toTarget * (_speed / _distance);

            return _desiredVelocity - _vehicle.Velocity;
        }

        return Vector3.zero;
    }

    private float _relativeHeading;
    private float _lookAheadTime;
    public Vector3 Pursuit(Vehicle evader)
    {
        _toTarget = evader.Position - _vehicle.Position;

        _relativeHeading = Vector3.Dot(_vehicle.Heading, evader.Heading);
        if(Vector3.Dot(_toTarget, _vehicle.Heading) > 0f && _relativeHeading < -0.95f)
        {
            return Seek(evader.Position);
        }

        _lookAheadTime = _toTarget.magnitude / (_vehicle.MaxSpeed / evader.Speed);

        return Seek(evader.Position + (evader.Velocity * _lookAheadTime));
    }

    public Vector3 Evade(Vehicle pursuer)
    {
        _toTarget = pursuer.Position - _vehicle.Position;

        _lookAheadTime = _toTarget.magnitude / (_vehicle.MaxSpeed + pursuer.Speed);

        return Flee(pursuer.Position + (pursuer.Velocity * _lookAheadTime));
    }

    private Vector3 _wanderTarget;
    private float _wanderJitter;
    private float _wanderRadius;
    private Vector3 _targetLocal;
    private float _wanderDistance;
    private Vector3 _targetWorld;
    public Vector3 Wander()
    {
        _wanderTarget += new Vector3(Random.Range(-1, 1) * _wanderJitter, _wanderTarget.y, Random.Range(-1, 1) * _wanderJitter);
        _wanderTarget.Normalize();
        _wanderTarget *= _wanderRadius;

        _targetLocal = _wanderTarget + new Vector3(_wanderDistance, 0f, _wanderDistance);
        _targetWorld = _vehicle.transform.TransformPoint(_targetLocal);

        return _targetWorld - _vehicle.Position;
    }
    
    private float _minDetectionBoxLength = 2f;
    private float _boxLength;
    private Transform _closestIntersectingObstacle;
    private float _distToClosestObstacle;
    private Vector3 _localPosOfClosestObstacle;
    private Vector3 _localPosition;
    public Vector3 ObstacleAvoidance(Transform[] obstacles)
    {
        _boxLength = _minDetectionBoxLength + (_vehicle.Speed / _vehicle.MaxSpeed) * _minDetectionBoxLength;

        //Insert method of tagging obstacles in the range of the box

        _closestIntersectingObstacle = null;
        _distToClosestObstacle = float.MaxValue;
        _localPosOfClosestObstacle = Vector3.zero;

        for (int i = 0; i < obstacles.Length; i++)
        {
            //Check if obstacle is tagged, only consider when it is

            _localPosition = _vehicle.transform.InverseTransformPoint(obstacles[i].position);
            _localPosition.y = _vehicle.transform.position.y;
            if(_localPosition.z >= 0f)
            {
                //Check if obstacle and vehicle could be intersecting
            }
        }

        return Vector3.zero;
    }
}
