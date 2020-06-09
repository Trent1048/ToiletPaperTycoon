using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

    protected static Tooltip instance;

	protected Text text;
	protected RectTransform background;
	protected bool activated;

	private void Awake() {
		Transform backgroundObject = transform.Find("Background");
		background = backgroundObject.GetComponent<RectTransform>();
		text = backgroundObject.Find("Text").GetComponent<Text>();
		Deactivate();

		if (instance == null) {
			instance = this;
		} else {
			Debug.LogError("Cannot have more than one tooltip");
		}
	}

	private void Update() {
		if (activated) {
			transform.position = Input.mousePosition;
		}
	}

	protected void Activate() {
		activated = true;
		text.gameObject.SetActive(true);
		background.gameObject.SetActive(true);
	}

	protected void Deactivate() {
		activated = false;
		text.gameObject.SetActive(false);
		background.gameObject.SetActive(false);
	}

	public static void ShowTooltip(string tooltipString) {
		instance.Activate();

		instance.text.text = tooltipString;

		float textPadding = 4f;
		Vector2 backgroundSize = new Vector2(instance.text.preferredWidth + (2f * textPadding), instance.text.preferredHeight + (2f * textPadding));
		instance.background.sizeDelta = backgroundSize;
	}

	public static void HideTooltip() {
		instance.Deactivate();
	}
}
