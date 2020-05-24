using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ConveyorController : MonoBehaviour
{
    //list of conveyors
    protected static List<ConveyorController> conveyorControllers;

    //doubly linked nodes
    public GameObject storedObject;
    public ConveyorController next;
    public ConveyorController prev;

    //variables for right-click switch
    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private int switchCounter = 0;

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

        FindConveyor();
    }

    // Update is called once per frame
    void Update()
    {
      

    }

    //unlinks any conveyor connected through prev and next, and removes conveyor
    private void OnDestroy()
    {
        if(prev != null) prev.next = null;
        if(next != null) next.prev = null;
        if (storedObject != null) Destroy(storedObject);

        conveyorControllers.Remove(this);
    }

    // moves object individually
    //!needs to be improved!
    public void MoveObject() {
        //moves object from this to next
        if (storedObject != null && next.storedObject == null)
        {
            Debug.Log("tryna move");
            next.storedObject = Instantiate(storedObject, transform);
            Destroy(storedObject);
        }
    }

    public static void MoveObjects()
    {
        if (conveyorControllers != null) {
            foreach (ConveyorController belt in conveyorControllers) {
                if (belt.next != null) {
                    Debug.Log("found next");
                    belt.MoveObject();
                }
            }
        }
    }

    //changes sprite with right-click and check for new reference
    private void OnMouseOver()
    {
        
        if (Input.GetMouseButtonDown(1))
        {
            switchCounter++;
            if (switchCounter > 3) switchCounter = 0;
            spriteRenderer.sprite = sprites[switchCounter];
            FindConveyor();
        }

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
    }

    //allows conveyor to find and reference another conveyor in front or behind it
    private void FindConveyor()
    {

        //searches for conveyor and references it
        foreach (GroundSpace space in transform.parent.GetComponent<GroundSpace>().GetNeighbors()) {

            GameObject objectAttachedToSpace = space.GetCurrentObject();
            // the space has something on it
            if (objectAttachedToSpace != null) {
                ConveyorController conveyor = objectAttachedToSpace.GetComponent<ConveyorController>();

                // the thing on that space is a conveyor belt
                if (conveyor != null) {

                    Vector2 otherPos = new Vector2(space.transform.position.x, space.transform.position.y);
                    Vector2 thisPos = new Vector2(transform.parent.position.x, transform.parent.position.y);

                    if (thisPos + offsetDictionary[switchCounter] == otherPos) {
                        next = conveyor;
                        conveyor.prev = this;
                        Debug.Log("front= " + next);
                    }
                    if (otherPos + offsetDictionary[switchCounter] == thisPos)
                    {
                        conveyor.next = this;
                        prev = conveyor;
                        Debug.Log("behind= " + conveyor.next);
                    }
                }
            }
        }
    }
}