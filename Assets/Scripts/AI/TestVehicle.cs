using UnityEngine;
using System.Collections;

public class TestVehicle : Vehicle {

    protected override void InitialiseSteeringBehaviours()
    {
        base.InitialiseSteeringBehaviours();
        _steeringBehaviours.ActivateArriveBehaviour(PlayerMovement.Instance.transform, Arrive.Deceleration.slow);
        _steeringBehaviours.ActivateObstacleAvoidanceBehaviour();
        _steeringBehaviours.ActivateWallAvoidanceBehaviour();
    }
}
