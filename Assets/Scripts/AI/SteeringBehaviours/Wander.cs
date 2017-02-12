using UnityEngine;
using System.Collections;

public class Wander {

    private Vehicle _vehicle;
    private Vector3 _wanderTarget;
    private float _wanderJitter;
    private float _wanderRadius;
    private float _wanderDistance;

    public Wander(Vehicle vehicleToAffect, Vector3 wanderTarget, float wanderJitter, float wanderRadius, float wanderDistance)
    {
        _vehicle = vehicleToAffect;
        _wanderTarget = wanderTarget;
        _wanderJitter = wanderJitter;
        _wanderRadius = wanderRadius;
        _wanderDistance = wanderDistance;
    }
    
    private Vector3 _targetLocal;
    private Vector3 _targetWorld;
    public Vector3 Calculate()
    {
        _wanderTarget += new Vector3(Random.Range(-1, 1) * _wanderJitter, _wanderTarget.y, Random.Range(-1, 1) * _wanderJitter);
        _wanderTarget.Normalize();
        _wanderTarget *= _wanderRadius;

        _targetLocal = _wanderTarget + new Vector3(_wanderDistance, 0f, _wanderDistance);
        _targetWorld = _vehicle.transform.TransformPoint(_targetLocal);

        return _targetWorld - _vehicle.Position;
    }
}
