using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"There can be only one instance of {nameof(T)} in the scene.");
            Destroy(this);
        }

        Instance = (T)this;

        if (Instance == null)
        {
            Debug.LogError($"Failed to assign single instence of {nameof(T)}");
            Destroy(this);
        }
    }
}
