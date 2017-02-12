using UnityEngine;
using System.Collections;

public class Pursuit {

    private Vehicle _vehicle;

    private Seek _seekBehaviour;

    public Pursuit(Vehicle vehicleToAffect)
    {
        _vehicle = vehicleToAffect;
        _seekBehaviour = new Seek(_vehicle);
    }

    private Vector3 _toTarget;
    private float _relativeHeading;
    private float _lookAheadTime;
    public Vector3 Calculate(Vehicle evader)
    {
        _toTarget = evader.Position - _vehicle.Position;

        _relativeHeading = Vector3.Dot(_vehicle.Heading, evader.Heading);
        if (Vector3.Dot(_toTarget, _vehicle.Heading) > 0f && _relativeHeading < -0.95f)
        {
            return _seekBehaviour.Calculate(evader.Position);
        }

        _lookAheadTime = _toTarget.magnitude / (_vehicle.MaxSpeed / evader.Speed);

        return _seekBehaviour.Calculate(evader.Position + (evader.Velocity * _lookAheadTime));
    }
}
