using UnityEngine;

public class SteeringBehaviours {

    private Vehicle _vehicle;

    public enum UpdateTypes { WeightedTruncatedSum, WeightedTruncatedSumWithPrioritisation, PrioritisedDithering }
    private UpdateTypes _updateMethod;

    #region - Behaviour Weights/Probabilities -
    [System.Serializable]
    public struct BehaviourModifiers
    {
        [Header("Weights for each behaviour. A high weight exerts more influence.")]
        [Range(0f, 1f)]
        public float SeekWeight;
        [Range(0f, 1f)]
        public float FleeWeight;
        [Range(0f, 1f)]
        public float ArriveWeight;
        [Range(0f, 1f)]
        public float PursuitWeight;
        [Range(0f, 1f)]
        public float WanderWeight;
        [Range(0f, 1f)]
        public float ObstacleAvoidanceWeight;
        [Range(0f, 1f)]
        public float WallAvoidanceWeight;
        [Range(0f, 1f)]
        public float InterposeWeight;
        [Range(0f, 1f)]
        public float HideWeight;
        [Range(0f, 1f)]
        public float FollowPathWeight;
        [Range(0f, 1f)]
        public float OffsetPursuitWeight;
        [Range(0f, 1f)]
        public float SeparationWeight;
        [Range(0f, 1f)]
        public float AlignmentWeight;
        [Range(0f, 1f)]
        public float CohesionWeight;

        [Header("Used in PrioritisedDithering update mode.")]
        [Header("A high probability means more potential influence.")]
        [Range(0f, 1f)]
        public float SeekProbability;
        [Range(0f, 1f)]
        public float FleeProbability;
        [Range(0f, 1f)]
        public float ArriveProbability;
        [Range(0f, 1f)]
        public float PursuitProbability;
        [Range(0f, 1f)]
        public float WanderProbability;
        [Range(0f, 1f)]
        public float ObstacleAvoidanceProbability;
        [Range(0f, 1f)]
        public float WallAvoidanceProbability;
        [Range(0f, 1f)]
        public float InterposeProbability;
        [Range(0f, 1f)]
        public float HideProbability;
        [Range(0f, 1f)]
        public float FollowPathProbability;
        [Range(0f, 1f)]
        public float OffsetPursuitProbability;
        [Range(0f, 1f)]
        public float SeparationProbability;
        [Range(0f, 1f)]
        public float AlignmentProbability;
        [Range(0f, 1f)]
        public float CohesionProbability;
    }
    #endregion

    private BehaviourModifiers _behaviourModifiers;

    private Seek _seekBehaviour;
    private Vector3 _seekTargetPos;
    private Transform _seekTargetTransform;

    private Flee _fleeBehaviour;
    private Vector3 _fleeTargetPos;
    private Transform _fleeTargetTransform;
    private float _fleeRadius;

    private Arrive _arriveBehaviour;
    private Vector3 _arriveTargetPos;
    private Transform _arriveTargetTransform;
    private Arrive.Deceleration _decelerationSpeed;
    private float _decelerationTweaker;

    private Pursuit _pursuitBehaviour;
    private Vehicle _pursuitVehicle;

    private Wander _wanderBehaviour;

    private ObstacleAvoidance _obstacleAvoidanceBehaviour;

    private WallAvoidance _wallAvoidanceBehaviour;

    private Interpose _interposeBehaviour;
    private Vehicle _interposeVehicleA;
    private Vehicle _interposeVehicleB;

    private Hide _hideBehaviour;
    private Vehicle _hideTarget;

    private FollowPath _followPathBehaviour;

    private OffsetPursuit _offsetPursuitBehaviour;
    private Vehicle _offsetPursuitLeader;
    private Vector3 _pursuitOffset;

    private float _neighbourhoodRadius;

    private Separation _separationBehaviour;

    private Alignment _alignmentBehaviour;

    private Cohesion _cohesionBehaviour;

