using UnityEngine;
using System.Collections.Generic;

public class GroundSpace : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Color startingColor;
    private Color hoverColor;

    private GroundSpace[] neighbors;
    private GameObject currentObject;
    public GameObject[] objects;

    // for graph based pathfinding
    public int tileNum;
    public bool marked;
    public bool hardMarked;

    private void Start() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        startingColor = spriteRenderer.color;
        hoverColor = new Color(0f, 0f, 0f, 0.5f);

        // initial tree generation
        if (Random.Range(0, 3) == 0) {
            int treeType = Random.Range(0, objects.Length);
            ChangeCurrentObject(objects[treeType]);
            currentObject.GetComponent<TreeController>().RandomizeAge();
        }
    }

    public void ChangeCurrentObject(GameObject newObject) {
        if (currentObject != null) {
            Destroy(currentObject);
        }
        if (newObject != null) {
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

    public GroundSpace[] GetNeighbors() {
        if (neighbors == null) {
            List<GroundSpace> neighborHelper = new List<GroundSpace>();
            GroundSpace[] allGroundTiles = GameController.instance.GetGroundTiles();

            // makes sure not to add tiles that don't exist or are on the 
            // other side of the ground area
            if (tileNum % 10 != 9) {
                neighborHelper.Add(allGroundTiles[tileNum + 1]);
            }
            if (tileNum % 10 != 0) {
                neighborHelper.Add(allGroundTiles[tileNum - 1]);
            }
            if (tileNum / 10 != 9) {
                neighborHelper.Add(allGroundTiles[tileNum + 10]);
            }
            if (tileNum / 10 != 0) {
                neighborHelper.Add(allGroundTiles[tileNum - 10]);
            }

            neighbors = neighborHelper.ToArray();
        }
        return neighbors;
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
            }
        }
    }

    private void OnMouseEnter() {
        if (!GameController.instance.GameIsPaused()) {
            spriteRenderer.color = hoverColor;
        }
    }

    private void OnMouseExit() {
        spriteRenderer.color = startingColor;
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
   
}
