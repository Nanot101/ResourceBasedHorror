using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; private set; }

    private void Awake()
    {
        Debug.Assert(Instance == null, $"There can be only one instance of {nameof(T)} in the scene.");

        Instance = (T)this;

        Debug.Assert(Instance != null, $"Failed to assign single instence of {nameof(T)}");
    }
}
