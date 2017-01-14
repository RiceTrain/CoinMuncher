using UnityEngine;

public class StateMachine {

    private State _globalState;
    public State GlobalState
    {
        get { return _globalState; }
    }

    private State _previousState;
    public State PreviousState
    {
        get { return _previousState; }
    }

    private State _currentState;
    public State CurrentState
    {
        get { return _currentState; }
    }

    public void Init(State initialGlobalState = null, State previousState = null, State initialState = null)
    {
        _globalState = initialGlobalState;
        _previousState = previousState;
        _currentState = initialState;
    }

    public void UpdateStates(MonoBehaviour stateMachineController)
    {
        if (_globalState != null)
        {
            _globalState.Execute(stateMachineController);
        }

        if (_currentState != null)
        {
            _currentState.Execute(stateMachineController);
        }
    }

    public void ChangeState(State newState, MonoBehaviour stateMachineController)
    {
        _previousState = _currentState;
        if(_currentState != null)
        {
            _currentState.ExitState(stateMachineController);
        }
        
        _currentState = newState;
        if (_currentState != null)
        {
            _currentState.EnterState(stateMachineController);
        }
    }

    public void RevertToPreviousState(MonoBehaviour stateMachineController)
    {
        ChangeState(_previousState, stateMachineController);
    }
}