    public SteeringBehaviours(Vehicle vehicleToControl, UpdateTypes updateMethod, BehaviourModifiers behaviourWeights, float neighbourhoodRadius)
    {
        _vehicle = vehicleToControl;
        _updateMethod = updateMethod;
        _behaviourModifiers = behaviourWeights;
        _neighbourhoodRadius = neighbourhoodRadius;
    }

    private Vector3 _steeringForce;
    public Vector3 Calculate()
    {
        _steeringForce = Vector3.zero;

        switch (_updateMethod)
        {
            case UpdateTypes.WeightedTruncatedSum:
                return CalculateWeightedSum();
            case UpdateTypes.WeightedTruncatedSumWithPrioritisation:
                return CalculateWeightedSumWithPrioritisation();
            case UpdateTypes.PrioritisedDithering:
                return CalculateDithered();
            default:
                Debug.LogError("Unrecognised SteeringBehaviour UpdateType");
                return CalculateWeightedSum();
        }
    }

    #region - Calculate Weighted Sum -
    private Vector3 CalculateWeightedSum()
    {
        if (SeekActivated)
        {
            _steeringForce += _seekBehaviour.Calculate(_seekTargetTransform != null ? _seekTargetTransform.position : _seekTargetPos) * _behaviourModifiers.SeekWeight;
        }
        if (FleeActivated)
        {
            _steeringForce += _fleeBehaviour.Calculate(_fleeTargetTransform != null ? _fleeTargetTransform.position : _fleeTargetPos, _fleeRadius) * _behaviourModifiers.FleeWeight;
        }
        if (ArriveActivated)
        {
            _steeringForce += _arriveBehaviour.Calculate(_arriveTargetTransform != null ? _arriveTargetTransform.position : _arriveTargetPos, _decelerationSpeed, _decelerationTweaker) * _behaviourModifiers.ArriveWeight;
        }
        if (PursuitActivated)
        {
            _steeringForce += _pursuitBehaviour.Calculate(_pursuitVehicle) * _behaviourModifiers.PursuitWeight;
        }
        if (WanderActivated)
        {
            _steeringForce += _wanderBehaviour.Calculate() * _behaviourModifiers.WanderWeight;
        }
        if (ObstacleAvoidanceActivated)
        {
            _steeringForce += _obstacleAvoidanceBehaviour.Calculate(ref _vehicle.ObstacleManagerReference.SteeringObstacles) * _behaviourModifiers.ObstacleAvoidanceWeight;
        }
        if (WallAvoidanceActivated)
        {
            _steeringForce += _wallAvoidanceBehaviour.Calculate(ref _vehicle.ObstacleManagerReference.SteeringWalls) * _behaviourModifiers.WallAvoidanceWeight;
        }
        if (InterposeActivated)
        {
            _steeringForce += _interposeBehaviour.Calculate(_interposeVehicleA, _interposeVehicleB) * _behaviourModifiers.InterposeWeight;
        }
        if (HideActivated)
        {
            _steeringForce += _hideBehaviour.Calculate(_hideTarget, ref _vehicle.ObstacleManagerReference.SteeringObstacles) * _behaviourModifiers.HideWeight;
        }
        if (FollowPathActivated)
        {
            _steeringForce += _followPathBehaviour.Calculate() * _behaviourModifiers.FollowPathWeight;
        }
        if (OffsetPursuitActivated)
        {
            _steeringForce += _offsetPursuitBehaviour.Calculate(_offsetPursuitLeader, _pursuitOffset) * _behaviourModifiers.OffsetPursuitWeight;
        }

        if (SeparationActivated || AlignmentActivated || CohesionActivated)
        {
            _vehicle.ObstacleManagerReference.TagVehiclesWithinNeighbourRadius(_vehicle, _neighbourhoodRadius);
        }

        if (SeparationActivated)
        {
            _steeringForce += _separationBehaviour.Calculate(ref _vehicle.ObstacleManagerReference.VehiclesInWorld) * _behaviourModifiers.SeparationWeight;
        }
        if (AlignmentActivated)
        {
            _steeringForce += _alignmentBehaviour.Calculate(ref _vehicle.ObstacleManagerReference.VehiclesInWorld) * _behaviourModifiers.AlignmentWeight;
        }
        if (CohesionActivated)
        {
            _steeringForce += _cohesionBehaviour.Calculate(ref _vehicle.ObstacleManagerReference.VehiclesInWorld) * _behaviourModifiers.CohesionWeight;
        }

        return Vector3.ClampMagnitude(_steeringForce, _vehicle.MaxForce);
    }
    #endregion

