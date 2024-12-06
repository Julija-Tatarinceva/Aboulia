using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDialogueTrigger : MonoBehaviour
{
  public Collider2D Collider;
    public GameObject Cat;
    public GameObject Saw;
    public GameObject Restriction;
    public Dialogue dialogue1;
    public Dialogue dialogue2;
    public Dialogue dialogue3;
    public DialogueManager dialogueManager;

    public void Say()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue1);
        FindObjectOfType<DialogueManager>().Enemy = true;
    }
    public void SayAboutDisposal()
    {
        FindObjectOfType<DialogueManager>().redScreenIsOpen = true;
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue2);
    }
    public void TellToRun()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue3);
        Saw.SetActive(true);
        Restriction.SetActive(false);
    }
}
