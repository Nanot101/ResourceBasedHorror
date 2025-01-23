using System.Collections.Generic;
using UnityEngine;

public class OnSceneStartComponentActivator : MonoBehaviour
{
    [SerializeField] private List<Component> componentsToActivate = new();

    private void Start()
    {
        foreach (var component in componentsToActivate)
        {
            switch (component)
            {
                case MonoBehaviour monoBehaviour:
                    monoBehaviour.enabled = true;
                    break;
                case Renderer renderer:
                    renderer.enabled = true;
                    break;
            }
        }

        Destroy(this);
    }
}