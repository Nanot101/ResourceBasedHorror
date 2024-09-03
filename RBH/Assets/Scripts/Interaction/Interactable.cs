using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    private InteractableUI UI;

    [field: SerializeField]
    public InteractionBase Interaction { get; private set; }

    public Vector2 Position => gameObject.transform.position;

    // Start is called before the first frame update
    void Start()
    {
        if (UI == null)
        {
            Debug.LogError("UI is required for interactable");
            Destroy(this);
        }

        if (Interaction == null)
        {
            Debug.LogError("Interaction is required for interactable");
            Destroy(this);
        }
    }

    public void ShowUI() => UI.Show();

    public void HideUI() => UI.Hide();
}
