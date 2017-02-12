using UnityEngine;
using System.Collections;

public class Hide {

    private Vehicle _vehicle;
    private float _distanceFromObstacleBoundary;
    private Evade _evadeBehaviour;
    private Arrive _arriveBehaviour;

    public Hide(Vehicle vehicleToAffect, float distanceFromObstacleBoundary = 1f)
    {
        _vehicle = vehicleToAffect;
        _distanceFromObstacleBoundary = distanceFromObstacleBoundary;
        _evadeBehaviour = new Evade(_vehicle);
        _arriveBehaviour = new Arrive(_vehicle);
    }

    private float _distToClosest;
    private Vector3 _bestHidingSpot;
    private Vector3 _hidingSpot;
    private float _distance;
    public Vector3 Calculate(Vehicle target, ref SteeringObstacle[] obstacles)
    {
        _distToClosest = float.MaxValue;
        _bestHidingSpot = Vector3.zero;

        for (int i = 0; i < obstacles.Length; i++)
        {
            _hidingSpot = GetHidingPosition(obstacles[i].Position, obstacles[i].Radius, target.Position);

            _distance = (_hidingSpot - _vehicle.Position).sqrMagnitude;
            if (_distance < _distToClosest)
            {
                _distToClosest = _distance;
                _bestHidingSpot = _hidingSpot;
            }
        }

        if (_distToClosest == float.MaxValue)
        {
            return _evadeBehaviour.Calculate(target);
        }

        return _arriveBehaviour.Calculate(_bestHidingSpot, Arrive.Deceleration.fast);
    }
    
    private float _distAway;
    private Vector3 _toObstacle;
    private Vector3 GetHidingPosition(Vector3 obstaclePosition, float obstacleRadius, Vector3 targetPosition)
    {
        _distAway = obstacleRadius + _distanceFromObstacleBoundary;
        _toObstacle = (obstaclePosition - targetPosition).normalized;

        return (_toObstacle * _distAway) + obstaclePosition;
    }
}
