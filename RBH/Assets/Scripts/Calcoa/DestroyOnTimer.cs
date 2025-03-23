using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTimer : MonoBehaviour
{
    public float time;

    void Update()
    {
        if (gameObject != null)
        {
            Destroy(gameObject, time);
        }
    }
}
