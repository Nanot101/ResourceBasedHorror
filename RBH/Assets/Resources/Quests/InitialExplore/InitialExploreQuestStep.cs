using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialExploreQuestStep : QuestStep
{
    [SerializeField] private string stepDesc;

    [SerializeField] private int areasExplored;

    [SerializeField] private int areasToComplete;

    void Start()
    {
        stepDesc = stepDescription;
    }

    public void AreaExplored()
    {      
        if(areasExplored < areasToComplete)
        {
            areasExplored++;
        }

        UpdateQuestUICount(areasExplored, areasToComplete);

        if (areasExplored >= areasToComplete)
        {
            areasExplored = areasToComplete;

            FinishQuestStep();
        }
    }

}
