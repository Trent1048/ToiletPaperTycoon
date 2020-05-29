using UnityEngine;

public class BoxController : MonoBehaviour {

    private int tp = 0;
    public GameObject tree;

    private void Start() {
        GameController.instance.AddBox(gameObject);
    }

    public GameObject GetTree() {
        return tree;
	}

    public void IncreaseToiletPaper(int amount) {
        tp += amount;
    }
}
