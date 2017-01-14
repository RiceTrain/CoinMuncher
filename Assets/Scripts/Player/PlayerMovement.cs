using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    [SerializeField]
    private float MovementSpeed = 5f;

    private Transform _playerTransform;
    private CharacterController _movementController;

    private void Awake()
    {
        _playerTransform = transform;
        _movementController = GetComponent<CharacterController>();
    }

    private Vector3 _movementDirection;
    private void Update()
    {
        _movementDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")) * MovementSpeed;
        
        _movementController.SimpleMove(_movementDirection);

        if (_movementDirection != Vector3.zero)
        {
            _playerTransform.rotation = Quaternion.LookRotation(_movementDirection, Vector3.up);
        }
    }
}
