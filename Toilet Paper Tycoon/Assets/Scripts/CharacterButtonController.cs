using UnityEditor;
using UnityEngine;

public class CharacterButtonController : MonoBehaviour {

	public CharacterControl characterControl;
	public GameObject optionsMenu;
	public ItemSelectionController selectionController;

	private void OnDisable() {
		optionsMenu.SetActive(false);
	}

	public void ToggleOptions() {
		selectionController.DisableAllCharacterOptions();
		optionsMenu.SetActive(!optionsMenu.activeInHierarchy);
	}
}
