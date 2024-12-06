using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour {
    public GameObject pauseMenuUI;
    public GameObject deathMenu;
    public GameObject levelFailedMenu;
    public GameObject player;
    public static bool GameIsPaused = false;

    void Update() {
        if (Input.GetButtonDown("Cancel")) {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause() {
        pauseMenuUI.SetActive(true);
        GameIsPaused = true;
        Time.timeScale = 0f;
    }

    public void MainMenu() {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }

    public void SetDeathMenuActive() {
        deathMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void Respawn() {
        deathMenu.SetActive(false);
        Time.timeScale = 1f;
        player.GetComponent<PlayerMovement2D>().Respawn();
    }

    public void Exit() {
        Application.Quit();
    }

    public void LevelFailed() {
        levelFailedMenu.SetActive(true);
        Time.timeScale = 0f;
    }
}
