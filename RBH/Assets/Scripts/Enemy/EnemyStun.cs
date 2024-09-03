using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStun : MonoBehaviour
{
    private bool isStunned = false;
    [SerializeField] float StunTime = 2f;
    private Coroutine stunCoroutine;

    [SerializeField] bool debug;
    public void TryStun()
    {
        if (!isStunned)
        {
            Stun();
        }
        else
        {
            StopCoroutine(stunCoroutine);
            Stun();
        }

        if (debug)
            Debug.Log("Enemy is stunned");
    }
    public void Stun()
    {
        isStunned = true;
        stunCoroutine = StartCoroutine(StunTimer());
    }
    private IEnumerator StunTimer()
    {
        yield return new WaitForSeconds(StunTime);
        isStunned = false;
        if (debug)
            Debug.Log("Enemy can move");
    }
}
