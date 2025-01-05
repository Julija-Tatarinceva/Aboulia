using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace General
{
    public class LevelManager : MonoBehaviour {
        public DimensionSwitcher dimensionSwitcher;
        public InputActionAsset inputActions; // Used for checking key binds
    
        public GameMenu gameMenu;
        public GameObject ide3D;
        public GameObject life3;
        public GameObject life2;
        public GameObject life1;

        public Text timeText;
        public Sprite lostLifeSprite;
        private static bool _isIn2D = false; //for now
    
        public int switchesPressed = 0;
        public int lives = 3;
        private int _seconds, _minutes, _startSeconds;
    
        private string _strMinutes = "00"; 
        private string _strSeconds = "0";
        public string interactButton = "";
        public InputAction interactAction;
    
        public float timePassed = 0;
        public bool Done2DTutorial = false;
        public bool Done3DTutorial = false;

        private void Start(){ // LM_F04
            // Applying user settings
            if(PlayerPrefs.HasKey("masterVolume")){ // Volume settings
                AudioListener.volume = PlayerPrefs.GetFloat("masterVolume");
            }
            if (!inputActions) {
                Warning.ShowWarning("Interact action not found! Set it manually!");
                return;
            }
            var playerActionMap = inputActions.FindActionMap("Player");
            playerActionMap.Enable();
            interactAction = inputActions.FindAction("Interact"); // Find the action by name
            interactAction.Enable();
            
            // Retrieve the binding
            var binding = interactAction.bindings[0];
            interactButton = InputControlPath.ToHumanReadableString( // Getting the exact key name out of the control path
                binding.effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice
            );
            Invoke(nameof(GetTutorial), 0.1f);
        }

        private void GetTutorial() {
            Done2DTutorial = FindObjectOfType<SavesSystem>().GetTutorialCompletion("2D tutorial");
            Done3DTutorial = FindObjectOfType<SavesSystem>().GetTutorialCompletion("3D tutorial");
        }

        // Update is called once per frame
        private void Update(){ // LM_F09
            #region Time Calculator
            timePassed += Time.deltaTime; // Fetching time that has passed since the last frame and adding it to the sum
            // Using math to output the time in seconds using minutes:seconds format
            _seconds = (int)timePassed - (60 * _minutes);
            if(_seconds==60) {
                // Adding another minute to the timer
                _minutes++;
                _strMinutes = _minutes switch {
                    >= 9 => _minutes.ToString(),
                    < 9 => "0" + _minutes
                };
            }
            _strSeconds = _seconds switch {
                <=9 => "0" + _seconds,
                _ => _seconds.ToString()
            };
            timeText.text = _strMinutes + ":" + _strSeconds;
            #endregion
        }
    
        public void SwitchPressed(){
            switchesPressed++;
        }

        public void LostLife() { // LM_F06
            lives--;
            switch (lives) { // Using switch since there are only 3 discrete situations
                case 2:
                    life3.GetComponent<Image>().sprite = lostLifeSprite;
                    break;
                case 1:
                    life2.GetComponent<Image>().sprite = lostLifeSprite;
                    break;
                case 0: // If there are no lives left then the level is failed
                    life1.GetComponent<Image>().sprite = lostLifeSprite;
                    gameMenu.OpenMenu("Level failed");
                    return;
            }
            gameMenu.OpenMenu("Death");
        }
        public void LoadNextLevel() { // LM_F13
            int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
            if (SceneManager.sceneCountInBuildSettings == nextScene) {
                SceneManager.LoadScene(0);
            }
            else{
                SceneManager.LoadScene(nextScene);
            }
            // SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings == nextScene ? 0 : nextScene);
        }
        public void SaveLevel() { // LM_F14
            SavesSystem savesSystem = FindObjectOfType<SavesSystem>();
            if(!savesSystem){
                Warning.ShowWarning("No save system found!");
            }
            int levelNumber = SceneManager.GetActiveScene().buildIndex;
            int previousRecord = savesSystem.GetLevelRecord(levelNumber);
            int newRecord = (int)timePassed;
            if (previousRecord != 0){
                if (previousRecord > newRecord) {
                    savesSystem.UpdateLevelRecord(levelNumber, newRecord);
                }
            }
            else
                savesSystem.UpdateLevelRecord(levelNumber, newRecord);
        
            // The level which player will be able to load is the next one after the last completed one
            // But if there is no next level, then the current level is the one to be loaded
            if (SceneManager.sceneCountInBuildSettings == SceneManager.GetActiveScene().buildIndex + 1) {
                savesSystem.UpdateLastSavedLevel(SceneManager.GetActiveScene().buildIndex);
            }
            else {
                savesSystem.UpdateLastSavedLevel(SceneManager.GetActiveScene().buildIndex + 1);
            }
            // savesSystem.UpdateLastSavedLevel(SceneManager.sceneCountInBuildSettings == SceneManager.GetActiveScene().buildIndex+1 ? 
            //     SceneManager.GetActiveScene().buildIndex : SceneManager.GetActiveScene().buildIndex+1);
        }
    }
}
