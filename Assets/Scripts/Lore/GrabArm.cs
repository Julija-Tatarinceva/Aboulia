using UnityEngine;
using UnityEngine.Serialization;

public class GrabArm : MonoBehaviour
{
    public GameObject armGeneral;
    public GameObject armFunctional;
    public GameObject armParent;
    public GameObject place;
    public GameObject redScreen;
    public GameObject ceilingCheck;
    public GameObject player;
    public Transform armFunctionalParent;

    public Collider2D colliderForDetecting;

    public Animator animator;
    public Animator playerAnimator;
    public Animator redScreenAnimator;

    private void Update() {
        if (FindObjectOfType<DialogueManager>().startedSearching&& playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("run") && !FindObjectOfType<GameOverLauncher>().gameIsOver) {
            redScreen.SetActive(true);
            redScreenAnimator.Play("flashingRedScreen");
            armGeneral.SetActive(false);
            StartDisposalProcedure(); //it is supposed to start dialogue
        }
    }

    public void StartSearching() {
        FindObjectOfType<DialogueManager>().startedSearching = true;
        animator.Play("SearchForPlayer");
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            armParent.transform.position = place.transform.position;
            animator.SetBool("Found", true);
            Invoke("MergeObjects", 1f);
        }
    }

    private void MergeObjects() {
        FixedJoint2D joint = armFunctional.GetComponent<FixedJoint2D>();
        joint.anchor = ceilingCheck.transform.position;
        joint.connectedBody = player.transform.GetComponentInParent<Rigidbody2D>();
        joint.enableCollision = false;
        player.transform.SetParent(armFunctionalParent);
        Debug.Log("Anchor created at " + ceilingCheck.transform.position.x + " " + ceilingCheck.transform.position.y);
    }

    private void StartDisposalProcedure() {
        FindObjectOfType<EnemyDialogueTrigger>().SayAboutDisposal();
        Invoke("DisableRedScreen", 2);
    }

    private void DisableRedScreen() {
        redScreen.SetActive(false);
    }
}