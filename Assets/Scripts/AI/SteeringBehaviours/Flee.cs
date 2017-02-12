using UnityEngine;

public class Flee {

    private Vehicle _vehicle;

    public Flee(Vehicle vehicleToAffect)
    {
        _vehicle = vehicleToAffect;
    }

    private Vector3 _desiredVelocity;
    public Vector3 Calculate(Vector3 targetPos, float fleeRadius = 0f)
    {
        if ((_vehicle.Position - targetPos).sqrMagnitude > fleeRadius * fleeRadius)
        {
            return Vector3.zero;
        }

        _desiredVelocity = Vector3.Normalize(_vehicle.Position - targetPos) * _vehicle.MaxSpeed;

        return (_desiredVelocity - _vehicle.Velocity);
    }
}
