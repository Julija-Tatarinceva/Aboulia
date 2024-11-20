using System.Collections;
using System.Collections.Generic;
using UnityEngine;
  public class SwitchScript : MonoBehaviour
{
    public LevelManager levelManager;
    public Door3DOpening DoorOpening;
    public GameObject textInstructions;
    public bool playerIsClose = false;
    public Sprite newSprite;
    public SpriteRenderer spriteRenderer;
    public Collider2D box;
    
    void Update() {
        if (playerIsClose && Input.GetButtonDown("Interact")) {
            spriteRenderer.sprite = newSprite;
            levelManager.switchesPressed += 1;
            box.enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            playerIsClose = true;
            textInstructions.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            playerIsClose = false;
            textInstructions.SetActive(false);
        }
    }
}
