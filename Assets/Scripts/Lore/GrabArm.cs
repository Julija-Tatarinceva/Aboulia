using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabArm : MonoBehaviour
{
    public GameObject arm_General;
    public GameObject arm_Functional;
    public GameObject ArmParent;
    public GameObject Place;
    public GameObject redScreen;
    public GameObject ceilingCheck;
    public GameObject Player;

    public Transform arm_FunctionalParent;

    public Collider2D colliderForDetecting;

    public Animator animator;
    public Animator playerAnimator;
    public Animator redScreenAnimator;

    void Update() {
        if (FindObjectOfType<DialogueManager>().startedSearching&& playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("run") && !FindObjectOfType<GameOverLauncher>().gameIsOver) {
            redScreen.SetActive(true);
            redScreenAnimator.Play("flashingRedScreen");
            arm_General.SetActive(false);
            StartDisposalProcedure(); //it is supposed to start dialogue
        }
    }

    public void StartSearching() {
        FindObjectOfType<DialogueManager>().startedSearching = true;
        animator.Play("SearchForPlayer");
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            ArmParent.transform.position = Place.transform.position;
            animator.SetBool("Found", true);
            Invoke("MergeObjects", 1f);
        }
    }
    void MergeObjects() {
        FixedJoint2D joint = arm_Functional.GetComponent<FixedJoint2D>();
        joint.anchor = ceilingCheck.transform.position;
        joint.connectedBody = Player.transform.GetComponentInParent<Rigidbody2D>();
        joint.enableCollision = false;
        Player.transform.SetParent(arm_FunctionalParent);
        Debug.Log("Anchor created at " + ceilingCheck.transform.position.x + " " + ceilingCheck.transform.position.y);
    }
    void StartDisposalProcedure() {
        FindObjectOfType<EnemyDialogueTrigger>().SayAboutDisposal();
        Invoke("DisableRedScreen", 2);
    }
    void DisableRedScreen() {
        redScreen.SetActive(false);
    }
}