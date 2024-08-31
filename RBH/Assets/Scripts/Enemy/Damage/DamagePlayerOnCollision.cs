using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamagePlayerOnCollision : MonoBehaviour
{
    [Tooltip("Damage dealt to the player during collision. Must be greater than 0.")]
    [SerializeField]
    private float damage = 5.0f;

    [Tooltip("Collider used for collision detection with player.")]
    [SerializeField]
    private new Collider2D collider;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(damage > 0, "Damage must be greater than 0");

        if (collider == null)
        {
            Debug.LogError("Collider is required for damage player on collision");
            Destroy(this);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var contacts = new List<ContactPoint2D>();

        if (collision.GetContacts(contacts) <= 0)
        {
            //Debug.Log("No contacts found");
            return;
        }

        var playerDamageRecivers = SelectPlayerDamageRecivers(contacts.Where(x => x.otherCollider == collider));

        foreach (var reciver in playerDamageRecivers)
        {
            //Debug.Log($"Dealing damage");
            _ = reciver.TryDealDamage(damage);
        }
    }

    private IEnumerable<PlayerDamageReceiver> SelectPlayerDamageRecivers(IEnumerable<ContactPoint2D> contacts)
    {
        foreach (var contact in contacts)
        {
            if (contact.collider.TryGetComponent<PlayerDamageReceiver>(out var playerDamageReciver))
            {
                yield return playerDamageReciver;
            }
        }
    }
}
