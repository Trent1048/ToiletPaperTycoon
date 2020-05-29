using System.Collections;
using System.Collections.Generic;
using UnityEngine;






public class MachineController : ConveyorController
{
  public GameObject wood;
  public GameObject toiletPaper;
  



    void Update()
    {

        if(storedObject) collectWood();

    }


    public void collectWood()
    {

        if (storedObject.CompareTag("wood"))
        {
            Destroy(storedObject);
            storedObject = Instantiate(toiletPaper);           
        }
    }
    
}