using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour {

    private float speed;
    public float accuracy;

    private Transform previousTargetLoc;

    private GameObject item;
    private GameObject itemBubble;
    private SpriteRenderer itemBubbleIcon;

    private Action CurrentAction;
    private Action AutoAction;
    private ActionType autoActionType = ActionType.Manual;
    private Queue<Action> actions;
    private bool shouldChopWood = false;
    private bool auto;

    private Animator animator;
    private Direction dir;

    public CharacterType charType;

    protected static List<CharacterControl> jerrys;
    protected static List<CharacterControl> rachels;
    protected static List<CharacterControl> ricks;

    private List<CharacterControl> equalTypeCharacters;

    private void Start() {

        // setup character control lists
        if (jerrys == null) {
            jerrys = new List<CharacterControl>();
        }
        if (rachels == null) {
            rachels = new List<CharacterControl>();
        }
        if (ricks == null) {
            ricks = new List<CharacterControl>();
        }

        Dictionary<CharacterType, List<CharacterControl>> charTypeToListDict = new Dictionary<CharacterType, List<CharacterControl>>() {
            {CharacterType.Jerry, jerrys},
            {CharacterType.Rachel, rachels},
            {CharacterType.Rick, ricks}
        };

        equalTypeCharacters = charTypeToListDict[charType];
        equalTypeCharacters.Add(this);

        animator = GetComponent<Animator>();
        actions = new Queue<Action>();

        itemBubble = transform.GetChild(0).gameObject;
        itemBubble.SetActive(false);
        itemBubbleIcon = itemBubble.transform.GetChild(0).GetComponent<SpriteRenderer>();

        GameObject gameSelectedSpace = GameController.instance.GetSelectedSpace();
        if (gameSelectedSpace != null) {
            previousTargetLoc = gameSelectedSpace.transform;
        }
    }

    private void FixedUpdate() {
        if (CurrentAction != null) {
            CurrentAction();
		} else if (actions.Count > 0) {
            CurrentAction = actions.Dequeue();
		} else if (auto && AutoAction != null) {
            AutoAction();
		}
    }

    // waits until a box is placed, then goes to the box
    private void GoToBox() {
        // wait for the box to exist
        if (!GameController.instance.BoxCanSpawn()) {
            GroundSpace boxLoc = GameController.instance.FindObjectInGround(null, "Box");
            if (boxLoc != null) {
                // move to the box
                CurrentAction = () => Move(boxLoc.transform);
            }
        }
    }

    // goes to the nearest box or conveyor belt, whatever is closer
    private void GoToBeltOrBox() {
        GroundSpace findStart = null;

        if (previousTargetLoc != null) {
            findStart = previousTargetLoc.GetComponent<GroundSpace>();
        }

        GroundSpace boxLoc = null;
        GroundSpace beltLoc = null;

        if (!GameController.instance.BoxCanSpawn()) {
            boxLoc = GameController.instance.FindObjectInGround(findStart, "Box");
        }

        if (ConveyorController.conveyorControllers != null && ConveyorController.conveyorControllers.Count > 0) {
            beltLoc = GameController.instance.FindObjectInGround(findStart, "Belt");
        }

        float boxDist = float.MaxValue;
        if (boxLoc != null) {
            boxDist = Vector3.Distance(transform.position, boxLoc.transform.position);
        }

        float beltDist = float.MaxValue;
        if (beltLoc != null) {
            beltDist = Vector3.Distance(transform.position, beltLoc.transform.position);
        }

        if (beltLoc != null || boxLoc != null) {
            Transform targetLoc;

            if (boxDist <= beltDist) {
                targetLoc = boxLoc.transform;
            } else {
                targetLoc = beltLoc.transform;
			}

            CurrentAction = () => Move(targetLoc);
        }
    }

    // the target should always be a ground tile
    protected void AddMove(GameObject targetLoc) {
        actions.Enqueue(() => Move(targetLoc.transform));
    }

    private void Move(Transform target) {
        Vector3 targetPos = target.position;

        // isographic coordinates
        float targetIsoX = targetPos.y / 0.25f + targetPos.x / 0.5f;
        float targetIsoY = targetPos.y / 0.25f - targetPos.x / 0.5f;

        float isoX = transform.position.y / 0.25f + transform.position.x / 0.5f;
        float isoY = transform.position.y / 0.25f - transform.position.x / 0.5f;

        float moveX = targetIsoX - isoX;
        float moveY = targetIsoY - isoY;

        // keep the player moving on only one isometric axis
        if (Math.Abs(moveX) < accuracy) {
            moveX = 0;
            if (moveY < 0) dir = Direction.FrontRight;
            else if (moveY > 0) dir = Direction.BackLeft;
        } else {
            moveY = 0;
            if (moveX < 0) dir = Direction.FrontLeft;
            else if (moveX > 0) dir = Direction.BackRight;
        }

        animator.SetInteger("Direction", (int)dir);

        // so it won't move when it is really close
        if (Math.Abs(moveX) > accuracy || Math.Abs(moveY) > accuracy) {

            Vector3 moveVector = new Vector3((moveX - moveY) * 0.5f, (moveX + moveY) * 0.25f, 0);

            // keep the speed the same regardless of distance or framerate
            moveVector.Normalize();
            moveVector *= Time.fixedDeltaTime * speed;

            transform.Translate(moveVector);
        } else {
            CurrentAction = null;
            previousTargetLoc = target;
        }
    }

    // moves the character to a tree, picks some leaves or chops wood, and then to the box if it exists
    private void AddChopTree() {

        // go to the tree
        actions.Enqueue(() => {

            // set speed based on character type
            if ((charType == CharacterType.Jerry && !shouldChopWood) || (charType == CharacterType.Rick && shouldChopWood)) {
                speed = 0.9f;
            } else {
                speed = 0.7f;
            }

            if (item == null) {
                GroundSpace previousSpace = null;
                if (previousTargetLoc != null) {
                    previousSpace = previousTargetLoc.GetComponent<GroundSpace>();
                }
                GroundSpace treeLoc = GameController.instance.FindAdultTree(previousSpace, !shouldChopWood);
                if (treeLoc != null) {
                    treeLoc.hardMarked = true;
                    CurrentAction = () => Move(treeLoc.transform);
                } else {
                    CurrentAction = null;
                }
            } else {
                CurrentAction = null;
			}
        });

        // chop the tree or pick it's leaves
        actions.Enqueue(() => {
            GroundSpace previousGroundSpace = previousTargetLoc.GetComponent<GroundSpace>();
            if(previousGroundSpace.hardMarked) {
                previousGroundSpace.hardMarked = false;

                GameObject newItem = previousGroundSpace.Harvest(shouldChopWood);
                if (newItem != null) {
                    AddItem(newItem);

                }
            }
            CurrentAction = null;
        });

        // go to the box or a conveyor belt
        actions.Enqueue(() => GoToBeltOrBox());

        // deposit the wood/leaves at the box or on the belt
        actions.Enqueue(() => {
            previousTargetLoc.GetComponent<GroundSpace>().Deposit(PopItem());
            CurrentAction = null;
        });
    }

    private void AddPlantTree() {

        // go to the box
        actions.Enqueue(() => {
            GoToBox();

            // set speed based on character type
            if (charType == CharacterType.Rachel) {
                speed = 0.9f;
			} else {
                speed = 0.7f;
			}
        });

        // get a tree from the box
        actions.Enqueue(() => {
            GameObject box = GameController.instance.GetBox();
            // makes sure the box exists and the character is at the box's location
            if (box != null && box.transform.parent == previousTargetLoc) {
                AddItem(box.GetComponent<BoxController>().GetTree());

                // make tree f
                if (item != null) {
                    ItemController itemController = item.GetComponent<ItemController>();
                    if (itemController != null) {
                        itemController.value = 0;
                    }
                }
            }
            CurrentAction = null;
        });

        // go to a clear patch of ground
        actions.Enqueue(() => {
            if (item != null) {
                GroundSpace previousSpace = null;
                if (previousTargetLoc != null) {
                    previousSpace = previousTargetLoc.GetComponent<GroundSpace>();
                }
                GroundSpace emptyLoc = GameController.instance.FindObjectInGround(previousSpace, null);
                if (emptyLoc != null) {
                    emptyLoc.hardMarked = true;
                    CurrentAction = () => Move(emptyLoc.transform);
                } else {
                    CurrentAction = null;
                }
            } else {
                CurrentAction = null;
			}
        });

        // plant the tree
        actions.Enqueue(() => {
            if (item != null) {
                GroundSpace previousGroundSpace = previousTargetLoc.GetComponent<GroundSpace>();

                if (previousGroundSpace.hardMarked) {

                    previousGroundSpace.hardMarked = false;
                    previousGroundSpace.ChangeCurrentObject(PopItem());
                }
            }
            CurrentAction = null;
        });
    }

    public void ChangeAutoAction(ActionType actionType) {
        autoActionType = actionType;

        if (actionType == ActionType.Manual) {
            auto = false;
            AutoAction = null;
        } else {
            auto = true;

            if (actionType == ActionType.Plant) {
                AutoAction = () => AddPlantTree();
            } else if (actionType == ActionType.Leaf) {
                shouldChopWood = false;
                AutoAction = () => AddChopTree();
            } else if (actionType == ActionType.Wood) {
                shouldChopWood = true;
                AutoAction = () => AddChopTree();
            }
        }
    }

    // apply ChangeAutoAction to all characters of the same type

    public void CommunalChangeAutoAction(ActionType actionType) {
        foreach (CharacterControl characterControl in equalTypeCharacters) {
            characterControl.ChangeAutoAction(actionType);
        }
    }

    public ActionType GetAutoActionType() {
        return autoActionType;
	}

    public void AddItem(GameObject item) {
        if (item != null) {
            this.item = item;
            itemBubbleIcon.sprite = item.GetComponent<SpriteRenderer>().sprite;
            itemBubble.SetActive(true);
        }
    }

    public bool HasItem() {
        return item != null;
    }

    // removes the item and returns it 
    public GameObject PopItem() {
        GameObject temp = item;
        item = null;
        itemBubble.SetActive(false);
        return temp;
    }

    public void ToggleAuto() {
        auto = !auto;
	}

    public bool InAutoMode() {
        return auto;
	}
}

public enum Direction {
    FrontLeft,
    FrontRight,
    BackLeft,
    BackRight
}

public enum ActionType {
    Plant,
    Leaf,
    Wood,
    Manual
}

public enum CharacterType {
    Jerry,
    Rachel,
    Rick
}