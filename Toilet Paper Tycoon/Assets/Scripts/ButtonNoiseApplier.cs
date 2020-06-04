using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ButtonNoiseApplier : MonoBehaviour {

    public AudioSource clickSound;

	// loop through all button children and give them sounds
	private void Start() {
		Stack<Transform> children = new Stack<Transform>();

		for (int childNum = 0; childNum < transform.childCount; childNum++) {
			children.Push(transform.GetChild(childNum));
		}

		int count = 0;

		while (children.Count > 0 && count < 100) {
			count++;

			Transform currentChild = children.Pop();

			Button currentButton = currentChild.GetComponent<Button>();
			if (currentButton != null) {
				currentButton.onClick.AddListener(() => clickSound.Play());
			}

			for (int childNum = 0; childNum < currentChild.transform.childCount; childNum++) {
				children.Push(currentChild.transform.GetChild(childNum));
			}
		}
	}
}
