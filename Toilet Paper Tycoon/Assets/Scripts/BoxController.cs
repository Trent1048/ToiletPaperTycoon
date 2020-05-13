using UnityEngine;

public class BoxController : MonoBehaviour {

    private int tp = 0;

    private void Start() {
        GameController.instance.AddBox(gameObject);
    }

    public void IncreaseToiletPaper(int amount) {
        tp += amount;
    }
}
