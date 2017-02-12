using UnityEngine;

public class Seek {

    private Vehicle _vehicle;

    public Seek(Vehicle vehicleToAffect)
    {
        _vehicle = vehicleToAffect;
    }

    private Vector3 _desiredVelocity;
    public Vector3 Calculate(Vector3 targetPos)
    {
        _desiredVelocity = Vector3.Normalize(targetPos - _vehicle.Position) * _vehicle.MaxSpeed;

        return (_desiredVelocity - _vehicle.Velocity);
    }
}
