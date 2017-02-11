using UnityEngine;

public class SteeringBehaviours {

    //[NOTE] Need to write methods of tagging obstacles, also may need to write world to local space functions based on velocities

    private Vehicle _vehicle;

    public void Init(Vehicle vehicleToControl)
    {
        _vehicle = vehicleToControl;
    }

    public Vector3 Calculate()
    {
        return Vector3.zero; //Implement partitioning of behaviours
    }

    private Vector3 _desiredVelocity;
	public Vector3 Seek(Vector3 targetPos)
    {
        _desiredVelocity = Vector3.Normalize(targetPos - _vehicle.Position) * _vehicle.MaxSpeed;

        return (_desiredVelocity - _vehicle.Velocity);
    }

    public Vector3 Flee(Vector3 targetPos, float fleeRadius = 0f)
    {
        if((_vehicle.Position - targetPos).sqrMagnitude > fleeRadius * fleeRadius)
        {
            return Vector3.zero;
        }

        _desiredVelocity = Vector3.Normalize(_vehicle.Position - targetPos) * _vehicle.MaxSpeed;

        return (_desiredVelocity - _vehicle.Velocity);
    }

    public enum Deceleration { slow = 3, normal = 2, fast = 1 }
    private Vector3 _toTarget;
    private float _distance;
    private float _speed;
    public Vector3 Arrive(Vector3 targetPos, Deceleration deceleration, float decelerationTweaker = 0.3f)
    {
        _toTarget = targetPos - _vehicle.Position;
        _distance = _toTarget.magnitude;
        if(_distance > 0f)
        {
            _speed = _distance / ((float)deceleration * decelerationTweaker);
            _speed = Mathf.Min(_speed, _vehicle.MaxSpeed);

            _desiredVelocity = _toTarget * (_speed / _distance);

            return _desiredVelocity - _vehicle.Velocity;
        }

        return Vector3.zero;
    }

    private float _relativeHeading;
    private float _lookAheadTime;
    public Vector3 Pursuit(Vehicle evader)
    {
        _toTarget = evader.Position - _vehicle.Position;

        _relativeHeading = Vector3.Dot(_vehicle.Heading, evader.Heading);
        if(Vector3.Dot(_toTarget, _vehicle.Heading) > 0f && _relativeHeading < -0.95f)
        {
            return Seek(evader.Position);
        }

        _lookAheadTime = _toTarget.magnitude / (_vehicle.MaxSpeed / evader.Speed);

        return Seek(evader.Position + (evader.Velocity * _lookAheadTime));
    }

    public Vector3 Evade(Vehicle pursuer)
    {
        _toTarget = pursuer.Position - _vehicle.Position;

        _lookAheadTime = _toTarget.magnitude / (_vehicle.MaxSpeed + pursuer.Speed);

        return Flee(pursuer.Position + (pursuer.Velocity * _lookAheadTime));
    }

    private Vector3 _wanderTarget;
    private float _wanderJitter;
    private float _wanderRadius;
    private Vector3 _targetLocal;
    private float _wanderDistance;
    private Vector3 _targetWorld;
    public Vector3 Wander()
    {
        _wanderTarget += new Vector3(Random.Range(-1, 1) * _wanderJitter, _wanderTarget.y, Random.Range(-1, 1) * _wanderJitter);
        _wanderTarget.Normalize();
        _wanderTarget *= _wanderRadius;

        _targetLocal = _wanderTarget + new Vector3(_wanderDistance, 0f, _wanderDistance);
        _targetWorld = _vehicle.transform.TransformPoint(_targetLocal);

        return _targetWorld - _vehicle.Position;
    }
    
