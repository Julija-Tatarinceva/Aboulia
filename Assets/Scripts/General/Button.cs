using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public GameObject button;
    public GameObject Vstuplenye;
    public AudioSource myFx;
    public AudioClip hoverSound;

    public void HoverSound()
    {
        myFx.PlayOneShot(hoverSound);
    }

    public void TurnOnButton()
    {
        button.SetActive(true);
    }
    public void Retry()
    {
        // SceneManager.LoadScene("TheStart");
        // Vstuplenye.SetActive(false);
    }
}
