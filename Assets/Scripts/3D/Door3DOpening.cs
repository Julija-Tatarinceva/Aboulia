using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door3DOpening : MonoBehaviour {
    private static readonly int PlayerIsNear = Animator.StringToHash("playerIsNear");
    public InstructionsText instructionsText;
    public Animator doorFragAnim;
    bool _playerNear;

    void Update()
    {
        if(LevelManager.SwitchesPressed == 2) {
            // doorFragAnim.Play("3DDoorUnlockedKeyboard");
        }
        if(_playerNear && Input.GetButtonDown("Interact")) {
            LevelManager.LoadNextLevel();
        }
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (LevelManager.SwitchesPressed == 2 && coll.CompareTag("Player")) {
            _playerNear = true;
            doorFragAnim.SetBool(PlayerIsNear, true);
            instructionsText.SetActive();
        }
    }
    void OnTriggerExit2D(Collider2D coll) {
        if(LevelManager.SwitchesPressed == 2 && coll.CompareTag("Player")) {
            _playerNear = false;
            doorFragAnim.SetBool(PlayerIsNear, false);
            instructionsText.SetInactive();
        }
    }
}
