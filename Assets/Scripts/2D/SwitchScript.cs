using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;

public class SwitchScript : MonoBehaviour {
    [FormerlySerializedAs("DoorOpening")] public Door3DOpening doorOpening;
    public InstructionsText textInstructions;
    public LevelManager levelManager;
    public bool playerIsClose = false;
    public Sprite newSprite;
    public SpriteRenderer spriteRenderer;
    public Collider2D box;

    void Start() {
        levelManager = FindObjectOfType<LevelManager>();
    }
    void Update() {
        if (playerIsClose && Input.GetKeyDown(levelManager.interactButton.ToLower())) {
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
