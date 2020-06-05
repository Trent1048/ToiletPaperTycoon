using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour {

    public float speed;
    public float accuracy;

    private Transform previousTargetLoc;

    private GameObject item;
    private GameObject itemBubble;
    private SpriteRenderer itemBubbleIcon;

    private Action CurrentAction;
    private Action AutoAction;
    private Queue<Action> actions;
    private bool shouldChopWood = false;
    private bool auto;

    private Animator animator;
    private Direction dir;

    private void Start() {
        animator = GetComponent<Animator>();
        actions = new Queue<Action>();

        itemBubble = transform.GetChild(0).gameObject;
        itemBubbleIcon = itemBubble.transform.GetChild(0).GetComponent<SpriteRenderer>();
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
        GroundSpace findStart = null;

        if (previousTargetLoc != null) {
            findStart = previousTargetLoc.GetComponent<GroundSpace>();
        }

        GroundSpace boxLoc = GameController.instance.FindObjectInGround(findStart, "Belt");
        if (boxLoc != null) {
            // move to the box
            CurrentAction = () => Move(boxLoc.transform);
        }

        /*
        // wait for the box to exist
        if (!GameController.instance.BoxCanSpawn()) {
            GroundSpace boxLoc = GameController.instance.FindObjectInGround(null, "Box");
            if (boxLoc != null) {
                // move to the box
                CurrentAction = () => Move(boxLoc.transform);
            }
        }
        */
    }

    // the target should always be a ground tile
    public void AddMove(GameObject targetLoc) {
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
    public void AddChopTree() {

        // go to the tree
        actions.Enqueue(() => {
            if (item == null) {
                GroundSpace previousSpace = null;
                if (previousTargetLoc != null) {
                    previousSpace = previousTargetLoc.GetComponent<GroundSpace>();
                }
                GroundSpace treeLoc = GameController.instance.FindAdultTree(previousSpace, true);
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

        // go to the box
        actions.Enqueue(() => GoToBox());

        // deposit the wood/leaves at the box
        actions.Enqueue(() => {

            previousTargetLoc.GetComponent<GroundSpace>().Deposit(PopItem());
            CurrentAction = null;

            /*
            GameObject box = GameController.instance.GetBox();
            // makes sure the box exists and the character is at the box's location
            if (box != null && box.transform.parent == previousTargetLoc) {
                previousTargetLoc.GetComponent<GroundSpace>().Deposit(PopItem());
            }
            CurrentAction = null;
            */
        });
    }

    public void AddPlantTree() {

        // go to the box
        actions.Enqueue(() => GoToBox());

        // get a tree from the box
        actions.Enqueue(() => {
            GameObject box = GameController.instance.GetBox();
            // makes sure the box exists and the character is at the box's location
            if (box != null && box.transform.parent == previousTargetLoc) {
                AddItem(box.GetComponent<BoxController>().GetTree());
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