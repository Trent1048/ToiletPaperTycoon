using UnityEngine;
using UnityEngine.UI;

public class BoxController : MonoBehaviour {

    private int tp = 0;
    public GameObject tree;
    private Text tpCount;

    private void Start() {
        GameController.instance.AddBox(gameObject);
        UpdateConveyors();
        tpCount = GameController.instance.tpCountText;
    }

    public GameObject GetTree() {
        if (tp > 0) {
            IncreaseToiletPaper(-1);
            return tree;
        } else {
            return null;
		}
	}

    public void IncreaseToiletPaper(int amount) {
        tp += amount;
        tpCount.text = "TP: " + tp;
    }

    private void UpdateConveyors() {

        //searches for conveyor and references it
        foreach (GroundSpace space in transform.parent.GetComponent<GroundSpace>().GetNeighbors()) {

            GameObject objectAttachedToSpace = space.GetCurrentObject();

            // the space has something on it
            if (objectAttachedToSpace != null) {
                ConveyorController conveyor = objectAttachedToSpace.GetComponent<ConveyorController>();

                if (conveyor != null) {
                    conveyor.FindGameObject();
				}
            }
        }
    }
}
