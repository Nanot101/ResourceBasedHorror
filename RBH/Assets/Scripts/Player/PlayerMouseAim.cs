using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouseAim : MonoBehaviour
{
    private float mouseX;
    private float mouseY;

    void Update()
    {
        if (GamePause.IsPaused)
        {
            return;
        }
        mouseX = InputManager.Instance.LookInput.x - (Screen.width / 2);
        mouseY = InputManager.Instance.LookInput.y - (Screen.height / 2);

        //Debug.Log(Mathf.Atan(mouseY / mouseX) * Mathf.Rad2Deg);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Mathf.Atan2(mouseY, mouseX) * Mathf.Rad2Deg - 90);
    }
}
