using UnityEngine;
using UnityEngine.UI;

public class BoxController : MonoBehaviour {

    private int tp = 0;
    public GameObject tree;
    private Text tpCount;

    private void Start() {
        GameController.instance.AddBox(gameObject);
        tpCount = GameController.instance.tpCountText;
    }

    public GameObject GetTree() {
        return tree;
	}

    public void IncreaseToiletPaper(int amount) {
        tp += amount;
        tpCount.text = "TP: " + tp;
    }
}