    #region - Calculate Weighted Sum With Prioritisation -
    private Vector3 _currentForce;
    private Vector3 CalculateWeightedSumWithPrioritisation()
    {
        if (SeekActivated)
        {
            _currentForce = _seekBehaviour.Calculate(_seekTargetTransform != null ? _seekTargetTransform.position : _seekTargetPos) * _behaviourModifiers.SeekWeight;

            if (!AccumulateForce(_currentForce))
            {
                return _steeringForce;
            }
        }
        if (FleeActivated)
        {
            _currentForce = _fleeBehaviour.Calculate(_fleeTargetTransform != null ? _fleeTargetTransform.position : _fleeTargetPos, _fleeRadius) * _behaviourModifiers.FleeWeight;

            if (!AccumulateForce(_currentForce))
            {
                return _steeringForce;
            }
        }
        if (ArriveActivated)
        {
            _currentForce = _arriveBehaviour.Calculate(_arriveTargetTransform != null ? _arriveTargetTransform.position : _arriveTargetPos, _decelerationSpeed, _decelerationTweaker) * _behaviourModifiers.ArriveWeight;

            if (!AccumulateForce(_currentForce))
            {
                return _steeringForce;
            }
        }
        if (PursuitActivated)
        {
            _currentForce = _pursuitBehaviour.Calculate(_pursuitVehicle) * _behaviourModifiers.PursuitWeight;

            if (!AccumulateForce(_currentForce))
            {
                return _steeringForce;
            }
        }
        if (WanderActivated)
        {
            _currentForce = _wanderBehaviour.Calculate() * _behaviourModifiers.WanderWeight;

            if (!AccumulateForce(_currentForce))
            {
                return _steeringForce;
            }
        }
        if (ObstacleAvoidanceActivated)
        {
            _currentForce = _obstacleAvoidanceBehaviour.Calculate(ref _vehicle.ObstacleManagerReference.SteeringObstacles) * _behaviourModifiers.ObstacleAvoidanceWeight;

            if (!AccumulateForce(_currentForce))
            {
                return _steeringForce;
            }
        }
        if (WallAvoidanceActivated)
        {
            _currentForce = _wallAvoidanceBehaviour.Calculate(ref _vehicle.ObstacleManagerReference.SteeringWalls) * _behaviourModifiers.WallAvoidanceWeight;

            if (!AccumulateForce(_currentForce))
            {
                return _steeringForce;
            }
        }
        if (InterposeActivated)
        {
            _currentForce = _interposeBehaviour.Calculate(_interposeVehicleA, _interposeVehicleB) * _behaviourModifiers.InterposeWeight;

            if (!AccumulateForce(_currentForce))
            {
                return _steeringForce;
            }
        }
        if (HideActivated)
        {
            _currentForce = _hideBehaviour.Calculate(_hideTarget, ref _vehicle.ObstacleManagerReference.SteeringObstacles) * _behaviourModifiers.HideWeight;

            if (!AccumulateForce(_currentForce))
            {
                return _steeringForce;
            }
        }
        if (FollowPathActivated)
        {
            _currentForce = _followPathBehaviour.Calculate() * _behaviourModifiers.FollowPathWeight;

            if (!AccumulateForce(_currentForce))
            {
                return _steeringForce;
            }
        }
        if (OffsetPursuitActivated)
        {
            _currentForce = _offsetPursuitBehaviour.Calculate(_offsetPursuitLeader, _pursuitOffset) * _behaviourModifiers.OffsetPursuitWeight;

            if (!AccumulateForce(_currentForce))
            {
                return _steeringForce;
            }
        }

        if (SeparationActivated || AlignmentActivated || CohesionActivated)
        {
            _vehicle.ObstacleManagerReference.TagVehiclesWithinNeighbourRadius(_vehicle, _neighbourhoodRadius);
        }

        if (SeparationActivated)
        {
            _currentForce = _separationBehaviour.Calculate(ref _vehicle.ObstacleManagerReference.VehiclesInWorld) * _behaviourModifiers.SeparationWeight;

            if (!AccumulateForce(_currentForce))
            {
                return _steeringForce;
            }
        }
        if (AlignmentActivated)
        {
            _currentForce = _alignmentBehaviour.Calculate(ref _vehicle.ObstacleManagerReference.VehiclesInWorld) * _behaviourModifiers.AlignmentWeight;

            if (!AccumulateForce(_currentForce))
            {
                return _steeringForce;
            }
        }
        if (CohesionActivated)
        {
            _currentForce = _cohesionBehaviour.Calculate(ref _vehicle.ObstacleManagerReference.VehiclesInWorld) * _behaviourModifiers.CohesionWeight;

            if (!AccumulateForce(_currentForce))
            {
                return _steeringForce;
            }
        }

        return _steeringForce;
    }

