using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class RotateSafeNumber : MonoBehaviour
{
    public static event Action<string, int> Rotated = delegate{};
    TextMeshProUGUI numberShownText;
    private int numberShown;
    void Start(){
        numberShownText = GetComponent<TextMeshProUGUI>();
    }
    public void NumberUP()
    {
        numberShown ++;
        if (numberShown>9)
        {
            numberShown = 0;
        }
        Rotated(name,numberShown);
    }
    public void NumberDown()
    {
        numberShown --;
        if (numberShown<0)
        {
            numberShown = 9;
        }
        Rotated(name,numberShown);
    }
    void Update(){
        numberShownText.text = numberShown.ToString();
    }
}
