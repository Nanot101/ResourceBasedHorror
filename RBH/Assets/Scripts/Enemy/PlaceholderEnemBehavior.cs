using UnityEngine;
using System.Collections;

public class PlaceholderEnemBehavior : MonoBehaviour
{
    [SerializeField] private EnemySO enemyData;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask wallLayer;

    private float currentHealth;
    private bool isStopped = false; // To handle collision pause

    private void Start()
    {
        // Initialize health
        currentHealth = enemyData.HealthPoints;
    }

    private void FixedUpdate()
    {
        if (currentHealth <= 0)
        {
            // Stop moving when health is 0
            rb.velocity = Vector2.zero;
            return;
        }

        if (isStopped)
        {
            // Stop movement temporarily on collision
            rb.velocity = Vector2.zero;
            return;
        }

        // Check if player is within detection range and not behind a wall
        if (PlayerDetected())
        {
            // Chase the player
            ChasePlayer();
        }
        else
        {
            // Stop movement if player is not detected
            rb.velocity = Vector2.zero;
        }
    }

    private bool PlayerDetected()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            // Check if there is an unobstructed line to the player
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionToPlayer, detectionRadius);

            foreach (var hit in hits)
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                    {
                        // Wall blocks the path
                        return false;
                    }
                    if (hit.collider.gameObject == player.gameObject)
                    {
                        // Player is detected
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void ChasePlayer()
    {
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.velocity = direction * enemyData.RunSpeed;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            // Handle enemy death (e.g., play animation, drop items)
            Debug.Log("Enemy defeated!");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            StartCoroutine(StopMovementTemporarily());
        }
    }

    private IEnumerator StopMovementTemporarily()
    {
        isStopped = true;
        yield return new WaitForSeconds(0.5f);
        isStopped = false;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection radius in the editor for visualization
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw a ray to show the detection direction and wall blocking
        if (player != null)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionToPlayer, detectionRadius);

            foreach (var hit in hits)
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                    {
                        Gizmos.color = Color.red; // Wall blocks detection
                        Gizmos.DrawLine(transform.position, hit.point);
                        break;
                    }
                    if (hit.collider.gameObject == player.gameObject)
                    {
                        Gizmos.color = Color.green; // Player detected
                        Gizmos.DrawLine(transform.position, player.position);

                        // Draw a blue debug sphere on the detection radius
                        Gizmos.color = Color.blue;
                        // Project the sphere onto the edge of the detection radius, in the direction of the player
                        Vector3 detectionEdgePosition = transform.position + (Vector3)(directionToPlayer * detectionRadius);
                        Gizmos.DrawSphere(detectionEdgePosition, 0.2f);
                        break;
                    }
                }
            }
        }
    }
}
