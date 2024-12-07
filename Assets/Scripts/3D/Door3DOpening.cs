using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door3DOpening : MonoBehaviour {
    private static readonly int PlayerIsNear = Animator.StringToHash("playerIsNear");
    public InstructionsText instructionsText;
    public LevelManager levelManager;
    public Animator doorFragAnim;
    private bool _playerNear;
    private bool _exitingLevel = false; // Needs to stop the door from trying to disable components when switching scenes (it crashes the game)

    void Start() {
        levelManager = FindObjectOfType<LevelManager>();
    }
    void Update() {
        if (!_playerNear || !Input.GetKeyDown(levelManager.interactButton.ToLower())) return;
        _exitingLevel = true;
        levelManager.SaveLevel();
        levelManager.LoadNextLevel();
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (levelManager.switchesPressed == 2 && coll.CompareTag("Player")) {
            _playerNear = true;
            doorFragAnim.SetBool(PlayerIsNear, true);
            instructionsText.SetActive();
        }
    }
    void OnTriggerExit2D(Collider2D coll) {
        if(levelManager.switchesPressed == 2 && coll.CompareTag("Player") && !_exitingLevel) {
            _playerNear = false;
            doorFragAnim.SetBool(PlayerIsNear, false);
            instructionsText.SetInactive();
        }
    }
}
