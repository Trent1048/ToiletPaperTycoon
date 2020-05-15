﻿using UnityEngine;

public class GroundSpace : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Color startingColor;
    private Color hoverColor;

    private GameObject currentObject;
    public GameObject[] objects;

    private void Start() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        startingColor = spriteRenderer.color;
        hoverColor = new Color(0f, 0f, 0f, 0.5f);

        // initial tree generation
        if (Random.Range(0, 3) == 0) {
            int treeType = Random.Range(0, objects.Length);
            ChangeCurrentObject(objects[treeType]);
        }
    }

    public void ChangeCurrentObject(GameObject newObject) {
        if (currentObject != null) {
            Destroy(currentObject);
        }
        if (newObject != null) {
            // makes sure the object is not a box or that a box can spawn if it is
            bool currentObjectIsBox = newObject.GetComponent<BoxController>() != null;
            if (!currentObjectIsBox || (currentObjectIsBox && GameController.instance.BoxCanSpawn())) {
                currentObject = Instantiate(newObject, transform);
            } else {
                currentObject = null;
            }
        } else {
            currentObject = null;
        }
    }

    public GameObject GetCurrentObject() {
        return currentObject;
    }

    public GameObject Interact() {
        TreeController treeControl = currentObject.GetComponent<TreeController>();
        if (treeControl != null) {
            return treeControl.PickLeaf();
        }
        return null;
    }

    private void OnMouseEnter() {
        if (!GameController.instance.GameIsPaused()) {
            spriteRenderer.color = hoverColor;
        }
    }

    private void OnMouseExit() {
        spriteRenderer.color = startingColor;
    }

    private void OnMouseDown() {
        if (!GameController.instance.GameIsPaused()) {
            if (GameController.instance.GetSelectedObject() != null) {
                if (currentObject == null) {
                    ChangeCurrentObject(GameController.instance.GetSelectedObject());
                } else {
                    ChangeCurrentObject(null);
                }
            }
            GameController.instance.ChangeSelectedSpace(gameObject);
        }
    }
   
}
