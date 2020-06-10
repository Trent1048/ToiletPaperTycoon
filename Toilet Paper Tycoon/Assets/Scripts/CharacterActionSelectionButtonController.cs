using UnityEngine;

public class CharacterActionSelectionButtonController : MonoBehaviour {

	public CharacterControl characterControl;
	public ActionType actionType;

	public void ChangeAction() {
		characterControl.CommunalChangeAutoAction(actionType);
	}

}
