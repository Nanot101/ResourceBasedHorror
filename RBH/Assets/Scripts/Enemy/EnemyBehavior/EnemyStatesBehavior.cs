using System;

[Serializable]
public abstract class EnemyStatesBehavior
{
    public string stateName;
    protected StateMachine stateMachine;

    public EnemyStatesBehavior() { }

    public void Setup(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
    public abstract void Initialize();
    public abstract void Execute();
    public abstract void Exit();
}
