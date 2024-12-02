using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class IntroductionInstructions : MonoBehaviour {
    private List<string[]> _instructionsAll = new List<string[]>();
    public List<GameObject> instructionsPanels = new List<GameObject>();
    public GameObject tutorialPanel;
    public string[] instructionsEnglish = new[] {
        "Click anywhere to continue...",
        "Move around using A and D (or the arrow keys) and Space",
        "Press all the buttons you can find", 
        "Then leave through the door"
    };
    public string[] instructionsLatvian = new[] {
        "Click anywhere to continue...",
        "Pārvietojieties, izmantojot A un D (vai bulttaustiņus) un Space",
        "Nospiediet visas pogas, kuras varat atrast",
        "Tad izejiet caur durvīm"
    };

    public string[] instructionsRussian = new[] {
        "Кликните в любом месте чтобы продолжить...",
        "Передвигайтесь, используя A и D (или стрелки) и Пробел",
        "Нажимайте на все кнопки, которые сможете найти",
        "Затем выходите через дверь"
    };

    public TextMeshProUGUI textToContinue;
    // Start is called before the first frame update
    void Start() {
        if (!PlayerPrefs.HasKey("Completed2DTutorial")){
            tutorialPanel.SetActive(true);
            _instructionsAll.Add(instructionsEnglish);
            _instructionsAll.Add(instructionsLatvian);
            _instructionsAll.Add(instructionsRussian);
            textToContinue.text = _instructionsAll[LevelManager.Language][0];
            ShowTutorial(_instructionsAll[LevelManager.Language], 0, 1);
            PlayerPrefs.SetInt("Completed2DTutorial", 1);
        }
    }

    public void ShowTutorial(string[] instructions, int panelIndex, int instructionIndex) {
        // Base case: If we are out of instructions, end the tutorial
        if (instructionIndex >= instructions.Length) {
            tutorialPanel.SetActive(false);
            Debug.Log("Tutorial Finished");
            return;
        }

        // Activate the current panel
        instructionsPanels[panelIndex].SetActive(true);
        instructionsPanels[panelIndex].GetComponent<TextMeshProUGUI>().text = instructions[instructionIndex];
        
        // Wait for left mouse click to proceed to the next step
        StartCoroutine(WaitForClick(() =>
        {
            // Progress to the next instruction
            instructionsPanels[panelIndex].SetActive(false);
            ShowTutorial(instructions, panelIndex +1, instructionIndex + 1);
        }));
    }

    private IEnumerator WaitForClick(System.Action onClick) {
        // Wait until the left mouse button is clicked
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        // Add a small delay to prevent rapid clicks
        yield return new WaitForSeconds(.1f);
        // Invoke the action to proceed
        onClick();
    }
}
