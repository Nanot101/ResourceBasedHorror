using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficePuzzle : MonoBehaviour
{
    bool isLocked;
    int [] result, correctCombination;
    int rightNumbers;
    // Start is called before the first frame update
    void Start()
    {
        result = new int[]{0,0,0,0,0};
        correctCombination = new int[]{2,8,5,3,9};
        // change these numbers if you want the combination to be different

        RotateSafeNumber.Rotated+= CheckResults;
    }
    private void CheckResults(string wheelname, int number)
    {
        switch (wheelname)
        {
            case "wheel1":
                result[0] = number;
                break;
                
            case"wheel2":
                result[1] = number;
                break;
                
            case"wheel3":
                result[2] = number;
                break;
                
            case"wheel4":
                result[3] = number;
                break;
                
            case"wheel5":
                result[4] = number;
                break;
        }
        for (int i = 0; i <result.Length; i++)
        {
            if (result[i]== correctCombination[i])
            {
                rightNumbers ++;
            }
        }
        if (rightNumbers == 5)
        {
            Debug.Log("Open");
            //open safe
        }
        else{
            rightNumbers = 0;
        }
    }
}