    private float _magnitudeSoFar;
    private float _magnitudeRemaining;
    private float _magnitudeToAdd;
    private bool AccumulateForce(Vector3 forceToAdd)
    {
        _magnitudeSoFar = _steeringForce.magnitude;

        _magnitudeRemaining = _vehicle.MaxForce - _magnitudeSoFar;

        if (_magnitudeRemaining <= 0f)
        {
            return false;
        }

        _magnitudeToAdd = forceToAdd.magnitude;
        if (_magnitudeToAdd < _magnitudeRemaining)
        {
            _steeringForce += forceToAdd;
        }
        else
        {
            _steeringForce += (forceToAdd.normalized * _magnitudeRemaining);
        }

        return true;
    }
    #endregion

    #region - Calculate Dithered -
    private Vector3 CalculateDithered()
    {
        if (SeekActivated && _behaviourModifiers.SeekProbability > Random.value)
        {
            _steeringForce = _seekBehaviour.Calculate(_seekTargetTransform != null ? _seekTargetTransform.position : _seekTargetPos) * _behaviourModifiers.SeekWeight / _behaviourModifiers.SeekProbability;

            if (_steeringForce != Vector3.zero)
            {
                return Vector3.ClampMagnitude(_steeringForce, _vehicle.MaxForce);
            }
        }
        if (FleeActivated && _behaviourModifiers.FleeProbability > Random.value)
        {
            _steeringForce = _fleeBehaviour.Calculate(_fleeTargetTransform != null ? _fleeTargetTransform.position : _fleeTargetPos, _fleeRadius) * _behaviourModifiers.FleeWeight / _behaviourModifiers.FleeProbability;

            if (_steeringForce != Vector3.zero)
            {
                return Vector3.ClampMagnitude(_steeringForce, _vehicle.MaxForce);
            }
        }
        if (ArriveActivated && _behaviourModifiers.ArriveProbability > Random.value)
        {
            _steeringForce = _arriveBehaviour.Calculate(_arriveTargetTransform != null ? _arriveTargetTransform.position : _arriveTargetPos, _decelerationSpeed, _decelerationTweaker) * _behaviourModifiers.ArriveWeight / _behaviourModifiers.ArriveProbability;

            if (_steeringForce != Vector3.zero)
            {
                return Vector3.ClampMagnitude(_steeringForce, _vehicle.MaxForce);
            }
        }
        if (PursuitActivated && _behaviourModifiers.PursuitProbability > Random.value)
        {
            _steeringForce = _pursuitBehaviour.Calculate(_pursuitVehicle) * _behaviourModifiers.PursuitWeight / _behaviourModifiers.PursuitProbability;

            if (_steeringForce != Vector3.zero)
            {
                return Vector3.ClampMagnitude(_steeringForce, _vehicle.MaxForce);
            }
        }
        if (WanderActivated && _behaviourModifiers.WanderProbability > Random.value)
        {
            _steeringForce = _wanderBehaviour.Calculate() * _behaviourModifiers.WanderWeight / _behaviourModifiers.WanderProbability;

            if (_steeringForce != Vector3.zero)
            {
                return Vector3.ClampMagnitude(_steeringForce, _vehicle.MaxForce);
            }
        }
        if (ObstacleAvoidanceActivated && _behaviourModifiers.ObstacleAvoidanceProbability > Random.value)
        {
            _steeringForce = _obstacleAvoidanceBehaviour.Calculate(ref _vehicle.ObstacleManagerReference.SteeringObstacles) * _behaviourModifiers.ObstacleAvoidanceWeight / _behaviourModifiers.ObstacleAvoidanceProbability;

            if (_steeringForce != Vector3.zero)
            {
                return Vector3.ClampMagnitude(_steeringForce, _vehicle.MaxForce);
            }
        }
        if (WallAvoidanceActivated && _behaviourModifiers.WallAvoidanceProbability > Random.value)
        {
            _steeringForce = _wallAvoidanceBehaviour.Calculate(ref _vehicle.ObstacleManagerReference.SteeringWalls) * _behaviourModifiers.WallAvoidanceWeight / _behaviourModifiers.WallAvoidanceProbability;

            if (_steeringForce != Vector3.zero)
            {
                return Vector3.ClampMagnitude(_steeringForce, _vehicle.MaxForce);
            }
        }
        if (InterposeActivated && _behaviourModifiers.InterposeProbability > Random.value)
        {
            _steeringForce = _interposeBehaviour.Calculate(_interposeVehicleA, _interposeVehicleB) * _behaviourModifiers.InterposeWeight / _behaviourModifiers.InterposeProbability;

            if (_steeringForce != Vector3.zero)
            {
                return Vector3.ClampMagnitude(_steeringForce, _vehicle.MaxForce);
            }
        }
        if (HideActivated && _behaviourModifiers.HideProbability > Random.value)
        {
            _steeringForce = _hideBehaviour.Calculate(_hideTarget, ref _vehicle.ObstacleManagerReference.SteeringObstacles) * _behaviourModifiers.HideWeight / _behaviourModifiers.HideProbability;

            if (_steeringForce != Vector3.zero)
            {
                return Vector3.ClampMagnitude(_steeringForce, _vehicle.MaxForce);
            }
        }
        if (FollowPathActivated && _behaviourModifiers.FollowPathProbability > Random.value)
        {
            _steeringForce = _followPathBehaviour.Calculate() * _behaviourModifiers.FollowPathWeight / _behaviourModifiers.FollowPathProbability;

            if (_steeringForce != Vector3.zero)
            {
                return Vector3.ClampMagnitude(_steeringForce, _vehicle.MaxForce);
            }
        }
        if (OffsetPursuitActivated && _behaviourModifiers.OffsetPursuitProbability > Random.value)
        {
            _steeringForce = _offsetPursuitBehaviour.Calculate(_offsetPursuitLeader, _pursuitOffset) * _behaviourModifiers.OffsetPursuitWeight / _behaviourModifiers.OffsetPursuitProbability;

            if (_steeringForce != Vector3.zero)
            {
                return Vector3.ClampMagnitude(_steeringForce, _vehicle.MaxForce);
            }
        }

        if (SeparationActivated || AlignmentActivated || CohesionActivated)
        {
            _vehicle.ObstacleManagerReference.TagVehiclesWithinNeighbourRadius(_vehicle, _neighbourhoodRadius);
        }

        if (SeparationActivated && _behaviourModifiers.SeparationProbability > Random.value)
        {
            _steeringForce = _separationBehaviour.Calculate(ref _vehicle.ObstacleManagerReference.VehiclesInWorld) * _behaviourModifiers.SeparationWeight / _behaviourModifiers.SeparationProbability;

            if (_steeringForce != Vector3.zero)
            {
                return Vector3.ClampMagnitude(_steeringForce, _vehicle.MaxForce);
            }
        }
        if (AlignmentActivated && _behaviourModifiers.AlignmentProbability > Random.value)
        {
            _steeringForce = _alignmentBehaviour.Calculate(ref _vehicle.ObstacleManagerReference.VehiclesInWorld) * _behaviourModifiers.AlignmentWeight / _behaviourModifiers.AlignmentProbability;

            if (_steeringForce != Vector3.zero)
            {
                return Vector3.ClampMagnitude(_steeringForce, _vehicle.MaxForce);
            }
        }
        if (CohesionActivated && _behaviourModifiers.CohesionProbability > Random.value)
        {
            _steeringForce = _cohesionBehaviour.Calculate(ref _vehicle.ObstacleManagerReference.VehiclesInWorld) * _behaviourModifiers.CohesionWeight / _behaviourModifiers.CohesionProbability;

            if (_steeringForce != Vector3.zero)
            {
                return Vector3.ClampMagnitude(_steeringForce, _vehicle.MaxForce);
            }
        }

        return _steeringForce;
    }
    #endregion

