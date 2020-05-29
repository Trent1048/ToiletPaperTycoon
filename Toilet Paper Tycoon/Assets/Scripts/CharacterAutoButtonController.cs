using UnityEngine;

public class CharacterAutoButtonController : MonoBehaviour {

    public CharacterControl characterControl;

	public void ToggleAuto() {
		characterControl.ToggleAuto();
	}
}
