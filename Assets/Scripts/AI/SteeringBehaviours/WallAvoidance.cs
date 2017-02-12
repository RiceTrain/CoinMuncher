using UnityEngine;
using System.Collections;

public class WallAvoidance {

    private Vehicle _vehicle;

    public WallAvoidance(Vehicle vehicleToAffect)
    {
        _vehicle = vehicleToAffect;
    }

    private Ray[] _feelers;
    private float _currentVehicleSpeed;
    private float _distToClosestIntersectionPoint;
    private int _closestWallIndex;
    private Vector3 _closestPoint = Vector3.zero;
    private Vector3 _closestWallNormal = Vector3.zero;
    private Vector3 _steeringForce;
    private RaycastHit _raycastHit;
    public Vector3 Calculate(ref SteeringWall[] walls)
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
                if (walls[w].AttachedCollider.Raycast(_feelers[f], out _raycastHit, _currentVehicleSpeed))
                {
                    if (_raycastHit.distance < _distToClosestIntersectionPoint)
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
}