    private bool SeekActivated { get { return _seekBehaviour != null; } }
    private bool FleeActivated { get { return _fleeBehaviour != null; } }
    private bool ArriveActivated { get { return _arriveBehaviour != null; } }
    private bool PursuitActivated { get { return _pursuitBehaviour != null; } }
    private bool WanderActivated { get { return _wanderBehaviour != null; } }
    private bool ObstacleAvoidanceActivated { get { return _obstacleAvoidanceBehaviour != null; } }
    private bool WallAvoidanceActivated { get { return _wallAvoidanceBehaviour != null; } }
    private bool InterposeActivated { get { return _interposeBehaviour != null; } }
    private bool HideActivated { get { return _hideBehaviour != null; } }
    private bool FollowPathActivated { get { return _followPathBehaviour != null; } }
    private bool OffsetPursuitActivated { get { return _offsetPursuitBehaviour != null; } }
    private bool SeparationActivated { get { return _separationBehaviour != null; } }
    private bool AlignmentActivated { get { return _alignmentBehaviour != null; } }
    private bool CohesionActivated { get { return _cohesionBehaviour != null; } }

    #region - Activate behaviours -
    public void ActivateSeekBehaviour(Vector3 targetPos)
    {
        _seekBehaviour = new Seek(_vehicle);
        _seekTargetPos = targetPos;
    }

