using UnityEngine;
using System.Collections;

public class OffsetPursuit {

    private Vehicle _vehicle;
    private Arrive _arriveBehaviour;

    public OffsetPursuit(Vehicle vehicleToAffect)
    {
        _vehicle = vehicleToAffect;
        _arriveBehaviour = new Arrive(_vehicle);
    }

    private Vector3 _offsetWorldPosition;
    private float _lookAheadTime;
    public Vector3 Calculate(Vehicle leader, Vector3 offset)
    {
        _offsetWorldPosition = leader.transform.TransformPoint(offset);

        _lookAheadTime = _offsetWorldPosition.magnitude / (_vehicle.MaxSpeed + leader.Speed);

        return _arriveBehaviour.Calculate(_offsetWorldPosition + (leader.Velocity * _lookAheadTime), Arrive.Deceleration.fast);
    }
}
