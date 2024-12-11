using UnityEngine;

namespace MerchantVoyage
{

    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;
        public static T Instance { get => instance; private set => instance = value; }

        protected virtual void Awake()
        {
            if (instance != null && instance != (T)this)
            {
                Destroy(gameObject);
                return;
            }
            instance = (T)this;
        }
    }

}
