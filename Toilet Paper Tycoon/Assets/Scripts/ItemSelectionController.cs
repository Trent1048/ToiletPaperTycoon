using UnityEngine;
using UnityEngine.UI;

public class ItemSelectionController : MonoBehaviour {

    public Image itemIcon;
    public GameObject itemSelectionMenu;

    public void ChangeIcon(Sprite sprite, Vector3 scale) {
        itemIcon.sprite = sprite;
        itemIcon.transform.localScale = scale;
    }

    public void ToggleMenu() {
        itemSelectionMenu.SetActive(!itemSelectionMenu.active);
        GameController.instance.TogglePause();
    }
}
