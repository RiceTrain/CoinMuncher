using UnityEngine;
using System.Collections;

public class PlayerFollowCamera : MonoBehaviour {

    [SerializeField]
    private float DistanceAbovePlayer = 6f;
    [SerializeField]
    private float DistanceBehindPlayer = 6f;

    private Transform _cameraTransform;
    private Transform _playerTransform;
    private Vector3 _cameraOffset;

    private void Awake()
    {
        _cameraTransform = transform;

        GameObject playerGO = GameObject.FindWithTag("Player");
        _playerTransform = playerGO.transform;

        _cameraOffset = (Vector3.up * DistanceAbovePlayer) + (Vector3.back * DistanceBehindPlayer);
    }

    private void LateUpdate()
    {
        _cameraTransform.position = _playerTransform.position + _cameraOffset;
        _cameraTransform.rotation = Quaternion.LookRotation(-_cameraOffset, Vector3.up);
    }

    private void Update()
    {
        _cameraOffset = (Vector3.up * DistanceAbovePlayer) + (Vector3.back * DistanceBehindPlayer);
    }
}
