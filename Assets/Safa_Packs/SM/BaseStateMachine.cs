using _Game_.Scripts.SM;
using UnityEngine;

public abstract class BaseStateMachine : MonoBehaviour
{
    private BaseState _currentState;
    
    [TextArea(10, 20)] public string stateHistory;

    protected virtual void Awake()
    {
        InitStateMachine();
    }

    protected virtual void Start()
    {
        _currentState = GetInitialState();
        _currentState?.Enter();
        SetStateHistory();
    }

    private void SetStateHistory()
    {
        stateHistory += $"{_currentState.StateName}\n";
    }

    protected virtual void Update()
    {
        _currentState?.Update();
    }

    /// <summary>
    /// Create states here; Example: RunState = new RunState(this, "stateName");
    /// </summary>
    protected abstract void InitStateMachine(); 
    
    /// <summary>
    /// // Set the initial state.. Which state will be the first? 
    /// </summary>
    /// <returns>State</returns>
    protected abstract BaseState GetInitialState(); 


    /// <summary>
    /// You can change the state using this: Example; ChangeState(StateClass)
    /// </summary>
    /// <param name="nextState"></param>
    /// 
    public void ChangeState(BaseState nextState) // Change the state
    {
        _currentState.Exit();
        _currentState = nextState;
        SetStateHistory();
        _currentState.Enter();
    }
}
