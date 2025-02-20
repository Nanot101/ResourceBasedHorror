using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipScene : MonoBehaviour
{
    public GameObject nextScene;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Todo: new Input system here
        if (Input.GetKeyDown(KeyCode.Escape)) {
            nextScene.SetActive(true);
        }
    }
}
