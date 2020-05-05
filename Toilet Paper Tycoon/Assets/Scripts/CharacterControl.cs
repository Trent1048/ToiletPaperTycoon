using System;
using UnityEngine;

public class CharacterControl : MonoBehaviour {

    public float speed;
    public float accuracy;

    private GameObject targetLoc;

    private Animator animator;
    private Direction dir;

    private void Start() {
        animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate() {
        if (targetLoc != null) {
            Move(targetLoc.transform);
        }
    }

    public void UpdateTarget(GameObject targetLoc) {
        this.targetLoc = targetLoc;
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
            targetLoc = null;
        }
    }
}

public enum Direction {
    FrontLeft,
    FrontRight,
    BackLeft,
    BackRight
}