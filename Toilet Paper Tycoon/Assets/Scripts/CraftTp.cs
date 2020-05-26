using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CraftTp : MonoBehaviour
{
    
    public GameObject wood;
    public GameObject onePly;
    public GameObject machineTemp;

    public void craft()
    {
        
        ConveyorController belt = GetComponent<ConveyorController>();      
        int woodPieces = 0;

        if (belt.next == machineTemp)
        {
            woodPieces++;
            Destroy(wood);


        }

        if (woodPieces == 2)
        {
            Instantiate(onePly);
            woodPieces = 0;
            
        }





    }

    
}

