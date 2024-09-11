using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private EnemySO attributes;

    // Start is called before the first frame update
    void Start()
    {
        if (attributes == null)
        {
            Debug.LogError("Attributes are required for enemy");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
