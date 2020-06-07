using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public string tooltipText;

	private void Start() {
		tooltipText = tooltipText.Replace("\\n", "\n");
	}

	public void OnPointerExit(PointerEventData eventData) {
		Tooltip.HideTooltip();
	}

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
		Tooltip.ShowTooltip(tooltipText);
	}
}
