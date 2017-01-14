using UnityEngine;

public class State {

    public virtual void EnterState(MonoBehaviour stateMachineController)
    {
        //Called by the state machine when changed to
    }

    public virtual void Execute(MonoBehaviour stateMachineController)
    {
        //Called by the state machine every frame
    }

    public virtual void ExitState(MonoBehaviour stateMachineController)
    {
        //Called by the state machine when switched from
    }
}
