using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDropItem : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private new Rigidbody2D rigidbody;

    private EnemyDropSO dropSO;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(spriteRenderer != null, $"{nameof(spriteRenderer)} is required for {nameof(EnemyDropItem)}", this);
        Debug.Assert(rigidbody != null, $"{nameof(rigidbody)} is required for {nameof(EnemyDropItem)}", this);
    }

    public void SetDropSO(EnemyDropSO dropSO)
    {
        this.dropSO = dropSO;
        spriteRenderer.sprite = this.dropSO.DropIcon;

        AddDropNameToGameObjectName(this.dropSO.DropName);
    }

    public void AddDropForce(Vector2 dropForce) => rigidbody.AddForce(dropForce);

    private void AddDropNameToGameObjectName(string dropName)
    {
        var currentGameObjectName = gameObject.name;

        gameObject.name = currentGameObjectName + " (" + dropName + ")";
    }
}
