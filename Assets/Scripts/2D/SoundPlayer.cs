using UnityEngine;

public class SoundPlayer : MonoBehaviour {
    public AudioSource stepSound;
    public AudioClip[] stepClips; // Array of step sounds

    public void PlayStepSound(bool moving, bool grounded) { // LM_F01
        // Playing sound of walking when player starts moving
        if (!moving || stepSound.isPlaying || !grounded) return;
        if (stepClips.Length > 0) {
            // Pick a random sound from the array
            stepSound.clip = stepClips[Random.Range(0, stepClips.Length)];
            stepSound.Play();
        } else
            Debug.LogWarning("Step clips array is empty!");
    }
}
