using UnityEngine;
using System.Collections;

public class Evade {

    private Vehicle _vehicle;

    private Flee _fleeBehaviour;

    public Evade(Vehicle vehicleToAffect)
    {
        _vehicle = vehicleToAffect;
        _fleeBehaviour = new Flee(_vehicle);
    }

    private Vector3 _toTarget;
    private float _lookAheadTime;
    public Vector3 Calculate(Vehicle pursuer)
    {
        _toTarget = pursuer.Position - _vehicle.Position;

        _lookAheadTime = _toTarget.magnitude / (_vehicle.MaxSpeed + pursuer.Speed);

        return _fleeBehaviour.Calculate(pursuer.Position + (pursuer.Velocity * _lookAheadTime));
    }
}
