using UnityEngine;
using System.Collections;

public class TagProjectile : MonoBehaviour {

    [SerializeField]
    private float SpeedMetersPerSecond = 4f;
    [SerializeField]
    private float LifeDurationSecs = 10f;

    private Vector3 _forwardDirection;
    private Rigidbody _attachedRigidbody;
    private float _lifeTimer = 0f;

    private void Awake()
    {
        _forwardDirection = transform.forward;
        _attachedRigidbody = GetComponent<Rigidbody>();
        _lifeTimer = LifeDurationSecs;
    }

    private void Update()
    {
        _attachedRigidbody.velocity = _forwardDirection * SpeedMetersPerSecond;

        _lifeTimer -= Time.deltaTime;
        if(_lifeTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
