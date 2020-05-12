using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController instance;
    private float masterTime;

    public GameObject initialSelectedObject;
    public GameObject initialCharacter;

    private GameObject selectedObject;
    private GameObject selectedSpace;
    private bool objectIsSelected = true;

    private CharacterControl selectedCharacterControl;

    private void Awake() {

        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError("Cannot have more than one game controller");
        }
        ChangeSelectedObject(initialSelectedObject);
        ChangeSelectedCharacter(initialCharacter);
    }

    private void FixedUpdate() {
        int previousSecond = (int)masterTime;
        masterTime += Time.fixedDeltaTime;
        if ((int)masterTime != previousSecond) {
            // calls GameTick once per second
            GameTick();
        }
    }

    // put anything that runs every tick in this function
    public void GameTick() {
        TreeController.GrowTrees();
    }

    public void ChangeSelectedCharacter(GameObject character) {
        try {
            selectedCharacterControl = character.GetComponent<CharacterControl>();
        } catch {
            Debug.LogError("That is not a valid character");
        }
    }

    public CharacterType GetSelectedCharacterType() {
        return selectedCharacterControl.characterType;
    }

    public void ToggleSelectedObject() {
        objectIsSelected = !objectIsSelected;
    }

    public void ChangeSelectedObject(GameObject selectedObject) {
        this.selectedObject = selectedObject;
    }

    public GameObject GetSelectedObject() {
        if (objectIsSelected) {
            return selectedObject;
        } else {
            return null;
        }
    }

    public void ChangeSelectedSpace(GameObject newSpace) {
        selectedSpace = newSpace;
        if (!objectIsSelected && selectedCharacterControl != null) {
            selectedCharacterControl.UpdateTarget(newSpace);
        }
    }

    public GameObject GetSelectedSpace() {
        return selectedSpace;
    }
}