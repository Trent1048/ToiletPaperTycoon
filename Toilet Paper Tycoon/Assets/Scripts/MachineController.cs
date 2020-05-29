using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MachineController : ConveyorController
{
  public GameObject wood;
  public GameObject toiletPaper;
  



    void Update()
    {

        if(storedObject) CollectWood();

    }


    public void CollectWood()
    {

        if (storedObject.CompareTag("Wood"))
        {
            Destroy(storedObject);
            storedObject = Instantiate(toiletPaper);           
        }
    }
    
}