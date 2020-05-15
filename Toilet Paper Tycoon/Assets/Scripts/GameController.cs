using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController instance;
    private float masterTime;

    public GameObject initialSelectedObject;
    public GameObject initialCharacter;

    private GameObject box;
    private GameObject selectedObject;
    private GameObject selectedSpace;
    private bool objectIsSelected = true;
    private bool gameIsPaused = false;

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

    private void Update() {
        if (Input.GetMouseButtonDown(1) && !objectIsSelected) {
            selectedCharacterControl.MoveToTree();
        }
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
        ConveyorController.MoveObject();
    }

    // Pausing and Resuming the Game:

    public void PauseGame() {
        Time.timeScale = 0;
        gameIsPaused = true;
    }

    public bool GameIsPaused() {
        return gameIsPaused;
    }

    public void ResumeGame() {
        Time.timeScale = 1;
        gameIsPaused = false;
    }

    public void TogglePause() {
        if (gameIsPaused) {
            ResumeGame();
        } else {
            PauseGame();
        }
    }

    // Character and Object selection:

    private void ChangeSelectedCharacter(GameObject character) {
        CharacterControl newCharacterControl = character.GetComponent<CharacterControl>();
        if (newCharacterControl != null) { 
            selectedCharacterControl = newCharacterControl;
        } else {
            Debug.LogError("That is not a valid character");
        }
    }

    public void ChangeSelectedObject(GameObject selectedObject) {
        if (selectedObject.GetComponent<CharacterControl>() == null) {
            objectIsSelected = true;
            this.selectedObject = selectedObject;
        } else {
            objectIsSelected = false;
            ChangeSelectedCharacter(selectedObject);
        }
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

    // there can only be one box in the game
    // this tells if there already is one
    // and will be used to prevent another from being created
    public bool BoxCanSpawn() {
        return box == null;
    }

    public void AddBox(GameObject box) {
        this.box = box;
    }

    public GameObject GetBox() {
        return box;
    }
}