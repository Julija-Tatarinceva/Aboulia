using System.Collections;
using System.Collections.Generic;
using UnityEngine;
  public class SwitchScript : MonoBehaviour
{
    public LevelManager levelManager;
    public Door3DOpening DoorOpening;
    // public GameObject ;
    public InstructionsText textInstructions;
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
        if (other.CompareTag("Player")) {
            playerIsClose = true;
            textInstructions.SetActive();
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerIsClose = false;
            textInstructions.SetInactive();
        }
    }
}
