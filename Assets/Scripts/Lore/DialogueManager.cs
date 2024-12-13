using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {
    public Text nameText;
    public Text dialogueText;

    public Animator boxAnimator;
    public Animator nameAnimator;

    private Queue<string> _sentences;
    public bool enemy = false;
    public GameObject button;
    public GameObject dialogueTextBox;
    public GameObject detectors;
    public GameObject redScreen;
    public Animator redScreenAnimator;

    public GrabArm grabArm;
    
    public bool redScreenIsOpen = false;
    public bool startedSearching = false;

    private void Start() {
        _sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue) {
        boxAnimator.SetBool("IsOpen", true);
        dialogueTextBox.SetActive(true);
        nameText.text = dialogue.name;
        nameAnimator.SetBool("IsOpen", true);
        Debug.Log("IsTalking");
        _sentences.Clear();

        foreach (string sentence in dialogue.sentences)
            _sentences.Enqueue(sentence);

        DisplayNextSentence();
    }

    public void DisplayNextSentence() {
        if (_sentences.Count == 0) {
            if (enemy) {
                FindObjectOfType<GrabArm>().StartSearching();
                enemy = false;
                Debug.Log("Dialogue1 - EndDialogue function fired");
            }
            else if (startedSearching == true) {
                Debug.Log("Dialogue2 - EndDialogue function fired");
                FindObjectOfType<EnemyDialogueTrigger>().TellToRun();
                return;
            }
            EndDialogue();
            CheckForFloatingStuff();
            return;
        }
        if (_sentences.Count < 1)
            CheckForFloatingStuff();

        string sentence = _sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence, 0.07f));
    }

    private IEnumerator TypeSentence(string sentence, float time) {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray()) {
            dialogueText.text += letter;
            yield return new WaitForSeconds(time);
        }
    }

    private void EndDialogue() {
        boxAnimator.SetBool("IsOpen", false);
        nameAnimator.SetBool("IsOpen", false);
        dialogueText.text = "";
        button.SetActive(false);
        detectors.SetActive(false);
    }

    private void CheckForFloatingStuff() {
        if (button.activeSelf == false)
            dialogueTextBox.SetActive(false);
    }
}
