using UnityEngine;
using UnityEngine.UI;

public class TipsController : MonoBehaviour {

	public string[] instructions;
	public Text instructionText;
	private int instructionNum;

	private void Start() {
		Time.timeScale = 0;
		instructionNum = -1;
	}

	private void Update() {
		if (Input.GetMouseButtonDown(0)) {
			GameController.instance.PlayClickNoise();
			instructionNum++;
			if (instructionNum < instructions.Length) {
				instructionText.text = instructions[instructionNum];
			} else {
				Time.timeScale = 1;
				gameObject.SetActive(false);
			}
		}
	}
}