    public void ActivateSeekBehaviour(Transform targetTransform)
    {
        _seekBehaviour = new Seek(_vehicle);
        _seekTargetTransform = targetTransform;
    }

    public void ActivateArriveBehaviour(Vector3 targetPos, Arrive.Deceleration decelerationSpeed, float decelerationTweaker = 0.3f)
    {
        _arriveBehaviour = new Arrive(_vehicle);
        _arriveTargetPos = targetPos;
        _decelerationSpeed = decelerationSpeed;
        _decelerationTweaker = decelerationTweaker;
    }

    public void ActivateArriveBehaviour(Transform targetTransform, Arrive.Deceleration decelerationSpeed, float decelerationTweaker = 0.3f)
    {
        _arriveBehaviour = new Arrive(_vehicle);
        _arriveTargetTransform = targetTransform;
        _decelerationSpeed = decelerationSpeed;
        _decelerationTweaker = decelerationTweaker;
    }

    public void ActivateFleeBehaviour(Vector3 fleePos, float fleeRadius = 0f)
    {
        _fleeBehaviour = new Flee(_vehicle);
        _fleeTargetPos = fleePos;
        _fleeRadius = fleeRadius;
    }

    public void ActivateFleeBehaviour(Transform fleeTransform, float fleeRadius = 0f)
    {
        _fleeBehaviour = new Flee(_vehicle);
        _fleeTargetTransform = fleeTransform;
        _fleeRadius = fleeRadius;
    }

