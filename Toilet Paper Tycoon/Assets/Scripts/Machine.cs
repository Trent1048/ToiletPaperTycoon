using System.Collections;
using System.Collections.Generic;
using UnityEngine;






public class Machine : ConveyorController
{
  public GameObject machine;
  public GameObject toiletPaper;
  int woodPiece = 0;

    public void checkConveyor()
    {
        if (conveyorControllers != null)
        {
            foreach (ConveyorController conveyor in filledConveyors)

            {
                if (conveyor.next == machine)
                {

                    woodPiece++;
                    Debug.Log("yes");
                    conveyor.MoveObject();
                    Destroy(storedObject);
                }
            }
        }
     }


  
        public void craftTp()
        {
            if (woodPiece == 2)
            {
                Instantiate(toiletPaper);
                Debug.Log("created");
                MoveObject();
            }
        }

}