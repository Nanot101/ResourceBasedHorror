
using UnityEngine;

public class EnemyVisionSensor : MonoBehaviour
{
    [SerializeField] float sightDistance;
    [SerializeField] LayerMask obstacleLayers;
    public bool CanSeeCustom(Transform target)
    {
        return CanSee(obstacleLayers,target);
    }
    public bool CanSee(LayerMask customLayer, Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, sightDistance, obstacleLayers);
        if (hit.transform != null && hit.transform == target)
        {
            return true;
        }
        return false;
    }
    public bool CanSeeCustom(Vector3 target,float radius)
    {
        return CanSee(obstacleLayers,target,radius);
    }

    public bool CanSee(LayerMask customLayer, Vector3 target,float distance)
    {
        Vector3 directionToTarget = (target - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, distance, obstacleLayers);
        if (hit.transform == null)
        {
            return true;
        }
        return false;
    }
}
