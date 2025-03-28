using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestEventsManager : MonoBehaviour
{
    public static QuestEventsManager instance { get; private set; }

    public QuestEvents questEvents;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Quest Events Manager in scene");
        }

        instance = this;

        questEvents = new QuestEvents();
    }

    void Start()
    {

    }
}
