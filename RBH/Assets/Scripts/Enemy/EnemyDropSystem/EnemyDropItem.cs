using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDropItem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private new Rigidbody2D rigidbody;

    [field: FormerlySerializedAs("dropSO")]
    public EnemyDropSO DropSO { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(spriteRenderer != null, $"{nameof(spriteRenderer)} is required for {nameof(EnemyDropItem)}", this);
        Debug.Assert(rigidbody != null, $"{nameof(rigidbody)} is required for {nameof(EnemyDropItem)}", this);
    }

    public void SetDropSO(EnemyDropSO dropSO)
    {
        this.DropSO = dropSO;
        spriteRenderer.sprite = this.DropSO.DropIcon;

        AddDropNameToGameObjectName(this.DropSO.DropName);
    }

    public void AddDropForce(Vector2 dropForce) => rigidbody.AddForce(dropForce);

    private void AddDropNameToGameObjectName(string dropName)
    {
        var currentGameObjectName = gameObject.name;

        gameObject.name = currentGameObjectName + " (" + dropName + ")";
    }
}