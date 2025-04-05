
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    //I can make a method to get points close to a point to make enemy don't go patrolling to a point too far away
    private int currentPatrolPointIndex;

    [SerializeField] Transform[] patrolPoints;
    public Transform GetNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return null;

        Transform nextPoint = patrolPoints[currentPatrolPointIndex];
        currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
        return nextPoint;
    }
    public Transform GetRandomPatrolPoint()
    {
        if (patrolPoints.Length == 0) return null;
        int randomPointIndex = Random.Range(0, patrolPoints.Length);
        Transform randomPoint = patrolPoints[randomPointIndex];
        return randomPoint;
    }

    public Transform[] GetAllPatrolPoints()
    {
        return patrolPoints;
    }
}