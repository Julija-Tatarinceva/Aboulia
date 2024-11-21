using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject deathMenu;
    public GameObject levelFailedMenu;
    public GameObject Spawn;
    public GameObject Spawn3D;
    public GameObject Player;
    public GameObject Player3D;
    public Animator Animator;
    public static bool gameIsPaused = false;

    void Update() {
        if (Input.GetButtonDown("Cancel")) {
            if (gameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    void Pause() {
        pauseMenuUI.SetActive(true);
        gameIsPaused = true;
        Time.timeScale = 0f;
    }

    public void MainMenu() {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }

    public void DeathMenuActive() {
        deathMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    // public void Respawn()
    // {
    //     if (FindObjectOfType<LevelManager>().levelIncludes3D == true)
    //     {
    //         if (Player3D.activeSelf)
    //         {
    //             FindObjectOfType<MController3D>().isGrounded = true;
    //             FindObjectOfType<MController3D>().dead = false;
    //             FindObjectOfType<Runestone>().Teleport();
    //             Player3D.transform.position = Spawn3D.transform.position;
    //         }
    //     }
    //     FindObjectOfType<LevelManager>().NumberOfLives(false);
    //     Player.transform.position = Spawn.transform.position;
    //     deathMenu.SetActive(false);
    //     Time.timeScale = 1f;
    //     Animator.Play("Idle");
    // }

    public void Exit() {
        Debug.Log("Quit");
    }

    public void LevelFailed() {
        levelFailedMenu.SetActive(true);
        Time.timeScale = 0f;
    }
}
