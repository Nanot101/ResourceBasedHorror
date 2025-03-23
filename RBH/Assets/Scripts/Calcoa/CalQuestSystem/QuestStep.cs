using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    private bool isFinished = false;

    protected void FinishQuestStep()
    {
        isFinished = true;

        //to do - advance the quest forward

        Destroy(this.gameObject);
    }
}
