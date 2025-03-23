using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterestArea : MonoBehaviour
{
    [SerializeField] private bool hasTriggered;

    [SerializeField] private InitialExploreQuestStep questStep;

    [SerializeField] private GameObject gatherEffect;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (!hasTriggered)
            {
                hasTriggered = true;

                if(questStep != null)
                {
                    questStep.AreaExplored();

                    ApplyEffects();
                }
            }
        }
    }

    private void ApplyEffects()
    {
        if(gatherEffect != null)
        {
            Instantiate(gatherEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, 0.25f);
    }
}
