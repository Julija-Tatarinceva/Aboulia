using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour {
    public GameObject pauseMenuUI;
    public GameObject deathMenu;
    public GameObject levelFailedMenu;
    public GameObject player;
    public static bool GameIsPaused = false;

    private void Update() { // MM_F01
        if (!Input.GetButtonDown("Cancel")) 
            return;
        if (GameIsPaused)
            Resume();
        else
            OpenMenu("Pause");
    }

    public void Resume() { // MM_F03
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    
    public void OpenMenu(string menuName) { // MM_F02
        switch (menuName) {
            case "Death":
                deathMenu.SetActive(true);
                break;
            case "Level failed":
                levelFailedMenu.SetActive(true);
                break;
            case "Pause":
                pauseMenuUI.SetActive(true);
                GameIsPaused = true;
                break;
            case "Main menu":
                SceneManager.LoadScene("MainMenu");
                Time.timeScale = 1f;
                return;
        }
        Time.timeScale = 0f;
    }
    
    public void Restart() { // MM_F04
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void Respawn() { // MM_F05
        deathMenu.SetActive(false);
        Time.timeScale = 1f;
        player.GetComponent<PlayerMovement2D>().Respawn();
    }

    public void Exit() { // MM_F06
        Application.Quit();
    }
}
