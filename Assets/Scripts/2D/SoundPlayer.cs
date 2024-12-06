using UnityEngine;

public class SoundPlayer : MonoBehaviour {
    public bool audioIsPaused = false;
    public AudioSource stepSound;
    public AudioClip[] stepClips; // Array of step sounds

    public void PlayStepSound(bool moving, bool grounded) {
        // Playing sound of walking when player starts moving
        if (moving && !stepSound.isPlaying && grounded)
            PlayRandomStepSound();
        // Pausing the sound when player stops or is airborne 
        // else if (stepSound.isPlaying && (!moving || !grounded)) {
        //     stepSound.Pause();
        //     audioIsPaused = true;
        // }
        // Unpausing when player moves again to avoid unpleasant repeated sounds
        // else if (moving && !stepSound.isPlaying && grounded && audioIsPaused) {
        //     stepSound.UnPause();
        //     audioIsPaused = false;
        // }
    }

    void PlayRandomStepSound() {
        if (stepClips.Length > 0) {
            // Pick a random sound from the array
            stepSound.clip = stepClips[Random.Range(0, stepClips.Length)];
            stepSound.Play();
        } else
            Debug.LogWarning("Step clips array is empty!");
    }
}
