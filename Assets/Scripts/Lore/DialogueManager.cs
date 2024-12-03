using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;

    public Animator boxAnimator;
    public Animator nameAnimator;

    private Queue<string> sentences;

    public GameObject button;
    public GameObject dialogueTextBox;
    public GameObject Detectors;

    void Start() {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue) {
        boxAnimator.SetBool("IsOpen", true);
        dialogueTextBox.SetActive(true);
        nameText.text = dialogue.name;
        nameAnimator.SetBool("IsOpen", true);
        Debug.Log("IsTalking");
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
            sentences.Enqueue(sentence);

        DisplayNextSentence();
    }

    public void DisplayNextSentence() {
        if (sentences.Count == 0) {
            EndDialogue();
            CheckForFloatingStuff();
            return;
        }
        if (sentences.Count < 1)
            CheckForFloatingStuff();

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence, 0.07f));
    }

    IEnumerator TypeSentence(string sentence, float time) {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray()) {
            dialogueText.text += letter;
            yield return new WaitForSeconds(time);
        }
    }

    void EndDialogue() {
        boxAnimator.SetBool("IsOpen", false);
        nameAnimator.SetBool("IsOpen", false);
        dialogueText.text = "";
        button.SetActive(false);
        Detectors.SetActive(false);
    }
    void CheckForFloatingStuff() {
        if (button.activeSelf == false)
            dialogueTextBox.SetActive(false);
    }
}
