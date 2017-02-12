using UnityEngine;
using System.Collections;

public class Cohesion {

    private Vehicle _vehicle;
    private Seek _seekBehaviour;

    public Cohesion(Vehicle vehicleToAffect)
    {
        _vehicle = vehicleToAffect;
        _seekBehaviour = new Seek(_vehicle);
    }

    private Vector3 _centerOfMass;
    private Vector3 _steeringForce;
    private int _neighbourCount;
    public Vector3 Calculate(ref Vehicle[] neighbours)
    {
        _centerOfMass = Vector3.zero;
        _steeringForce = Vector3.zero;
        _neighbourCount = 0;

        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i] != _vehicle && neighbours[i].TaggedForGroupBehaviours)
            {
                _centerOfMass += neighbours[i].Position;
                _neighbourCount++;
            }
        }

        if (_neighbourCount > 0)
        {
            _centerOfMass /= (float)_neighbourCount;
            _steeringForce = _seekBehaviour.Calculate(_centerOfMass);
        }

        return _steeringForce;
    }
}
