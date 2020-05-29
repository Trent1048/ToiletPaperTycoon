using System.Collections;
using System.Collections.Generic;
using UnityEngine;






public class Machine : ConveyorController
{
  public GameObject wood;
  public GameObject toiletPaper;
  



    void Update()
    {

        collectWood();

    }


    public void collectWood()
    {

        if (storedObject.CompareTag("wood"))


        {
            Debug.Log("worked");
            Destroy(storedObject);
            storedObject = Instantiate(toiletPaper);
           
        }
        else
        {
            Debug.Log("no wood");
        }

    }
    
}