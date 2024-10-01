using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    Debug.LogError($"Failed to assign single instence of {nameof(T)}");
                }
            }

            return instance;
        }
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
