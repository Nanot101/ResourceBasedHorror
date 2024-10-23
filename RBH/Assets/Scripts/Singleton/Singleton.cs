using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    Debug.LogError($"Failed to assign single instence of {nameof(T)}");
                }
            }

            return _instance;
        }
    }

    public static bool TryGetInstance(out T instance)
    {
        if (_instance != null)
        {
            instance = _instance;
            return true;
        }

        instance = FindObjectOfType<T>();

        if (instance == null)
        {
            return false;
        }

        _instance = instance;

        return true;
    }

    private void Awake()
    {
        if (Instance == this)
        {
            return;
        }

        Debug.LogError($"There can be only one instance of {nameof(T)} in the scene.");
        Destroy(this);
    }
}
