using UnityEngine;
using System.Collections.Generic;
using System;

public class GroundSpace : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Color startingColor;
    private Color hoverColor;

    private GroundSpace[] neighbors;
    private GameObject currentObject;
    public GameObject[] objects;

    private ContactFilter2D contactFilter;

    // for graph based pathfinding
    public int tileNum;
    public bool marked;
    public bool hardMarked;

    private void Start() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        startingColor = spriteRenderer.color;
        hoverColor = new Color(0f, 0f, 0f, 0.5f);

        // initial tree generation
        if (UnityEngine.Random.Range(0, 3) == 0) {
            int treeType = UnityEngine.Random.Range(0, objects.Length);
            ChangeCurrentObject(objects[treeType]);
            currentObject.GetComponent<TreeController>().RandomizeAge();
        }

        contactFilter = new ContactFilter2D();
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

    //find neighbors for tile type dirtTile (significantly more expensive)
    public GroundSpace[] GetEdgeNeighbors() {
        
        List<GroundSpace> neighborHelper = new List<GroundSpace>();
        List<Collider2D> hitColliders = new List<Collider2D>();
        Physics2D.OverlapCircle(transform.position, 0.5f, contactFilter, hitColliders);

        foreach (Collider2D collider in hitColliders)
        {
            GroundSpace groundTile = collider.GetComponent<GroundSpace>();
            if (groundTile)
            {
                Vector2 checkPos = (Vector2)transform.position + new Vector2(-0.5f, -0.25f);
                if (checkPos == (Vector2)collider.transform.position){ //backleft
                    neighborHelper.Add(groundTile);
                }

                checkPos = (Vector2)transform.position + new Vector2(-0.5f, 0.25f);
                if (checkPos == (Vector2)collider.transform.position){ //frontleft
                    neighborHelper.Add(groundTile);
                }

                checkPos = (Vector2)transform.position + new Vector2(0.5f, 0.25f);
                if (checkPos == (Vector2)collider.transform.position){ //frontright
                    neighborHelper.Add(groundTile);
                }

                checkPos = (Vector2)transform.position + new Vector2(0.5f, -0.25f);
                if (checkPos == (Vector2)collider.transform.position){ //backright
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
