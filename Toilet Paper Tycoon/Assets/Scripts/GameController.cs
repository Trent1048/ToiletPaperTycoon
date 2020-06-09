using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public static GameController instance;
    private float masterTime;

    public GameObject initialSelectedObject;
    public GameObject initialCharacter;

    public GameObject groundTileParent;
    private GroundSpace[] groundTiles;

    public Text tpCountText;

    private GameObject box;
    private GameObject selectedObject;
    private GameObject selectedSpace;
    private bool objectIsSelected = true;
    private bool gameIsPaused = false;
    private bool removeObject = false;

    private int tp;

    private CharacterControl selectedCharacterControl;

    private void Awake() {

        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError("Cannot have more than one game controller");
        }
        ChangeSelectedObject(initialSelectedObject);
        ChangeSelectedCharacter(initialCharacter);

        // set up the array of ground tiles for making a graph
        groundTiles = new GroundSpace[100];
        int currentSpace = 0;
        foreach (Transform tile in groundTileParent.transform) {
            groundTiles[currentSpace] = tile.gameObject.GetComponent<GroundSpace>();
            tile.GetComponent<GroundSpace>().tileNum = currentSpace;
            currentSpace++;
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
        ConveyorController.MoveObjects();
    }

    // Pausing and Resuming the Game:

    public void PauseGame() {
        //Time.timeScale = 0;
        gameIsPaused = true;
    }

    public bool GameIsPaused() {
        return gameIsPaused;
    }

    public void ResumeGame() {
        //Time.timeScale = 1;
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
            removeObject = selectedObject.CompareTag("Shovel");
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
        if (!objectIsSelected && selectedCharacterControl != null && !selectedCharacterControl.InAutoMode()) {
            selectedCharacterControl.AddMove(newSpace);
        }
    }

    public GameObject GetSelectedSpace() {
        return selectedSpace;
    }

    public GroundSpace[] GetGroundTiles() {
        return groundTiles;
    }

    // unmarks all ground tiles as being searched already
    public void ResetGroundSearch() {
        foreach (GroundSpace tile in groundTiles) {
            tile.marked = tile.hardMarked;
        }
    }

    // returns the nearest ground tile that contains an object with the specified tag
    // if none are found, returns null
    public GroundSpace FindObjectInGround(GroundSpace start, string tag) {
        // setup
        ResetGroundSearch();
        if (start == null) {
            start = groundTiles[55]; // picks a tile in the middle if start is null
        }

        // uses a queue for breadth first search in order to find the nearest tile
        Queue<GroundSpace> spacesToCheck = new Queue<GroundSpace>();
        spacesToCheck.Enqueue(start);

        // the queue is not empty
        while (spacesToCheck.Count != 0) {
            GroundSpace current = spacesToCheck.Dequeue();
            current.marked = true;
            if (current != start && ((current.GetCurrentObject() == null && tag == null) ||
                (tag != null && current.GetCurrentObject() != null && current.GetCurrentObject().CompareTag(tag)))) {
                return current;
            }
            // add all unmarked spaces to the queue
            foreach (GroundSpace neigbor in current.GetNeighbors()) {
                if (!neigbor.marked) {
                    spacesToCheck.Enqueue(neigbor);
                }
            }

        }

        return null;
    }

    // returns the nearest ground tile that contains an adult tree
    // if none are found, returns null
    public GroundSpace FindAdultTree(GroundSpace start, bool mustHaveLeaves = false) {
        // setup
        ResetGroundSearch();
        if (start == null) {
            start = groundTiles[55]; // picks a tile in the middle if start is null
        }

        // uses a queue for breadth first search in order to find the nearest tile
        Queue<GroundSpace> spacesToCheck = new Queue<GroundSpace>();
        spacesToCheck.Enqueue(start);

        // the queue is not empty
        while (spacesToCheck.Count != 0) {
            GroundSpace current = spacesToCheck.Dequeue();
            current.marked = true;
            GameObject currentObject = current.GetCurrentObject();
            if (current != start && currentObject != null) {
                TreeController currentTree = currentObject.GetComponent<TreeController>();
                if (currentTree != null && (currentTree.CanPickLeaves() || !mustHaveLeaves)) {
                    return current;
                }
            }
            // add all unmarked spaces to the queue
            foreach (GroundSpace neigbor in current.GetNeighbors()) {
                if (!neigbor.marked) {
                    spacesToCheck.Enqueue(neigbor);
                }
            }

        }

        return null;
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

    // object deletion stuff
    public bool CanRemoveObject() {
        return removeObject;
	}

    // tp counting stuff

    public int GetToiletPaper() {
        return tp;
	}

    public void IncreaseToiletPaper(int amount) {
        tp += amount;
        tpCountText.text = "TP: " + tp;
    }
}