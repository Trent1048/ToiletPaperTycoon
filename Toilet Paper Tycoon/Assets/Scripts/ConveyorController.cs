using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    //list of conveyors
    protected static List<ConveyorController> conveyorControllers;

    //connected conveyors
    private ConveyorNode head;

    //variables for right-click switch
    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private int switchCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        //makes new conveyorlist, if it doesn't exist
        if (conveyorControllers == null)
        {
            conveyorControllers = new List<ConveyorController>();
        }
        conveyorControllers.Add(this);

        //checks if inital sprite is null
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer.sprite == null)
        {
            spriteRenderer.sprite = sprites[0];
        }
        else
        {
            sprites[0] = spriteRenderer.sprite;
        }

    }

    // Update is called once per frame
    void Update()
    {
        //overlapcircle
        //checkposition
        //check using ground tile
        //usedirection
        

    }
    private void OnDestroy()
    {
        conveyorControllers.Remove(this);
    }

    public static void MoveObject()
    {

    }

    //switch on right click
    private void OnMouseOver()
    {
        
        if (Input.GetMouseButtonDown(1))
        {
            switchCounter++;
            spriteRenderer.sprite = sprites[switchCounter];
            if (switchCounter >= 3)
            {
                switchCounter = -1;
            }
        }
    }
}

public enum Direction
{
    FrontLeft,
    FrontRight,
    BackLeft,
    BackRight
}
