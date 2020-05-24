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

    private Action currentAction;
    private Queue<Action> actions;

    private Animator animator;
    private Direction dir;

    private void Start() {
        animator = GetComponent<Animator>();
        actions = new Queue<Action>();

        itemBubble = transform.GetChild(0).gameObject;
        itemBubbleIcon = itemBubble.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate() {
        if (currentAction != null) {
            currentAction();
		} else if (actions.Count > 0) {
            currentAction = actions.Dequeue();
		}
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
            currentAction = null;
            previousTargetLoc = target;
        }
    }

    // moves the character to a tree, picks some leaves, and then to the box if it exists
    public void AddMoveToTree() {

        actions.Enqueue(() => {
            GroundSpace previousSpace = null;
            if (previousTargetLoc != null) {
                previousSpace = previousTargetLoc.GetComponent<GroundSpace>();
            }
            GroundSpace treeLoc = GameController.instance.FindObjectInGround(previousSpace, "Tree");
            if (treeLoc != null) {
                currentAction = () => Move(treeLoc.transform);   
            } else {
                currentAction = null;
			}
        });

        actions.Enqueue(() => {
            GameObject newItem = previousTargetLoc.GetComponent<GroundSpace>().Interact();
            if (newItem != null) {
                AddItem(newItem);
            }
            currentAction = null;
        });

        actions.Enqueue(() => {
            GroundSpace boxLoc = GameController.instance.FindObjectInGround(null, "Box");
            if (boxLoc != null) {
                currentAction = () => Move(boxLoc.transform);
            } else {
                currentAction = null;
			}
        });
    }

    public void AddItem(GameObject item) {
        this.item = item;
        itemBubbleIcon.sprite = item.GetComponent<SpriteRenderer>().sprite;
        itemBubble.SetActive(true);
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
}

public enum Direction {
    FrontLeft,
    FrontRight,
    BackLeft,
    BackRight
}