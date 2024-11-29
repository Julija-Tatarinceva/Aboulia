using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Button : MonoBehaviour
{
    public GameObject button;
    [FormerlySerializedAs("Vstuplenye")] public GameObject vstuplenye;
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
