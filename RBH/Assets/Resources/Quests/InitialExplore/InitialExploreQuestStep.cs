using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialExploreQuestStep : QuestStep
{
    [SerializeField] private int areasExplored;

    [SerializeField] private int areasToComplete;

    public void AreaExplored()
    {
        if(areasExplored < areasToComplete)
        {
            areasExplored++;
        }

        if (areasExplored >= areasToComplete)
        {
            FinishQuestStep();
        }
    }

}
