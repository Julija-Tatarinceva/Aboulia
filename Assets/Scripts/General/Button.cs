using UnityEngine;

public class Button : MonoBehaviour {
    public GameObject button;
    public AudioSource myFx;
    public AudioClip hoverSound;

    public void HoverSound() {
        myFx.PlayOneShot(hoverSound);
    }

    public void TurnOnButton() {
        button.SetActive(true);
    }
}
