﻿using UnityEngine;
using UnityEngine.UI;

public class ItemSelectionController : MonoBehaviour {

    public Image itemIcon;
    public GameObject itemSelectionMenu;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ToggleMenu();
        }
    }

    public void ChangeIcon(Sprite sprite, Vector3 scale) {
        itemIcon.sprite = sprite;
        itemIcon.transform.localScale = scale;
    }

    public void ToggleMenu() {
        itemSelectionMenu.SetActive(!itemSelectionMenu.activeInHierarchy);
        GameController.instance.TogglePause();
    }
}