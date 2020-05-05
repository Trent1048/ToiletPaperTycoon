using UnityEngine;
using UnityEngine.UI;

public class SelectedCharacterDisplay : MonoBehaviour {

    public Sprite[] characterImages;
    private Image characterImage;

    private void Start() {
        characterImage = GetComponent<Image>();
    }

    private void Update() {
        characterImage.sprite = characterImages[(int)GameController.instance.GetSelectedCharacterType()];
    }
}
