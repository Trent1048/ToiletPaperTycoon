using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject pauseMenu;

    public void Play() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

    public void Menu() {
        SceneManager.LoadScene(0);
    }

    public void Quit() {
        Debug.Log("Quit");
        Application.Quit();
	}

    public void TogglePauseMenu() {
        pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
	}
}
