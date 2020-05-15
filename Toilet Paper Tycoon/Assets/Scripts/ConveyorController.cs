using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ConveyorController : MonoBehaviour
{
    //list of conveyors
    protected static List<ConveyorController> conveyorControllers;

    //singly linked nodes
    public GameObject storedObject;
    public ConveyorController next;


    //variables for right-click switch
    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private int switchCounter = 0;

    private ContactFilter2D contactFilter;
    private Direction dir;

    private Dictionary<int, Vector3> offsetDictionary = new Dictionary<int, Vector3>
    {
        {0, new Vector3(-0.5f,-0.25f,0)},
        {1, new Vector3(-0.5f,0.25f,0)},
        {2, new Vector3(0.5f,0.25f,0)},
        {3, new Vector3(0.5f,-0.25f,0)}
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

        contactFilter = new ContactFilter2D();
    }

    // Update is called once per frame
    void Update()
    {
      

    }
    private void OnDestroy()
    {
        conveyorControllers.Remove(this);
    }

    public static void MoveObject()
    {
        //moves object
    }

    //switch on right click
    private void OnMouseOver()
    {
        
        if (Input.GetMouseButtonDown(1))
        {
            switchCounter++;
            spriteRenderer.sprite = sprites[switchCounter];

            List<Collider2D> hitColliders = new List<Collider2D>();
            Physics2D.OverlapCircle(transform.position, 0.5f, contactFilter, hitColliders);

            foreach (Collider2D col in hitColliders)
            {
                ConveyorController conveyor = col.GetComponent<ConveyorController>();
                Vector3 otherPos = new Vector3(col.transform.parent.position.x, col.transform.parent.position.y);
                Vector3 thisPos = new Vector3(transform.parent.position.x, transform.parent.position.y);
                if (conveyor != null)
                {
                    if (thisPos + offsetDictionary[switchCounter] == otherPos)
                    {
                        next = conveyor.next;
                    }
                }
            }

            if (switchCounter >= 3)
            {
                switchCounter = -1;
            }
        }
    }
}