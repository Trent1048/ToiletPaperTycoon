﻿using UnityEngine;

public class GroundSpace : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Color startingColor;
    private Color hoverColor;

    private GameObject currentObject;

    private void Start() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        startingColor = spriteRenderer.color;
        hoverColor = new Color(0f, 0f, 0f, 0.5f);
    }

    private void OnMouseEnter() {
        spriteRenderer.color = hoverColor;
    }

    private void OnMouseExit() {
        spriteRenderer.color = startingColor;
    }

    private void OnMouseDown() {
        if (GameController.instance.selectedObject != null) {
            if (currentObject == null) {
                currentObject = Instantiate(GameController.instance.selectedObject, transform);
            } else {
                Destroy(currentObject);
                currentObject = null;
            }
        }
        GameController.instance.ChangeSelectedSpace(gameObject);
    }
}
