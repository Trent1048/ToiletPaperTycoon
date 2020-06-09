using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ConveyorController : MonoBehaviour
{
    //list of conveyors
    protected static List<ConveyorController> conveyorControllers;

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

        //initialize hover color
        startingColor = spriteRenderer.color;
        hoverColor = new Color(0.5f,0.5f,0.5f, 1f);

        FindGameObject();
    }

    //unlinks any conveyor connected through prev and next, and removes conveyor
    private void OnDestroy()
    {
        if (storedObject != null) Destroy(storedObject);
        conveyorControllers.Remove(this);
    }

    // for use by the character to put an object on the conveyor belt
    public bool AddObject(GameObject newObject) {
        if (storedObject != null) {
            return false;
		} else {
            storedObject = Instantiate(newObject, transform);
            storedObject.transform.SetParent(transform, false);
            return true;
		}
    }

    // moves object individually
    // returns if it sucessfully completed it's move regardless if it had an object on it
    public bool MoveObject() {

        if (storedObject == null) {
            return true;
        }

        if (next != null) {
            ConveyorController conveyor = next.GetComponent<ConveyorController>();
            if (conveyor != null) {
                if (conveyor.storedObject != null) {
                    return false;
                } else {
                    conveyor.storedObject = storedObject;
                    conveyor.storedObject.transform.SetParent(conveyor.transform, false);
                    storedObject = null;
                    return true;
                }
            }


            BoxController box = next.GetComponent<BoxController>();

            if (box != null) {

                int amount = 0;
                ItemController itemController = storedObject.GetComponent<ItemController>();
                if (itemController != null) {
                    amount = itemController.value;
                }

                Destroy(storedObject);
                storedObject = null;
                GameController.instance.IncreaseToiletPaper(amount);
            }
        }

        return true;
    }

    public static void MoveObjects()
    {
        if (conveyorControllers != null) {

            Queue<ConveyorController> unMovedConveyors = new Queue<ConveyorController>();
            foreach (ConveyorController conveyor in conveyorControllers) {
                if (conveyor.storedObject != null) {
                    unMovedConveyors.Enqueue(conveyor);
				}
			}

            // used to make sure the while loop won't go on forever
            int previousSize = unMovedConveyors.Count;
            int count = 0;

            ConveyorController belt;
            while (unMovedConveyors.Count > 0) {
                count++;
                belt = unMovedConveyors.Dequeue();
                if (!belt.MoveObject()) {
                    unMovedConveyors.Enqueue(belt);
				}
                if (count == previousSize) {
                    if (unMovedConveyors.Count == previousSize) {
                        // the size of the queue hasn't changed since the last check,
                        // meaning none of the belts moved and the movements are done
                        return;
					}
                    previousSize = unMovedConveyors.Count;
                    count = 0;
                }
			}
        }
    }

    // allows conveyor to finds any game object in front and only conveyors from behind.
    public void FindGameObject()
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

                ConveyorController conveyor = objectAttachedToSpace.GetComponent<ConveyorController>();
                // gameobject behind is a conveyor belt
                if (conveyor != null)
                {
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