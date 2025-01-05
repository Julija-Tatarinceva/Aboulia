using General;
using UnityEngine;

namespace _3D
{
    public class Door3DOpening : MonoBehaviour {
        private static readonly int PlayerIsNear = Animator.StringToHash("playerIsNear");
        public InstructionsText instructionsText;
        public LevelManager levelManager;
        public Animator doorFragAnim;
        private bool _playerNear;
        private bool _exitingLevel = false; // Needs to stop the door from trying to disable components when switching scenes (it crashes the game)

        private void Start() {
            levelManager = FindObjectOfType<LevelManager>();
        }

        private void Update() { // LM_F03
            if (!_playerNear || !Input.GetKeyDown(levelManager.interactButton.ToLower())) 
                return;
            _exitingLevel = true;
            levelManager.SaveLevel();
            levelManager.LoadNextLevel();
        }

        private void OnTriggerEnter2D(Collider2D coll) { // LM_F03
            if (levelManager.switchesPressed == 2 && coll.CompareTag("Player")) {
                _playerNear = true;
                doorFragAnim.SetBool(PlayerIsNear, true);
                instructionsText.SetActive();
            }
        }

        private void OnTriggerExit2D(Collider2D coll) { // LM_F03
            if(levelManager.switchesPressed == 2 && coll.CompareTag("Player") && !_exitingLevel) {
                _playerNear = false;
                doorFragAnim.SetBool(PlayerIsNear, false);
                instructionsText.SetInactive();
            }
        }
    }
}
