using System;

public class StateMachine
{
    //Right now it is setup for enemies but can be hopefully easily changed to whatever is needed
    //Just gotta abstract the Behavior and StatesBehavior
    public EnemyBehavior behavior;
    public EnemyStatesBehavior CurrentState { get; private set; }

    public Action<EnemyStatesBehavior> OnStateChanged;

    public void Initialize(EnemyStatesBehavior state, EnemyBehavior behavior)
    {
        this.behavior = behavior;
        CurrentState = state;
        CurrentState.Initialize();
        OnStateChanged?.Invoke(CurrentState);
    }
    public void ChangeState(EnemyStatesBehavior newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Initialize();
        OnStateChanged?.Invoke(CurrentState);
    }
    public EnemyStatesBehavior GetCurrentState()
    {
        return CurrentState;
    }

}
