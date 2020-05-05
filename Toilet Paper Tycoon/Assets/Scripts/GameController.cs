using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController instance;

    // TODO: add a system to change these with UI
    public GameObject selectedObject;
    private GameObject selectedSpace;

    public GameObject initialCharacter; // this is temporary, will go away with the UI
    private CharacterControl selectedCharacterControl;

    private void Awake() {

        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError("Cannot have more than one game controller");
        }

        ChangeSelectedCharacter(initialCharacter);
    }

    public void ChangeSelectedCharacter(GameObject character) {
        try {
            selectedCharacterControl = character.GetComponent<CharacterControl>();
        } catch {
            Debug.LogError("That is not a valid character");
        }
    }

    public void ChangeSelectedSpace(GameObject newSpace) {
        selectedSpace = newSpace;
        if (selectedCharacterControl != null) {
            selectedCharacterControl.UpdateTarget(newSpace);
        }
    }

    public GameObject GetSelectedSpace() {
        return selectedSpace;
    }
}