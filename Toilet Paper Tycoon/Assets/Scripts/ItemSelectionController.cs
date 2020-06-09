using UnityEngine;
using UnityEngine.UI;

public class ItemSelectionController : MonoBehaviour {

    public Image itemIcon;
    public GameObject itemSelectionMenu;
    public GameObject[] characterOptions;

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

    public void DisableAllCharacterOptions(GameObject ignore = null) {
        foreach (GameObject option in characterOptions) {
            if (option != ignore) {
                option.SetActive(false);
            }
		}
	}
}
