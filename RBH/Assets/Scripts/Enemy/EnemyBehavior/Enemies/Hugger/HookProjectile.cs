using System;
using UnityEngine;

public class HookProjectile : MonoBehaviour
{
    private Vector3 direction;
    private Transform startPoint;
    private float speed;
    private float maxDistance;
    private Action OnComplete;

    private bool isPullingPlayer = false;
    private bool isRetracting = false;
    private Transform hookedTarget;
    public float retractSpeedMultiplier = 1.5f;
    public LineRenderer lineRenderer;
    public void Initialize(Vector3 direction, Transform startPoint, float speed, float maxDistance, Action OnComplete)
    {
        this.direction = direction;
        this.direction.z = 0;
        this.startPoint = startPoint;
        this.speed = speed;
        this.maxDistance = maxDistance;
        this.OnComplete = OnComplete;
    }

    private void Update()
    {
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, transform.position);
        if (isPullingPlayer)
        {
            PullPlayer();
            return;
        }
        if (isRetracting)
        {
            RetractHook();
            return;
        }
        transform.position += direction * speed * Time.deltaTime;

        float distanceTraveled = Vector2.Distance(startPoint.position, transform.position);
        if (distanceTraveled >= maxDistance)
        {
            isRetracting = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPullingPlayer||isRetracting) return;
        if (collision.CompareTag("Player"))
        {
            hookedTarget = collision.transform;

            PlayerMovement move = hookedTarget.GetComponent<PlayerMovement>();
            if (move != null)
            {
                move.enabled = false;
            }

            isPullingPlayer = true;
        }
    }

    private void PullPlayer()
    {
        if (hookedTarget == null)
        {
            EndHook();
            return;
        }
        
        hookedTarget.position = Vector3.MoveTowards(hookedTarget.position, startPoint.position, speed * Time.deltaTime);
        transform.position = hookedTarget.position;
        if (Vector3.Distance(hookedTarget.position, startPoint.position) < 0.5f)
        {
            hookedTarget.GetComponent<PlayerMovement>().enabled = true;
            EndHook();
        }
    }
    private void RetractHook()
    {
        transform.position = Vector3.MoveTowards(transform.position, startPoint.position, speed * retractSpeedMultiplier * Time.deltaTime);
        if (Vector2.Distance(transform.position,startPoint.position) < 0.1f)
        {
            EndHook();
        }
    }
    private void EndHook()
    {
        OnComplete?.Invoke();
        Destroy(gameObject);
    }
}