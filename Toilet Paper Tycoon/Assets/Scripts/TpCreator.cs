using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TpCreator : ConveyorController
{

    public GameObject tp;
    public int howMuchTp;

    // Update is called once per frame
    void Update()
    {
        CountPaper(); 
    }


    public void CountPaper()
    {
        int paperNum = 0;
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
