using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestStepUI : MonoBehaviour
{
    public GameObject checkMark;
    public GameObject countPanel;
    public QuestStep questStep;

    private bool isFinished;

    public TextMeshProUGUI stepUIText;
    public string steptext;

    public TextMeshProUGUI countText;
    public TextMeshProUGUI countTextMax;
    public bool hasCount;
    public int maxCount;
    public int count;

    public void CheckBox()
    {
        if(checkMark != null)
        {
            checkMark.SetActive(true);
        }
    }

    public void UnCheckBox()
    {
        if (checkMark != null)
        {
            checkMark.SetActive(false);
        }
    }

    public void UpdateText(string stext)
    {
        if(stext != null && stext != "")
        {
            steptext = stext;
        }

        stepUIText.text = steptext;
    }

    public void UpdateCount(int num, int max)
    {
        if (hasCount)
        {
            if (countPanel != null)
            {
                countPanel.SetActive(true);
            }

            count = num;

            if (count >= maxCount)
            {
                count = maxCount;

                CheckBox();
            }

            if (countText != null)
            {
                countText.text = "" + count;
            }

            if (countTextMax != null)
            {
                countTextMax.text = "" + maxCount;
            }

            Debug.Log("called update counts in queststepUI: count " + count + " and max count " + maxCount);
        }
        else
        {
            if (countPanel != null)
            {
                countPanel.SetActive(true);
            }
        }
    }

    public void KillStepItem()
    {
        if (!isFinished)
        {
            isFinished = true;

            Destroy(this.gameObject, 0.1f);
        }
    }
}
