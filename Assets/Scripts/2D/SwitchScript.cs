using UnityEngine;
using UnityEngine.Serialization;

public class SwitchScript : MonoBehaviour {
    public Door3DOpening doorOpening;
    public InstructionsText textInstructions;
    public LevelManager levelManager;
    public bool playerIsClose = false;
    public Sprite newSprite;
    public SpriteRenderer spriteRenderer;
    public Collider2D box;
    public Material greenMaterial;

    private void Start() {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void Update() {
        if (playerIsClose && Input.GetKeyDown(levelManager.interactButton.ToLower())) {
            spriteRenderer.sprite = newSprite;
            levelManager.SwitchPressed();
            box.enabled = false;
            if(transform.GetComponent<TransitionablePair>())
                transform.GetComponent<TransitionablePair>().target.transform.Find("Button").GetComponent<Renderer>().material = greenMaterial;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerIsClose = true;
            textInstructions.SetActive();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerIsClose = false;
            textInstructions.SetInactive();
        }
    }
}
