using UnityEngine;

public class SteeringBehaviours {

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
}
