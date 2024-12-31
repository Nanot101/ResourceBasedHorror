using UnityEngine;

public class PlaceholderEnemBehavior : MonoBehaviour
{
    [SerializeField] private EnemySO enemyData;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask wallLayer;

    private float currentHealth;

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

    private void OnDrawGizmosSelected()
    {
        // Draw detection radius in the editor for visualization
        Gizmos.color = Color.red;
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
                        break;
                    }
                }
            }
        }
    }
}
