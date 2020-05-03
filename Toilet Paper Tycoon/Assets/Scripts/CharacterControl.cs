using System;
using UnityEngine;

public class CharacterControl : MonoBehaviour {

    public float speed;
    private Animator animator;
    private Direction dir;

    private void Start() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        // get input
        float xIn = Input.GetAxis("Horizontal");
        float yIn = Input.GetAxis("Vertical");

        // keep the player moving on only one isometric axis
        if (Math.Abs(xIn) < Math.Abs(yIn)) {
            xIn = 0;
            if (yIn < 0) dir = Direction.FrontRight;
            else if (yIn > 0) dir = Direction.BackLeft;
        } else {
            yIn = 0;
            if (xIn < 0) dir = Direction.FrontLeft;
            else if (xIn > 0) dir = Direction.BackRight;   
        }

        animator.SetInteger("Direction", (int)dir);

        xIn *= Time.deltaTime * speed;
        yIn *= Time.deltaTime * speed;

        // deal with isometric movement
        float xMove = (xIn - yIn) * 0.5f;
        float yMove = (xIn + yIn) * 0.25f;

        transform.Translate(xMove, yMove, 0);
    }
}

public enum Direction {
    FrontLeft,
    FrontRight,
    BackLeft,
    BackRight
}