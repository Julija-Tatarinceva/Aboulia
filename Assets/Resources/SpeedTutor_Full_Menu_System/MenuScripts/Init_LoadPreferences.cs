using General;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.SpeedTutor_Full_Menu_System.MenuScripts
{
    public class InitLoadPreferences : MonoBehaviour {
        #region Variables
        //VOLUME
        [SerializeField] private Text volumeText;
        [SerializeField] private Slider volumeSlider;
    
        [SerializeField] private bool canUse = false;
        [SerializeField] private MenuController menuController;
        #endregion

        private void Awake() {
            Debug.Log("Loading player prefs test");
            if (canUse) {
                //VOLUME
                if (PlayerPrefs.HasKey("masterVolume")) {
                    float localVolume = PlayerPrefs.GetFloat("masterVolume");

                    volumeText.text = localVolume.ToString("0.0");
                    volumeSlider.value = localVolume;
                    AudioListener.volume = localVolume;
                }
                else
                    menuController.ResetSettings("Audio");
            }
        }
    }
}
