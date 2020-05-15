using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour {

    public float speed;
    public float accuracy;

    private GameObject targetLoc;
    private GameObject previousTargetLoc;
    private Action DoAfterMove;

    private GameObject item;
    private GameObject itemBubble;
    private SpriteRenderer itemBubbleIcon;

    private ContactFilter2D contactFilter;
    private Animator animator;
    private Direction dir;

    private void Start() {
        animator = GetComponent<Animator>();
        contactFilter = new ContactFilter2D();

        itemBubble = transform.GetChild(0).gameObject;
        itemBubbleIcon = itemBubble.transform.GetChild(0).GetComponent<SpriteRenderer>();
        DoAfterMove = DoNothing;
    }

    private void FixedUpdate() {
        if (targetLoc != null) {
            Move(targetLoc.transform);
        }
    }

    public void UpdateTarget(GameObject targetLoc) {
        this.targetLoc = targetLoc;
    }

    // Actions

    private void DoNothing() {
        targetLoc = null;
    }

    private void TakeItem() {
        GroundSpace ground = targetLoc.GetComponent<GroundSpace>();
        if (ground != null) {
            AddItem(ground.Interact());
        }
    }

    // Movement

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
            previousTargetLoc = targetLoc;
            DoAfterMove();
        }
    }

    public void MoveToTree(float radius = 1f) {
        List<Collider2D> hitColliders = new List<Collider2D>();
        Physics2D.OverlapCircle(transform.position, radius, contactFilter, hitColliders);

        foreach (Collider2D col in hitColliders) {
            GroundSpace ground = col.GetComponent<GroundSpace>();
            if (ground != null && col.gameObject != previousTargetLoc) {
                GameObject groundCurrentObject = ground.GetCurrentObject();
                if (groundCurrentObject != null && groundCurrentObject.GetComponent<TreeController>() != null) {
                    UpdateTarget(col.gameObject);

                    // makes the player move to the box after it's done moving
                    DoAfterMove = () => {
                        TakeItem();
                        GameObject box = GameController.instance.GetBox();

                        if (box != null) {
                            UpdateTarget(box.transform.parent.gameObject);
                        } else {
                            UpdateTarget(null);
                        }

                        DoAfterMove = () => DoNothing();
                    };

                    return;
                }
            }
        }
        if (radius < 10f) {
            MoveToTree(radius * 2);
        }
    }

    // Item Stuff:

    public void AddItem(GameObject item) {
        this.item = item;
        itemBubbleIcon.sprite = item.GetComponent<SpriteRenderer>().sprite;
        itemBubble.SetActive(true);
    }

    public bool HasItem() {
        return item != null;
    }

    // removes the item and returns it 
    public GameObject GetItem() {
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