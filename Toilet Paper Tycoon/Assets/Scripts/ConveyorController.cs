using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    //list of conveyors
    protected static List<ConveyorController> conveyorControllers;

    //list of filled conveyors
    protected static List<ConveyorController> filledConveyors;

    //singly linked nodes
    public GameObject storedObject;
    public GameObject next;

    //variables for right-click switch
    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private int switchCounter = 0;

    //hover color
    private Color startingColor;
    private Color hoverColor;

    private Dictionary<int, Vector2> offsetDictionary = new Dictionary<int, Vector2>
    {
        {0, new Vector2(-0.5f,-0.25f)},
        {1, new Vector2(-0.5f,0.25f)},
        {2, new Vector2(0.5f,0.25f)},
        {3, new Vector2(0.5f,-0.25f)}
    };

    //instantiation var
    public GameObject newObject;

    // Start is called before the first frame update
    void Start()
    {
        //makes new conveyorlist, if it doesn't exist
        if (conveyorControllers == null)
        {
            conveyorControllers = new List<ConveyorController>();
            filledConveyors = conveyorControllers;
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

        //initialize hover color
        startingColor = spriteRenderer.color;
        hoverColor = new Color(0.5f,0.5f,0.5f, 1f);

        FindGameObject();
    }

    // Update is called once per frame
    void Update()
    { 
        if(storedObject != null)
        {
            filledConveyors.Add(this);
        }
        else if(filledConveyors.Contains(this) && storedObject == null)
        {
            filledConveyors.Remove(this);
        }
    }

    //unlinks any conveyor connected through prev and next, and removes conveyor
    private void OnDestroy()
    {
        if (storedObject != null) Destroy(storedObject);
        conveyorControllers.Remove(this);
    }

    // moves object individually
    //!needs to be improved!
    public void MoveObject() {

        //if next is belt
        if (next.CompareTag("Belt"))
        {
            ConveyorController conveyor = next.GetComponent<ConveyorController>();
            if (conveyor != null && conveyor.storedObject == null && storedObject!=null)
            {
                conveyor.storedObject = storedObject;
                conveyor.storedObject.transform.SetParent(conveyor.transform, false);
                storedObject = null;
            }
        }

        //if next is a box and object is toilet paper
        if (next.CompareTag("Box"))
        {
            BoxController box = next.GetComponent<BoxController>();
            if (box != null && storedObject.CompareTag("ToiletPaper"))
            {
                box.IncreaseToiletPaper(1);
                Destroy(storedObject);
                storedObject = null;
            }
        }
        
    }

    public static void MoveObjects()
    {
        if (filledConveyors != null) {
            foreach (ConveyorController belt in filledConveyors) {
                if (belt.next != null) {
                    belt.MoveObject();
                }
            }
        }
    }

    //allows conveyor to finds any game object in front and only conveyors from behind.
    private void FindGameObject()
    {

        //searches for conveyor and references it
        foreach (GroundSpace space in transform.parent.GetComponent<GroundSpace>().GetNeighbors()) {

            GameObject objectAttachedToSpace = space.GetCurrentObject();

            // the space has something on it
            if (objectAttachedToSpace != null)
            {
                Vector2 otherPos = new Vector2(space.transform.position.x, space.transform.position.y);
                Vector2 thisPos = new Vector2(transform.parent.position.x, transform.parent.position.y);

                //gameobject infront is any gameobject
                if(thisPos + offsetDictionary[switchCounter] == otherPos)
                {
                    next = objectAttachedToSpace;
                }

                //gameobject behind is a conveyor belt
                if (objectAttachedToSpace.CompareTag(tag))
                {
                    ConveyorController conveyor = objectAttachedToSpace.GetComponent<ConveyorController>();
                    if (otherPos + offsetDictionary[conveyor.switchCounter] == thisPos)
                    {
                        conveyor.next = gameObject;
                    }
                }
            }
        }
    }

    public void EnterHover()
    {
        spriteRenderer.color = hoverColor;
    }

    public void ExitHover()
    {
        spriteRenderer.color = startingColor;
    }

    //changes sprite with right-click and check for new reference
    public void WhileHover()
    {
        
        
        if (Input.GetMouseButtonDown(1))
        {
            switchCounter++;
            if (switchCounter > 3) switchCounter = 0;
            spriteRenderer.sprite = sprites[switchCounter];
            FindGameObject();
        }

        //instantiate object for testing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(storedObject != null)
            {
                Destroy(storedObject);
            }
            else
            {
                storedObject = Instantiate(newObject, transform);
            }
        }

        //checks reference for testing
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log(next);
        }
    }

    private void OnMouseOver()
    {
        EnterHover();
        WhileHover();
        if (Input.GetMouseButtonDown(0))
        {
            transform.parent.GetComponent<GroundSpace>().MouseLeftClick();
        }
    }

    private void OnMouseExit()
    {
        ExitHover();
    }
}