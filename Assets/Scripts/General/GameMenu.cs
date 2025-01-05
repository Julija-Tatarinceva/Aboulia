using _2D;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace General
{
    public class GameMenu : MonoBehaviour {
        public GameObject pauseMenuUI;
        public GameObject deathMenu;
        public GameObject levelFailedMenu;
        public GameObject player;
        public RespawnAnimation arm;
        private bool GameIsPaused = false;
        private bool GameIsStopped = false;

        private void Update() { // MM_F01
            if (!Input.GetButtonDown("Cancel")) 
                return;
            if (GameIsPaused)
                Resume();
            else if(!GameIsStopped)
                OpenMenu("Pause");
        }

        public void Resume() { // MM_F03
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
        }
    
        public void OpenMenu(string menuName) { // MM_F01
            switch (menuName) {
                case "Death":
                    deathMenu.SetActive(true);
                    GameIsStopped = true;
                    break;
                case "Level failed":
                    levelFailedMenu.SetActive(true);
                    GameIsStopped = true;
                    break;
                case "Pause":
                    pauseMenuUI.SetActive(true);
                    GameIsPaused = true;
                    break;
                case "Main menu":
                    SceneManager.LoadScene("MainMenu");
                    Time.timeScale = 1f;
                    return;
                default:
                    Warning.ShowWarning("No valid menu selected");
                    break;
            }
            Time.timeScale = 0f;
        }
    
        public static void Restart() { // MM_F04
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Time.timeScale = 1f;
        }

        public void Respawn() { // MM_F05
            deathMenu.SetActive(false);
            Time.timeScale = 1f;
            arm.DragPlayerToSpawn();
            GameIsStopped = false;
        }

        public static void Exit() { // MM_F06
            Application.Quit();
        }
    }
}
