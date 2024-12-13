using UnityEngine;

public class EnemyDialogueTrigger : MonoBehaviour {
    public Collider2D collider;
    public GameObject cat;
    public GameObject saw;
    public GameObject restriction;
    public Dialogue dialogue1;
    public Dialogue dialogue2;
    public Dialogue dialogue3;
    public DialogueManager dialogueManager;

    public void Say()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue1);
        FindObjectOfType<DialogueManager>().enemy = true;
    }
    public void SayAboutDisposal()
    {
        FindObjectOfType<DialogueManager>().redScreenIsOpen = true;
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue2);
    }
    public void TellToRun()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue3);
        saw.SetActive(true);
        restriction.SetActive(false);
    }
}
