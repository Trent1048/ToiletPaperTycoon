using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialControl : MonoBehaviour
{
    public GameObject[] gameTip;
    private int gameTipIndex;

    void Update()
    {
        for (int i = 0; i < gameTip.Length; i++)
        {
            if (i == gameTipIndex)
            {
                gameTip[i].SetActive(true);
            } else
            {
                gameTip[i].SetActive(false);
            }
        }
        
         if (Input.GetMouseButtonDown(0))
         {
            gameTipIndex++;
         }
        if (Input.GetMouseButtonDown(1))
        {
            gameTipIndex--;
        }
    }
}
