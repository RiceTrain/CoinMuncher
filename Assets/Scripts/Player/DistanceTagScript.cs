using UnityEngine;
using System.Collections;

public class DistanceTagScript : MonoBehaviour {

    [SerializeField]
    private GameObject TagPrefab;
    [SerializeField]
    private float TagThrowInterval = 1f;

    private float _tagThrownTimer = 0f;

    private void Update()
    {
        if(_tagThrownTimer <= 0f)
        {
            if(Input.GetButtonDown("Throw"))
            {
                CreateAndThrowTag();
                _tagThrownTimer = TagThrowInterval;
            }
        }
        else
        {
            _tagThrownTimer -= Time.deltaTime;
        }
    }

    private void CreateAndThrowTag()
    {
        GameObject thrownTag = Instantiate(TagPrefab, transform.position, Quaternion.LookRotation(transform.forward, Vector3.up)) as GameObject;
    }
}
