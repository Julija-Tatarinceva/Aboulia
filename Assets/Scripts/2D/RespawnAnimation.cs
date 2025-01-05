using UnityEngine;

namespace _2D
{
    public class RespawnAnimation : MonoBehaviour {
        private static readonly int Found = Animator.StringToHash("Found");
        public FixedJoint2D connectionWithPlayer;
        public GameObject armFunctional;
        public GameObject armParent;
        public GameObject place;
        public GameObject ceilingCheck;
        public GameObject player;
        public GameObject spawn;
        public Transform playerParentOriginal;
        public Animator animator;

        public void DragPlayerToSpawn() {
            armFunctional.SetActive(true);
            armParent.transform.position = place.transform.position;
            animator.SetBool(Found, true);
            Invoke(nameof(MergeObjects), 1f);
        }

        private void MergeObjects() {
            if (!armFunctional.GetComponent<FixedJoint2D>())
                connectionWithPlayer = armFunctional.AddComponent<FixedJoint2D>();
            else
                connectionWithPlayer = armFunctional.GetComponent<FixedJoint2D>();
            connectionWithPlayer.anchor = ceilingCheck.transform.position;
            connectionWithPlayer.connectedBody = player.transform.GetComponentInParent<Rigidbody2D>();
            connectionWithPlayer.enableCollision = false;
            playerParentOriginal = player.transform.parent.GetComponent<Transform>();
            player.transform.SetParent(armFunctional.transform);
            Debug.Log("Anchor created at " + ceilingCheck.transform.position.x + " " + ceilingCheck.transform.position.y);
            Invoke(nameof(RedrawWorld), 1f);
        }

        private void RedrawWorld() {
            player.GetComponent<PlayerMovement2D>().Respawn();
            armParent.transform.position = spawn.transform.position;
            Invoke(nameof(ReleasePlayerToSpawn), 1f);
        }

        public void ReleasePlayerToSpawn() {
            if (connectionWithPlayer != null)
                Destroy(connectionWithPlayer);
            // Apply a small force to release the player
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
                playerRb.AddForce(Vector2.down * 5f, ForceMode2D.Impulse);
            // Detach the player from the arm
            if(playerParentOriginal != null)
                player.transform.SetParent(playerParentOriginal);
            else
                player.transform.SetParent(null);
            animator.SetBool(Found, false);
        }

        public void DisableArm() {
            FindObjectOfType<PlayerMovement2D>().isRespawning = false;
            armFunctional.SetActive(false);
        }
    }
}