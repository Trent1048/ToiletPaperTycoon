using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController gameController;

    public GameObject selectedObject; // TODO: Add system to change the selected object
    public GameObject ground;

    private void Awake() {

        if (gameController == null) {
            gameController = this;
        } else {
            Debug.LogError("Cannot have more than one game controller");
        }
    }
}
