using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterestArea : MonoBehaviour
{
    [SerializeField] private bool hasTriggered;

    public InitialExploreQuestStep questStep;

    [SerializeField] private GameObject gatherEffect;
    [SerializeField] private GameObject questManager;

    void Update()
    {
        if(questStep == null)
        {
            if(questManager != null)
            {
                if(questManager.GetComponentInChildren<InitialExploreQuestStep>() != null)
                {
                    questStep = questManager.GetComponentInChildren<InitialExploreQuestStep>();
                }
            }
        }

        if(questManager == null)
        {
            if(GameObject.FindGameObjectWithTag("QuestManager") != null)
            {
                if(GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>() != null)
                {
                    questManager = GameObject.FindGameObjectWithTag("QuestManager");
                }
            }
        }
    }

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
