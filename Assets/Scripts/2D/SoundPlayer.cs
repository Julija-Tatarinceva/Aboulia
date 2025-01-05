using General;
using UnityEngine;

namespace _2D
{
    public class SoundPlayer : MonoBehaviour {
        public AudioSource stepSound;
        public AudioClip[] stepClips; // Array of step sounds

        public void PlayStepSound() { // LM_F01
            // Playing sound of walking when player starts moving
            if (stepSound.isPlaying) 
                return;
            if (stepClips.Length > 0) {
                // Pick a random sound from the array
                stepSound.clip = stepClips[Random.Range(0, stepClips.Length)];
                stepSound.Play();
            } 
            else
                Warning.ShowWarning("Step clips array is empty!");
        }
    }
}
