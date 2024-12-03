using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue1;

    public void Say() {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue1);
    }
}
