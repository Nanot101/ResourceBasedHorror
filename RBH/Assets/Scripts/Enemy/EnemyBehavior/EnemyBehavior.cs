using System.Collections.Generic;
using UnityEngine;
public enum EnemyStateType { Default,Idle, Patrol, Chase, Attack, Search,Stunned }

public abstract class EnemyBehavior : MonoBehaviour
{
    protected Dictionary<EnemyStateType, EnemyStatesBehavior> stateDictionary;
    protected StateMachine stateMachine;
    protected virtual void Awake()
    {
        stateMachine = new StateMachine();
    }
    public EnemyStatesBehavior GetState(EnemyStateType type)
    {
        return stateDictionary[type];
    }
    public StateMachine GetStateMachine() { return stateMachine; }
    protected virtual void Update()
    {
        stateMachine.GetCurrentState().Execute();
    }
}
