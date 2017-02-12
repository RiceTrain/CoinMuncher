using UnityEngine;
using System.Collections;

public class Separation {

    private Vehicle _vehicle;

    public Separation(Vehicle vehicleToAffect)
    {
        _vehicle = vehicleToAffect;
    }

    private Vector3 _steeringForce;
    private Vector3 _toTarget;
    public Vector3 Calculate(ref Vehicle[] neighbours)
    {
        _steeringForce = Vector3.zero;
        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i] != _vehicle && neighbours[i].TaggedForGroupBehaviours)
            {
                _toTarget = _vehicle.Position - neighbours[i].Position;

                _steeringForce += _toTarget.normalized / _toTarget.magnitude;
            }
        }

        return _steeringForce;
    }
}
