using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace General
{
    public class IntroductionInstructions : MonoBehaviour {
        public List<GameObject> instructionsPanels = new List<GameObject>();
        public GameObject tutorialPanel;
    
        // Start is called before the first frame update
        private void Start() {
            Invoke(nameof(StartTutorial), 0.1f);
        }

        private void StartTutorial() { // TM_F04
            if (FindObjectOfType<LevelManager>().Done2DTutorial && SceneManager.GetActiveScene().buildIndex == 1){
                tutorialPanel.SetActive(true);
                ShowTutorial(0);
            }
            else if (FindObjectOfType<LevelManager>().Done3DTutorial && SceneManager.GetActiveScene().buildIndex == 2){
                tutorialPanel.SetActive(true);
                ShowTutorial(0);
            }
        }

        private void ShowTutorial(int panelIndex) { // TM_F05
            // Base case: If we are out of instructions, end the tutorial
            if (panelIndex >= instructionsPanels.Count) {
                tutorialPanel.SetActive(false);
                return;
            }

            // Activate the current panel
            instructionsPanels[panelIndex].SetActive(true);
        
            // Wait for left mouse click to proceed to the next step
            if (SceneManager.GetActiveScene().buildIndex == 1) {
                StartCoroutine(WaitForClick(() => {
                    // Progress to the next instruction
                    instructionsPanels[panelIndex].SetActive(false);
                    ShowTutorial(panelIndex +1);
                }));
            }
            // Wait for 'T' to proceed to the next step
            else if (SceneManager.GetActiveScene().buildIndex == 2 && panelIndex == 0) {
                StartCoroutine(WaitForKey(KeyCode.T, () => {
                    instructionsPanels[panelIndex].SetActive(false);
                    ShowTutorial(panelIndex + 1);
                }));
            }
            // Wait for 'Q' to proceed to the next step
            else {
                StartCoroutine(WaitForKey(KeyCode.Q, () => {
                    instructionsPanels[panelIndex].SetActive(false);
                    ShowTutorial(panelIndex + 1);
                }));
            }
        }

        #region Waiting for Key
        private IEnumerator WaitForClick(System.Action onClick) {
            // Wait until the left mouse button is clicked
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            // Add a small delay to prevent rapid clicks
            yield return new WaitForSeconds(.1f);
        
            // Invoke the action to proceed
            onClick();
        }
        private IEnumerator WaitForKey(KeyCode key, System.Action onKeyPress) {
            // Wait until the specified key is pressed
            yield return new WaitUntil(() => Input.GetKeyDown(key));
            // Add a small delay to prevent rapid input
            yield return new WaitForSeconds(.1f);
            // Invoke the action to proceed
            onKeyPress();
        }
        #endregion
    }
}
