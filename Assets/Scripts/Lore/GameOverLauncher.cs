using UnityEngine;
using UnityEngine.Serialization;

public class GameOverLauncher : MonoBehaviour {
    public GameObject gameOverScreen;
    public GameObject retryButton;
    public bool gameIsOver;
    public void GameOver() {
        gameOverScreen.SetActive(true);
        gameIsOver = true;
    }
    public void TurnOnRetry() {
        retryButton.SetActive(true);
    }
}
