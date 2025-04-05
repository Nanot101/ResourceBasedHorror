using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIController : MonoBehaviour
{
    private NavMeshAgent agent;

    float originalSpeed;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        //transform.GetChild(0).eulerAngles = new Vector3(90,0,0);
        originalSpeed = agent.speed;
    }
    public void SetDestination(Vector3 position)
    {
        if (NavMesh.SamplePosition(position,out NavMeshHit hit, 2,NavMesh.AllAreas))
        {
            agent.SetDestination(position);
        }
    }
    public void SetVelocity(float velocity)
    {

    }

    public void Stop()
    {
        agent.ResetPath();
        agent.velocity = Vector3.zero;
    }
    public void ResetPath()
    {
        agent.ResetPath();
    }
    
    public void SetSpeed(float speed)
    {
        agent.speed = speed;
    }
    public void ResetSpeed()
    {
        agent.speed = originalSpeed;
    }

    public void SetVelocity(Vector3 velocity)
    {
        agent.velocity = velocity;
    }
    public void Move(Vector3 direction)
    {
        agent.Move(direction);
    }
    public bool HasPath()
    {
        return agent.hasPath || agent.pathPending;
    }

    public void SetUpdateRotation(bool value)
    {
        agent.updateRotation = value;
    }
    public Vector3 GetVelocity()
    {
        return agent.velocity;
    }
}