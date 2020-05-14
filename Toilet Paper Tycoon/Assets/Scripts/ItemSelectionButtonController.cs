using UnityEngine;
using UnityEngine.UI;

public class ItemSelectionButtonController : MonoBehaviour {

	public GameObject item;
	public ItemSelectionController selectionController;
	private Sprite itemIcon;
	private Vector3 scale;

	private void Start() {
		Transform icon = transform.GetChild(0).GetChild(0);
		itemIcon = icon.GetComponent<Image>().sprite;
		scale = icon.localScale;
	}

	public Sprite GetItemIcon() {
		return itemIcon;
	}

	public void SelectItem() {
		GameController.instance.ChangeSelectedObject(item);
		selectionController.ChangeIcon(itemIcon, scale);
	}
}
