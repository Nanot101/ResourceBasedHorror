using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueDebug : MonoBehaviour
{
    public DialogueSystem ds;
    public TextAsset dialogueScript;
    public List<Texture> iconList;

    void Update()
    {
        if(Input.GetKeyDown("q"))
        {
            ds.StartDialogue(dialogueScript, iconList);
        }
    }
}
