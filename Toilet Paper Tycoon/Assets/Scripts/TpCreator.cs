using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (storedObject.CompareTag("Tree"))//change to "paper"
        {
            paperNum++;
            Destroy(storedObject);
            if (paperNum == howMuchTp)
            {
                storedObject = Instantiate(tp);
            }
        }

    } 
}
