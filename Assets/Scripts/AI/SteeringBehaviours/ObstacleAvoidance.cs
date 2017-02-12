using UnityEngine;
using System.Collections;

public class ObstacleAvoidance {

    private Vehicle _vehicle;
    private float _minDetectionBoxLength;

    public ObstacleAvoidance(Vehicle vehicleToAffect, float minDetectionBoxLength = 2f)
    {
        _vehicle = vehicleToAffect;
        _minDetectionBoxLength = minDetectionBoxLength;
    }
    
    private float _boxLength;
    private SteeringObstacle _closestIntersectingObstacle;
    private float _distToClosestIntersectionPoint;
    private Vector3 _localPosOfClosestObstacle;
    private Vector3 _circleLocalPosition;
    private float _expandedRadius;
    private float _intersectionSqrt;
    private float _intersectionPoint;
    private Vector3 _steeringForce;
    private float _steeringForceMultiplier;
    private float _brakingWeight = 0.2f;
    public Vector3 Calculate(ref SteeringObstacle[] obstacles)
    {
        _boxLength = _minDetectionBoxLength + (_vehicle.Speed / _vehicle.MaxSpeed) * _minDetectionBoxLength;

        //Tag obstacles in the range of the box
        _vehicle.SteeringEntityManagerReference.TagObstaclesWithinRange(_vehicle, _boxLength);

        _closestIntersectingObstacle = null;
        _distToClosestIntersectionPoint = float.MaxValue;
        _localPosOfClosestObstacle = Vector3.zero;

        for (int i = 0; i < obstacles.Length; i++)
        {
            //Check if obstacle is tagged, only consider when it is
            if (obstacles[i].TaggedForAvoidance)
            {
                _circleLocalPosition = _vehicle.transform.InverseTransformPoint(obstacles[i].Position);
                _circleLocalPosition.y = _vehicle.transform.position.y;
                if (_circleLocalPosition.z >= 0f)
                {
                    //Check if obstacle and vehicle could be intersecting
                    _expandedRadius = obstacles[i].Radius + _vehicle.Radius;
                    if (Mathf.Abs(_circleLocalPosition.x) < _expandedRadius && Mathf.Abs(_circleLocalPosition.y) < _expandedRadius)
                    {
                        //Perform a line/circle intersection test. Intersection points are given by formula x = circleX +/- sqrt(radius^2 - circleZ^2) for y = 0.
                        _intersectionSqrt = Mathf.Sqrt(_expandedRadius * _expandedRadius - _circleLocalPosition.x * _circleLocalPosition.x);

                        _intersectionPoint = _circleLocalPosition.z - _intersectionSqrt;
                        if (_intersectionPoint <= 0f)
                        {
                            _intersectionPoint = _circleLocalPosition.x + _intersectionSqrt;
                        }

                        if (_intersectionPoint < _distToClosestIntersectionPoint)
                        {
                            _distToClosestIntersectionPoint = _intersectionPoint;
                            _closestIntersectingObstacle = obstacles[i];
                            _localPosOfClosestObstacle = _circleLocalPosition;
                        }
                    }
                }
            }
        }

        _steeringForce = Vector3.zero;
        if (_closestIntersectingObstacle != null)
        {
            _steeringForceMultiplier = 1f + (_boxLength - _localPosOfClosestObstacle.z) / _boxLength;

            _steeringForce.x = (_closestIntersectingObstacle.Radius - _localPosOfClosestObstacle.x) * _steeringForceMultiplier;
            _steeringForce.z = (_closestIntersectingObstacle.Radius - _localPosOfClosestObstacle.z) * _brakingWeight;
        }

        return _vehicle.transform.TransformPoint(_steeringForce);
    }
}