    public void ActivatePursuitBehaviour(Vehicle vehicleToPursue)
    {
        _pursuitBehaviour = new Pursuit(_vehicle);
        _pursuitVehicle = vehicleToPursue;
    }

    public void ActivateWanderBehaviour(Vector3 target, float jitter, float radius, float distance)
    {
        _wanderBehaviour = new Wander(_vehicle, target, jitter, radius, distance);
    }

    public void ActivateObstacleAvoidanceBehaviour(float minBoxDetectionLength = 2f)
    {
        _obstacleAvoidanceBehaviour = new ObstacleAvoidance(_vehicle, minBoxDetectionLength);
    }

    public void ActivateWallAvoidanceBehaviour()
    {
        _wallAvoidanceBehaviour = new WallAvoidance(_vehicle);
    }

    public void ActivateInterposeBehaviour(Vehicle interposeVehicleA, Vehicle interposeVehicleB)
    {
        _interposeBehaviour = new Interpose(_vehicle);
        _interposeVehicleA = interposeVehicleA;
        _interposeVehicleB = interposeVehicleB;
    }

    public void ActivateHideBehaviour(Vehicle vehicleToHideFrom, float distanceFromObstacleBoundary = 1f)
    {
        _hideBehaviour = new Hide(_vehicle, distanceFromObstacleBoundary);
        _hideTarget = vehicleToHideFrom;
    }

    public void ActivateFollowPathBehaviour(SteeringPath pathToFollow)
    {
        _followPathBehaviour = new FollowPath(_vehicle, pathToFollow);
    }

    public void ActivateOffsetPursuitBehaviour(Vehicle leader, Vector3 offset)
    {
        _offsetPursuitBehaviour = new OffsetPursuit(_vehicle);
        _offsetPursuitLeader = leader;
        _pursuitOffset = offset;
    }

    public void ActivateAlignmentBehaviour()
    {
        _alignmentBehaviour = new Alignment(_vehicle);
    }

    public void ActivateCohesionBehaviour()
    {
        _cohesionBehaviour = new Cohesion(_vehicle);
    }
    #endregion

    #region - Deactivate behaviours -
    public void DeactivateSeekBehaviour()
    {
        _seekBehaviour = null;
        _seekTargetTransform = null;
    }

    public void DeactivateArriveBehaviour()
    {
        _arriveBehaviour = null;
        _arriveTargetTransform = null;
    }

    public void DeactivateFleeBehaviour()
    {
        _fleeBehaviour = null;
        _fleeTargetTransform = null;
    }

    public void DeactivatePursuitBehaviour()
    {
        _pursuitBehaviour = null;
        _pursuitVehicle = null;
    }

    public void DeactivateWanderBehaviour()
    {
        _wanderBehaviour = null;
    }

    public void DeactivateObstacleAvoidanceBehaviour()
    {
        _obstacleAvoidanceBehaviour = null;
    }

    public void DeactivateWallAvoidanceBehaviour()
    {
        _wallAvoidanceBehaviour = null;
    }

    public void DeactivateInterposeBehaviour()
    {
        _interposeBehaviour = null;
        _interposeVehicleA = null;
        _interposeVehicleB = null;
    }

    public void DeactivateHideBehaviour()
    {
        _hideBehaviour = null;
        _hideTarget = null;
    }

    public void DeactivateFollowPathBehaviour()
    {
        _followPathBehaviour = null;
    }

    public void DeactivateOffsetPursuitBehaviour()
    {
        _offsetPursuitBehaviour = null;
        _offsetPursuitLeader = null;
    }

    public void DeactivateSeparationBehaviour()
    {
        _separationBehaviour = null;
    }

    public void DeactivateAlignmentBehaviour()
    {
        _alignmentBehaviour = null;
    }

    public void DeactivateCohesionBehaviour()
    {
        _cohesionBehaviour = null;
    }
    #endregion
}