    private float _minDetectionBoxLength = 2f;
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
    public Vector3 ObstacleAvoidance(ref SteeringObstacle[] obstacles)
    {
        _boxLength = _minDetectionBoxLength + (_vehicle.Speed / _vehicle.MaxSpeed) * _minDetectionBoxLength;

        //Tag obstacles in the range of the box
        _vehicle.ObstacleManagerReference.TagObstaclesWithinRange(_vehicle, _boxLength);

        _closestIntersectingObstacle = null;
        _distToClosestIntersectionPoint = float.MaxValue;
        _localPosOfClosestObstacle = Vector3.zero;

        for (int i = 0; i < obstacles.Length; i++)
        {
            //Check if obstacle is tagged, only consider when it is
            if(obstacles[i].TaggedForAvoidance)
            {
                _circleLocalPosition = _vehicle.transform.InverseTransformPoint(obstacles[i].Position);
                _circleLocalPosition.y = _vehicle.transform.position.y;
                if (_circleLocalPosition.z >= 0f)
                {
                    //Check if obstacle and vehicle could be intersecting
                    _expandedRadius = obstacles[i].Radius + _vehicle.Radius;
                    if(Mathf.Abs(_circleLocalPosition.x) < _expandedRadius && Mathf.Abs(_circleLocalPosition.y) < _expandedRadius)
                    {
                        //Perform a line/circle intersection test. Intersection points are given by formula x = circleX +/- sqrt(radius^2 - circleZ^2) for y = 0.
                        _intersectionSqrt = Mathf.Sqrt(_expandedRadius * _expandedRadius - _circleLocalPosition.x * _circleLocalPosition.x);

                        _intersectionPoint = _circleLocalPosition.z - _intersectionSqrt;
                        if(_intersectionPoint <= 0f)
                        {
                            _intersectionPoint = _circleLocalPosition.x + _intersectionSqrt;
                        }

                        if(_intersectionPoint < _distToClosestIntersectionPoint)
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
        if(_closestIntersectingObstacle != null)
        {
            _steeringForceMultiplier = 1f + (_boxLength - _localPosOfClosestObstacle.z) / _boxLength;

            _steeringForce.x = (_closestIntersectingObstacle.Radius - _localPosOfClosestObstacle.x) * _steeringForceMultiplier;
            _steeringForce.z = (_closestIntersectingObstacle.Radius - _localPosOfClosestObstacle.z) * _brakingWeight;
        }

        return _vehicle.transform.TransformPoint(_steeringForce);
    }

    private Ray[] _feelers;
    private float _currentVehicleSpeed;
    private int _closestWallIndex;
    private Vector3 _closestPoint = Vector3.zero;
    private Vector3 _closestWallNormal = Vector3.zero;
    private RaycastHit _raycastHit;
    public Vector3 WallAvoidance(SteeringWall[] walls)
    {
        _currentVehicleSpeed = _vehicle.Speed;
        _distToClosestIntersectionPoint = float.MaxValue;
        _closestWallIndex = -1;
        _closestPoint = Vector3.zero;
        _closestWallNormal = Vector3.zero;
        _steeringForce = Vector3.zero;

        CreateFeelers();
        for (int f = 0; f < _feelers.Length; f++)
        {
            for (int w = 0; w < walls.Length; w++)
            {
                if(walls[w].AttachedCollider.Raycast(_feelers[f], out _raycastHit, _currentVehicleSpeed))
                {
                    if(_raycastHit.distance < _distToClosestIntersectionPoint)
                    {
                        _distToClosestIntersectionPoint = _raycastHit.distance;
                        _closestWallIndex = w;
                        _closestPoint = _raycastHit.point;
                        _closestWallNormal = _raycastHit.normal;
                    }
                }
            }

            if (_closestWallIndex >= 0)
            {
                Vector3 overShoot = _feelers[f].GetPoint(_currentVehicleSpeed) - _closestPoint;
                _steeringForce = _closestWallNormal * overShoot.magnitude;
            }
        }

        return _steeringForce;
    }

    private Quaternion _rotatedDirection;
    private void CreateFeelers()
    {
        _feelers = new Ray[3];
        //forward ray
        _feelers[0] = new Ray(_vehicle.Position, _vehicle.transform.forward);

        _rotatedDirection = _vehicle.transform.rotation;
        _rotatedDirection *= Quaternion.Euler(_vehicle.transform.up * 40f);
        _feelers[1] = new Ray(_vehicle.Position, _rotatedDirection.eulerAngles);

        _rotatedDirection = _vehicle.transform.rotation;
        _rotatedDirection *= Quaternion.Euler(_vehicle.transform.up * -40f);
        _feelers[2] = new Ray(_vehicle.Position, _rotatedDirection.eulerAngles);
    }

    private Vector3 _midPoint;
    private float _timeToReachMidPoint;
    private Vector3 _futureAPosition;
    private Vector3 _futureBPosition;
    public Vector3 Interpose(Vehicle agentA, Vehicle agentB)
    {
        _midPoint = (agentA.Position + agentB.Position) / 2f;
        _timeToReachMidPoint = Vector3.Distance(_vehicle.Position, _midPoint) / _vehicle.MaxSpeed;

        _futureAPosition = agentA.Position + agentA.Velocity * _timeToReachMidPoint;
        _futureBPosition = agentB.Position + agentB.Velocity * _timeToReachMidPoint;

        _midPoint = (_futureAPosition + _futureBPosition) / 2f;

        return Arrive(_midPoint, Deceleration.fast);
    }

    private float _distToClosest;
    private Vector3 _bestHidingSpot;
    private Vector3 _hidingSpot;
    public Vector3 Hide(Vehicle target, SteeringObstacle[] obstacles)
    {
        _distToClosest = float.MaxValue;
        _bestHidingSpot = Vector3.zero;

        for (int i = 0; i < obstacles.Length; i++)
        {
            _hidingSpot = GetHidingPosition(obstacles[i].Position, obstacles[i].Radius, target.Position);

            _distance = (_hidingSpot - _vehicle.Position).sqrMagnitude;
            if(_distance < _distToClosest)
            {
                _distToClosest = _distance;
                _bestHidingSpot = _hidingSpot;
            }
        }

        if(_distToClosest == float.MaxValue)
        {
            return Evade(target);
        }

        return Arrive(_bestHidingSpot, Deceleration.fast);
    }

    private float _distanceFromObstacleBoundary = 5f;
    private float _distAway;
    private Vector3 _toObstacle;
    private Vector3 GetHidingPosition(Vector3 obstaclePosition, float obstacleRadius, Vector3 targetPosition)
    {
        _distAway = obstacleRadius + _distanceFromObstacleBoundary;
        _toObstacle = (obstaclePosition - targetPosition).normalized;

        return (_toObstacle * _distAway) + obstaclePosition;
    }

    private SteeringPath _currentPath;
    private float _waypointDistSqr = 1f;
    public Vector3 FollowPath()
    {
        if(Vector3.SqrMagnitude(_currentPath.CurrentWaypoint - _vehicle.Position) < _waypointDistSqr)
        {
            _currentPath.SetNextWaypoint();
        }

        if (!_currentPath.AtLastWaypoint())
        {
            return Seek(_currentPath.CurrentWaypoint);
        }
        else
        {
            return Arrive(_currentPath.CurrentWaypoint, Deceleration.normal);
        }
    }

    private Vector3 _offsetWorldPosition;
    public Vector3 OffsetPursuit(Vehicle leader, Vector3 offset)
    {
        _offsetWorldPosition = leader.transform.TransformPoint(offset);

        _lookAheadTime = _offsetWorldPosition.magnitude / (_vehicle.MaxSpeed + leader.Speed);

        return Arrive(_offsetWorldPosition + (leader.Velocity * _lookAheadTime), Deceleration.fast);
    }

    public Vector3 Separation(Vehicle[] neighbours)
    {
        _steeringForce = Vector3.zero;
        for (int i = 0; i < neighbours.Length; i++)
        {
            if(neighbours[i] != _vehicle && neighbours[i].TaggedForGroupBehaviours)
            {
                _toTarget = _vehicle.Position - neighbours[i].Position;

                _steeringForce += _toTarget.normalized / _toTarget.magnitude;
            }
        }

        return _steeringForce;
    }

    private Vector3 _averageHeading;
    private int _neighbourCount;
    public Vector3 Alignment(Vehicle[] neighbours)
    {
        _averageHeading = Vector3.zero;
        _neighbourCount = 0;

        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i] != _vehicle && neighbours[i].TaggedForGroupBehaviours)
            {
                _averageHeading += neighbours[i].Heading;
                _neighbourCount++;
            }
        }

        if(_neighbourCount > 0)
        {
            _averageHeading /= (float)_neighbourCount;
            _averageHeading -= _vehicle.Heading;
        }

        return _averageHeading;
    }

    private Vector3 _centerOfMass;
    public Vector3 Cohesion(Vehicle[] neighbours)
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
            _steeringForce = Seek(_centerOfMass);
        }

        return _steeringForce;
    }
}
