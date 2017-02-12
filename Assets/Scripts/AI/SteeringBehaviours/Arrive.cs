using UnityEngine;
using System.Collections;

public class Arrive {

    private Vehicle _vehicle;

    public Arrive(Vehicle vehicleToAffect)
    {
        _vehicle = vehicleToAffect;
    }

    public enum Deceleration { slow = 3, normal = 2, fast = 1 }

    private Vector3 _toTarget;
    private float _distance;
    private float _speed;
    private Vector3 _desiredVelocity;
    public Vector3 Calculate(Vector3 targetPos, Deceleration deceleration, float decelerationTweaker = 0.3f)
    {
        _toTarget = targetPos - _vehicle.Position;
        _distance = _toTarget.magnitude;
        if (_distance > 0f)
        {
            _speed = _distance / ((float)deceleration * decelerationTweaker);
            _speed = Mathf.Min(_speed, _vehicle.MaxSpeed);

            _desiredVelocity = _toTarget * (_speed / _distance);

            return _desiredVelocity - _vehicle.Velocity;
        }

        return Vector3.zero;
    }
}
