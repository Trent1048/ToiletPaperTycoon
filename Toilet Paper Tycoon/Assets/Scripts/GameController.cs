using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public static GameController instance;
    private float masterTime;

    public GameObject initialSelectedObject;
    public GameObject initialCharacter;

    public GameObject groundTileParent;
    public GameObject AdditionalTiles;
    private List<GroundSpace> groundTiles;

    public AudioSource errorNoise;
    public AudioSource buildNoise;
    public AudioSource digNoise;

    private int AddCount;
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

        AddCount = AdditionalTiles.transform.childCount;
        ChangeSelectedObject(initialSelectedObject);
        ChangeSelectedCharacter(initialCharacter);

        // set up the array of ground tiles for making a graph
        if(groundTiles == null)
        {
            groundTiles = new List<GroundSpace>();
        }
        int currentSpace = 0;
        foreach (Transform tile in groundTileParent.transform) {
            groundTiles.Add(tile.gameObject.GetComponent<GroundSpace>());
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
        GroundSpace.GetNeighbor();
    }

    // put anything that runs every tick in this function
    public void GameTick() {
        TreeController.GrowTrees();
        ConveyorController.MoveObjects();
    }

    private void Update()
    {
        //if we expand map
        if (AdditionalTiles.transform.childCount != AddCount)
        {
            AddCount = AdditionalTiles.transform.childCount;
            UpdateTileCount();
        }
    }

    //add new groundspaces into groundtiles
    private void UpdateTileCount()
    {
        if (groundTiles.Count != groundTileParent.transform.childCount)
        {
            int currentSpace = 0;
            if(groundTileParent.transform.childCount > 100)
            {
                currentSpace = 100 * (groundTileParent.transform.childCount / 100 - 1);
            }

            for(int i=0; i<100; i++)
            {
                Transform tile = groundTileParent.transform.GetChild(currentSpace+i);
                groundTiles.Add(tile.gameObject.GetComponent<GroundSpace>());
                GroundSpace currentGround = tile.GetComponent<GroundSpace>();
                currentGround.tileNum = currentSpace + i;
                tile.name = "GroundTile (" + (currentSpace + i) + ")";
            }
        }
    }

    // sound effects

    public void PlayErrorNoise() {
        errorNoise.Play();
	}

    public void PlayBuildNoise() {
        buildNoise.Play();
	}

    public void PlayDigNoise() {
        digNoise.Play();
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
        if (!objectIsSelected && selectedCharacterControl != null) {
            if (tp - 30 >= 0) {
                GameObject newCharacter = Instantiate(selectedCharacterControl.gameObject, newSpace.transform.position, new Quaternion(0, 0, 0, 0));
                CharacterControl newCharacterController = newCharacter.GetComponent<CharacterControl>();
                newCharacterController.ChangeAutoAction(selectedCharacterControl.GetAutoActionType());
                IncreaseToiletPaper(-30);
            } else {
                PlayErrorNoise();
			}
        }
    }

    public GameObject GetSelectedSpace() {
        return selectedSpace;
    }

    public List<GroundSpace> GetGroundTiles() {
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
        start.marked = true;

        // the queue is not empty
        while (spacesToCheck.Count != 0) {
            GroundSpace current = spacesToCheck.Dequeue();
            if (current != start && ((current.GetCurrentObject() == null && tag == null) ||
                (tag != null && current.GetCurrentObject() != null && current.GetCurrentObject().CompareTag(tag)))) {
                return current;
            }
            // add all unmarked spaces to the queue
            foreach (GroundSpace neigbor in current.GetNeighbors()) {
                if (!neigbor.marked) {
                    neigbor.marked = true;
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
        start.marked = true;

        // the queue is not empty
        while (spacesToCheck.Count != 0) {
            GroundSpace current = spacesToCheck.Dequeue();
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
                    neigbor.marked = true;
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