using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour {

    public Sprite[] plantStage;
    private float growCount;
    private int currentPlantStage;
    private SpriteRenderer spriteRenderer;

    public GameObject leaf;
    public GameObject wood;
    private bool hasLeaves;
    protected static List<TreeController> treeControllers;

    public static void GrowTrees() {
        if (treeControllers != null) {
            foreach (TreeController tree in treeControllers) {
                tree.Grow();
            }
        }
    }

    private void Awake() {
        if (treeControllers == null) {
            treeControllers = new List<TreeController>();
        }
        treeControllers.Add(this);
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        growCount = 0f;
        currentPlantStage = (int)growCount;
        
        spriteRenderer.sprite = plantStage[currentPlantStage];
        hasLeaves = true;
    }

    public void RandomizeAge() {
        growCount = Random.Range(0f, 3f);
        currentPlantStage = (int)growCount;

        spriteRenderer.sprite = plantStage[currentPlantStage];
    }

    private void OnDestroy() {
        treeControllers.Remove(this);
    }

    public void Grow() {
        int previousPlantStage = currentPlantStage;
        growCount += 0.05f;
        currentPlantStage = (int)growCount;

        if (currentPlantStage < plantStage.Length - 1 && previousPlantStage != currentPlantStage) {
            spriteRenderer.sprite = plantStage[currentPlantStage];
        }        
    }

    public bool CanChopWood() {
        return currentPlantStage >= 2;
    }

    public bool CanPickLeaves() {
        return hasLeaves && currentPlantStage >= 2;
    }

    public GameObject PickLeaf() {
        if (CanPickLeaves()) {
            spriteRenderer.sprite = plantStage[3];
            hasLeaves = false;
            return leaf;
        } else {
            return null;
		}
    }

    public GameObject ChopWood() {
        if (CanChopWood()) {
            Destroy(gameObject);
            return wood;
        } else {
            return null;
        }
    }
}
