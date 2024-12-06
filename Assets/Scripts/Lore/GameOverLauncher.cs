using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverLauncher : MonoBehaviour
{
    public GameObject GameOverScreen;
    //public GameObject Player;
    public GameObject retryButton;
    public bool gameIsOver;
    public void GameOver()
    {
        GameOverScreen.SetActive(true);
        gameIsOver = true;
    }
    public void TurnOnRetry()
    {
        retryButton.SetActive(true);
    }
}
