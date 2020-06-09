using UnityEngine;
using UnityEngine.UI;

public class BoxController : MonoBehaviour {

    public GameObject tree;

    private void Start() {
        GameController.instance.AddBox(gameObject);
        UpdateConveyors();
    }

    public GameObject GetTree() {
        if (GameController.instance.GetToiletPaper() > 0) {
            GameController.instance.IncreaseToiletPaper(-1);
            return tree;
        } else {
            return null;
		}
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
