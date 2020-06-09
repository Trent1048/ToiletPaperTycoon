using UnityEngine;
using System.Collections.Generic;

public class GroundSpace : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Color startingColor;
    private Color hoverColor;

    private GroundSpace[] neighbors;
    private GameObject currentObject;
    public GameObject[] objects;

    private ContactFilter2D contactFilter;

    //checks getNeighbor every gametick
    protected static List<GroundSpace> noNeighborSpaces;
    public static int SEARCH_NEIGHBOR = 1;

    // for graph based pathfinding
    public int tileNum;
    public bool marked;
    public bool hardMarked;

    private void Start() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        startingColor = spriteRenderer.color;
        hoverColor = new Color(0f, 0f, 0f, 0.5f);

        if(noNeighborSpaces == null)
        {
            noNeighborSpaces = new List<GroundSpace>();
        }
        noNeighborSpaces.Add(this);

        contactFilter = new ContactFilter2D();

        // initial tree generation
        if (UnityEngine.Random.Range(0, 3) == 0) {
            int treeType = UnityEngine.Random.Range(0, objects.Length);
            ChangeCurrentObject(objects[treeType]);
            currentObject.GetComponent<TreeController>().RandomizeAge();
        }
    }

    public void ChangeCurrentObject(GameObject newObject) {
        if (currentObject != null) {
            if (GameController.instance.CanRemoveObject()) {
                Destroy(currentObject);
            }
        } else if (newObject != null && !newObject.CompareTag("Shovel")) {
            // makes sure the object is not a box or that a box can spawn if it is
            bool currentObjectIsBox = newObject.GetComponent<BoxController>() != null;
            if (!currentObjectIsBox || (currentObjectIsBox && GameController.instance.BoxCanSpawn())) {
                currentObject = Instantiate(newObject, transform);
            } else {
                currentObject = null;
            }
        } else {
            currentObject = null;
        }
    }

    public GameObject GetCurrentObject() {
        return currentObject;
    }

    //calls getNeighbors SEARCH_NEIGHBOR times per second/GameTick.
    public static void GetNeighbor()
    {
        for(int i=0; i<SEARCH_NEIGHBOR; i++)
        {
            int rand = Random.Range(0, (noNeighborSpaces.Count) - 1);
            noNeighborSpaces[rand].GetNeighbors();
        }
    }

    //finds neighbors of 'this', returns array of groundSpaces neighboring 'this'
    public GroundSpace[] GetNeighbors()
    {
        int modTileNum = tileNum % 100;
        if (neighbors == null)
        {
            if (modTileNum < 10 || modTileNum % 10 == 0 || (modTileNum - 9) % 10 == 0 || modTileNum > 89)
            {
                return GetEdgeNeighbors();
            }
            else
            {
                return GetMidNeighbors(modTileNum);
            }
        }

        if (noNeighborSpaces.Contains(this)) noNeighborSpaces.Remove(this);
        return neighbors;
    }

    //find neighbors for tile type grassTile
    private GroundSpace[] GetMidNeighbors(int modTileNum)
    {
        List<GroundSpace> neighborHelper = new List<GroundSpace>();
        GroundSpace[] allGroundTiles = GameController.instance.GetGroundTiles();

        // makes sure not to add tiles that don't exist or are on the 
        // other side of the ground area (10x10)
        if (modTileNum % 10 != 9) { //frontright
            neighborHelper.Add(allGroundTiles[tileNum + 1]);
        }
        if (modTileNum % 10 != 0) { //backleft
            neighborHelper.Add(allGroundTiles[tileNum - 1]);
        } 
        if (modTileNum / 10 != 9) { //backright
            neighborHelper.Add(allGroundTiles[tileNum + 10]);
        }
        if (modTileNum / 10 != 0) { //frontleft
            neighborHelper.Add(allGroundTiles[tileNum - 10]);
        }
        neighbors = neighborHelper.ToArray();

        return neighbors;
    }

    //find neighbors for tile type dirtTile (improved?)
    public GroundSpace[] GetEdgeNeighbors() {
        
        List<GroundSpace> neighborHelper = new List<GroundSpace>();

        //finds collider in a box area, max collider = 4
        List<Collider2D> hitColliders = new List<Collider2D>();
        Vector2 pointA = (Vector2)transform.position + new Vector2(-0.25f, 0.2f);
        Vector2 pointB = (Vector2)transform.position + new Vector2(0.25f, -0.2f);
        Physics2D.OverlapArea(pointA, pointB, contactFilter, hitColliders);
        hitColliders.Remove(gameObject.GetComponent<Collider2D>()); //remove 'this' collider from list

        //searches each collider to see if it neighbors 'this'
        foreach (Collider2D collider in hitColliders)
        {
            GroundSpace groundTile = collider.GetComponent<GroundSpace>();
            if (groundTile)
            {
                Vector2 collPos = collider.transform.position;
                Vector2 thisPos = transform.position;
                if (thisPos + new Vector2(-0.5f, -0.25f) == collPos) { //backleft
                    neighborHelper.Add(groundTile);
                }
                if (thisPos + new Vector2(-0.5f, 0.25f) == collPos) { //frontleft
                    neighborHelper.Add(groundTile);
                }
                if (thisPos + new Vector2(0.5f, 0.25f) == collPos) { //frontright
                    neighborHelper.Add(groundTile);
                }
                if (thisPos + new Vector2(0.5f, -0.25f) == collPos) { //backright
                    neighborHelper.Add(groundTile);
                }
            }
        }
        neighbors = neighborHelper.ToArray();

        return neighbors;
    }

    public void ResetNeighbors()
    {
        neighbors = null;
        noNeighborSpaces.Add(this);
    }

    public GameObject Harvest(bool chopWood = false) {
        if (currentObject != null) {
            TreeController treeControl = currentObject.GetComponent<TreeController>();
            if (treeControl != null) {
                if (chopWood) {
                    return treeControl.ChopWood();
                } else {
                    return treeControl.PickLeaf();
                }
            }
        }
        return null;
	}

    public void Deposit(GameObject item) {
        if (currentObject != null && item != null) {
            BoxController boxControl = currentObject.GetComponent<BoxController>();
            if (boxControl != null) {
                ItemController itemControl = item.GetComponent<ItemController>();
                int amount = 0;

                if (itemControl != null) {
                    amount = itemControl.value;
				}

                boxControl.IncreaseToiletPaper(amount);
            } else {
                ConveyorController conveyorControl = currentObject.GetComponent<ConveyorController>();
                if (conveyorControl != null) {
                    conveyorControl.AddObject(item);
				}
			}
        }
    }

    private void OnMouseEnter() {
        if (!GameController.instance.GameIsPaused()) {
            spriteRenderer.color = hoverColor;
            if(currentObject)
            {
                ConveyorController conveyor = currentObject.GetComponent<ConveyorController>();
                if (conveyor)
                {
                    conveyor.EnterHover();
                }
            }
        }
    }

    private void OnMouseExit() {
        spriteRenderer.color = startingColor;
        if (currentObject)
        {
            ConveyorController conveyor = currentObject.GetComponent<ConveyorController>();
            if (conveyor)
            {
                conveyor.ExitHover();
            }
        }
    }

    private void OnMouseOver()
    {
        if (currentObject)
        {
            ConveyorController conveyor = currentObject.GetComponent<ConveyorController>();
            if (conveyor)
            {
                conveyor.WhileHover();
            }
        }
    }

    private void OnMouseDown() {
        if (!GameController.instance.GameIsPaused()) {
            if (GameController.instance.GetSelectedObject() != null) {
                if (currentObject == null) {
                    ChangeCurrentObject(GameController.instance.GetSelectedObject());
                } else {
                    ChangeCurrentObject(null);
                }
            }
            GameController.instance.ChangeSelectedSpace(gameObject);
        }
    }
    
    public void MouseLeftClick()
    {
        OnMouseDown();
    }
}
