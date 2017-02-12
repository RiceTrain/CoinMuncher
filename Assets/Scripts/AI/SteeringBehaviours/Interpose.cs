using UnityEngine;
using System.Collections;

public class Interpose {

    private Vehicle _vehicle;
    private Arrive _arriveBehaviour;

    public Interpose(Vehicle vehicleToAffect)
    {
        _vehicle = vehicleToAffect;
        _arriveBehaviour = new Arrive(vehicleToAffect);
    }

    private Vector3 _midPoint;
    private float _timeToReachMidPoint;
    private Vector3 _futureAPosition;
    private Vector3 _futureBPosition;
    public Vector3 Calculate(Vehicle agentA, Vehicle agentB)
    {
        _midPoint = (agentA.Position + agentB.Position) / 2f;
        _timeToReachMidPoint = Vector3.Distance(_vehicle.Position, _midPoint) / _vehicle.MaxSpeed;

        _futureAPosition = agentA.Position + agentA.Velocity * _timeToReachMidPoint;
        _futureBPosition = agentB.Position + agentB.Velocity * _timeToReachMidPoint;

        _midPoint = (_futureAPosition + _futureBPosition) / 2f;

        return _arriveBehaviour.Calculate(_midPoint, Arrive.Deceleration.fast);
    }
}
