﻿using UnityEngine;

public class TpCreator : ConveyorController
{

    public GameObject tp;
    public int howMuchTp;

    private int paperNum = 0;

    // Update is called once per frame
    void Update()
    {
        if (storedObject) CountPaper(); 
    }


    public void CountPaper()
    {
        if (storedObject.CompareTag("Single Ply Tp"))
        {
            paperNum++;
            Destroy(storedObject);
            if (paperNum == howMuchTp)
            {
                paperNum = 0;
                storedObject = Instantiate(tp, transform);
            }
        }

    } 
}
