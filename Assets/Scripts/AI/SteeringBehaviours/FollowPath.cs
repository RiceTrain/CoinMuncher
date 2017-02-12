using UnityEngine;
using System.Collections;

public class FollowPath {

    private Vehicle _vehicle;
    private SteeringPath _currentPath;
    private Seek _seekBehaviour;
    private Arrive _arriveBehaviour;

    public FollowPath(Vehicle vehicleToAffect, SteeringPath pathToFollow)
    {
        _vehicle = vehicleToAffect;
        _currentPath = pathToFollow;
        _seekBehaviour = new Seek(_vehicle);
        _arriveBehaviour = new Arrive(_vehicle);
    }
    
    private float _waypointDistSqr = 1f;
    public Vector3 Calculate()
    {
        if (Vector3.SqrMagnitude(_currentPath.CurrentWaypoint - _vehicle.Position) < _waypointDistSqr)
        {
            _currentPath.SetNextWaypoint();
        }

        if (!_currentPath.AtLastWaypoint())
        {
            return _seekBehaviour.Calculate(_currentPath.CurrentWaypoint);
        }
        else
        {
            return _arriveBehaviour.Calculate(_currentPath.CurrentWaypoint, Arrive.Deceleration.normal);
        }
    }
}
