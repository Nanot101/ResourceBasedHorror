using UnityEngine;

[RequireComponent(typeof(EnemyDropSystem))]
public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private EnemyDropSystem dropSystem;

    private float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(dropSystem != null, $"{nameof(dropSystem)} is required for {nameof(EnemyHealth)}", this);
    }

    public void SetHealth(float newHealth)
    {
        Debug.Assert(newHealth >= 0, $"{nameof(newHealth)} argument must be greater than 0", this);

        currentHealth = newHealth;
    }

    public void DecreaseHealth(float amountToDecrease)
    {
        Debug.Assert(amountToDecrease >= 0, $"{nameof(amountToDecrease)} argument must be greater than 0", this);

        currentHealth -= amountToDecrease;

        Debug.Log($"{gameObject.name} has been hit! Damage Amount: {amountToDecrease}, Current Health: {currentHealth}", this);

        if (currentHealth > 0)
        {
            return;
        }

        Destroy(gameObject);

        dropSystem.DropItems();
    }
    [ContextMenu("Kill")]
    public void Kill() {
        DecreaseHealth(currentHealth);
    }
}
