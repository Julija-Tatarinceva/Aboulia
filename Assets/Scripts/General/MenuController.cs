using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace General
{
    public class MenuController : MonoBehaviour {
        #region Serialized Fields
        [Header("Default Values")]
        [SerializeField] private float defaultVolume = 1f;
        [SerializeField] private int defaultSensitivity = 2;
        [SerializeField] private string defaultInteractButton = "<Keyboard>/E";

        [Header("Levels To Load")]
        [SerializeField] private string newGameLevelName;

        [Header("UI Menus")]
        [SerializeField] private GameObject menuDefaultCanvas;
        [SerializeField] private GameObject generalSettingsCanvas;
        [SerializeField] private GameObject soundMenu;
        [SerializeField] private GameObject gameplayMenu;
        [SerializeField] private GameObject confirmationMenu;
        [SerializeField] private GameObject noSaveDialog;
        [SerializeField] private GameObject newGameDialog;
        [SerializeField] private GameObject loadGameDialog;
        [SerializeField] private GameObject recordsDialog;
        [SerializeField] private GameObject deleteGameDialog;
        [SerializeField] private GameObject licenseInfo;

        [Header("UI Inputs")]
        [SerializeField] private Text volumeText;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private TextMeshProUGUI interactPlaceholderText;
        [SerializeField] private Slider interactInput;
    
        [SerializeField] private AudioSource clickSoundSource;
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private InputAction interactAction;
    
        [Header("Values To Apply")]
        private float _masterVolume;
        public string newBindingPath = "<Keyboard>/E";

        #endregion

        private MenuState _currentMenu;
        private bool _audioIsPaused;

        // Enum representing different menu states
        private enum MenuState {
            MainMenu, 
            OptionsMenu,
            SoundMenu, 
            GameplayMenu,
            ControlsMenu, 
            NewGameDialog,
            LoadGameDialog, 
            RecordsDialog,
            NoSaveDialog, 
            DeleteGameDialog
        }

        #region Initialization
        private void Start() { 
            _currentMenu = MenuState.MainMenu; // Start in the main menu
            interactAction = inputActions.FindAction("Interact"); // Find the action by name to have as placeholder in the options menu
            // Retrieve the binding
            var binding = interactAction.bindings[0];
            interactPlaceholderText.text = InputControlPath.ToHumanReadableString( // Getting the exact key name out of the control path
                binding.effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice
            );
            volumeSlider.value = PlayerPrefs.GetFloat("Volume", defaultVolume);
        }
        #endregion

        #region Update - Handle Escape Key
        private void Update() { 
            if (Input.GetKeyDown(KeyCode.Escape)) {
                GoBackToPreviousMenu();
            }
        }
        #endregion
        #region Button Click Handlers
        public void OnMouseClick(string buttonType) { // MM_F12
            switch (buttonType) {
                case "Sound":
                    OpenMenu(MenuState.SoundMenu, soundMenu, generalSettingsCanvas);
                    break;
                case "Gameplay":
                    OpenMenu(MenuState.GameplayMenu, gameplayMenu, generalSettingsCanvas);
                    break;
                case "Options":
                    OpenMenu(MenuState.OptionsMenu, generalSettingsCanvas, menuDefaultCanvas);
                    break;
                case "LoadGame":
                    FindObjectOfType<LoadRecords>().PopulateLoadGameMenu();
                    OpenMenu(MenuState.LoadGameDialog, loadGameDialog, menuDefaultCanvas);
                    break;
                case "NewGame":
                    OpenMenu(MenuState.NewGameDialog, newGameDialog, menuDefaultCanvas);
                    break;
                case "DeleteGame":
                    OpenMenu(MenuState.DeleteGameDialog, deleteGameDialog, recordsDialog);
                    break;
                case "Records":
                    FindObjectOfType<LoadRecords>().PopulateRecordsMenu();
                    OpenMenu(MenuState.RecordsDialog, recordsDialog, menuDefaultCanvas);
                    break;
                case "Exit":
                    Application.Quit();
                    break;
                case "Back":
                    GoBackToPreviousMenu();
                    break;
                default:
                    Warning.ShowWarning("Unknown button code");
                    break;
            }
        }

        public void GoBackToPreviousMenu() {
            switch (_currentMenu) {
                case MenuState.OptionsMenu:
                case MenuState.RecordsDialog:
                case MenuState.LoadGameDialog:
                case MenuState.NewGameDialog:
                    GoBackToMainMenu();
                    break;
                case MenuState.SoundMenu:
                case MenuState.GameplayMenu:
                    OpenMenu(MenuState.OptionsMenu, generalSettingsCanvas, null);
                    soundMenu.SetActive(false);
                    gameplayMenu.SetActive(false);
                    break;
                case MenuState.ControlsMenu:
                    OpenMenu(MenuState.GameplayMenu, gameplayMenu, null);
                    break;
                case MenuState.DeleteGameDialog:
                    OpenMenu(MenuState.RecordsDialog, recordsDialog, deleteGameDialog);
                    break;
                default:
                    Warning.ShowWarning("Unknown menu code");
                    break;
            }
            PlayClickSound();
        }
        #endregion

        #region Volume
        public void VolumeSlider(float volume) { // MM_F12
            AudioListener.volume = volume;
            volumeText.text = volume.ToString("0.0");
            _masterVolume = volume;
        }
    
        public void ApplyVolumeSettings() { // MM_F
            PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
            PlayerPrefs.Save();
            StartCoroutine(ShowConfirmationCoroutine());
        }
        // MM_F07
        public void TMP_InputField(string userInput) {  
            // Convert the character to lowercase to handle case-insensitivity
            if (string.IsNullOrEmpty(userInput) || userInput.Length != 1) {
                Warning.ShowWarning("Input must be a valid character!");
                return;
            }
            
            char lowerChar = char.ToLower(userInput[0]);
            // Map the character to the corresponding keyboard path
            if (char.IsLetterOrDigit(lowerChar))
                // Check if it's a letter or digit
                newBindingPath = $"<Keyboard>/{lowerChar}";
            else
                switch (lowerChar) {
                    case ' ': newBindingPath = "<Keyboard>/space"; break;
                    case '.': newBindingPath = "<Keyboard>/period"; break;
                    case ',': newBindingPath = "<Keyboard>/comma"; break;
                    case ';': newBindingPath = "<Keyboard>/semicolon"; break;
                    case '/': newBindingPath = "<Keyboard>/slash"; break;
                    case '\\': newBindingPath = "<Keyboard>/backslash"; break;
                    case '[': newBindingPath = "<Keyboard>/leftBracket"; break;
                    case ']': newBindingPath = "<Keyboard>/rightBracket"; break;
                    case '-': newBindingPath = "<Keyboard>/minus"; break;
                    case '=': newBindingPath = "<Keyboard>/equals"; break;
                    default:
                        Debug.LogWarning($"Character '{userInput[0]}' is not mapped to a valid control path.");
                        return; // Return null for unmapped characters
                }
        }
        public void ApplyInteractionSettings() { // MM_F07
            // Find the "Interact" action
            var interactAction = inputActions.FindAction("Interact");
            if (interactAction == null) {
                Debug.LogError("Interact action not found.");
                return;
            }
            // Find the binding the player wants to change (e.g., the first one)
            var bindingIndex = 0;
            if (bindingIndex >= interactAction.bindings.Count) {
                Debug.LogError("Invalid binding index.");
                return;
            }

            // Change the binding to the new path
            interactAction.ChangeBinding(bindingIndex).WithPath(newBindingPath);

            // Optional: Update the readable button name
            var binding = interactAction.bindings[bindingIndex];
            interactPlaceholderText.text = InputControlPath.ToHumanReadableString(
                binding.effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice
            );
            StartCoroutine(ShowConfirmationCoroutine());
        }
        #endregion

        #region Reset Settings
        public void ResetSettings(string resetType) { // MM_F09
            switch (resetType) {
                case "Audio":
                    VolumeSlider(defaultVolume);
                    volumeSlider.value = defaultVolume;
                    _masterVolume = defaultVolume;
                    ApplyVolumeSettings();
                    break;
                case "InteractKeybind":
                    newBindingPath = defaultInteractButton;
                    ApplyInteractionSettings();
                    break;
                default:
                    Warning.ShowWarning("Unknown reset type code");
                    break;
            }
        }
        #endregion

        #region Dialog Actions
        public void OnNewGameDialog(string action) { // MM_F09
            if (action == "Yes") 
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            else 
                GoBackToMainMenu();
        }

        public void OnDeleteSavesDialog(string action) { // MM_F13
            if (action == "Yes") { // Player pressed "Yes"
                SavesSystem saveSystem = FindObjectOfType<SavesSystem>();
                if (saveSystem) {
                    bool saveFileDeleted = saveSystem.DeleteSaveFile();
                    if(!saveFileDeleted) // Nothing happened
                        return;
                    // Show visual confirmation
                    StartCoroutine(ShowConfirmationCoroutine());
                
                    // Refresh menus
                    FindObjectOfType<LoadRecords>().PopulateRecordsMenu();
                    FindObjectOfType<LoadRecords>().PopulateLoadGameMenu();
                
                    // Close the dialog window
                    GoBackToPreviousMenu();
                }
                else
                    Warning.ShowWarning("No save system found");
            }
            else // Player pressed "No"
                GoBackToPreviousMenu();
        }
        public void OnLoadGameDialog(string action) { // MM_F10
            if (action == "Yes") {
                int levelToLoad = FindObjectOfType<SavesSystem>().GetLastSavedLevel();
                if(levelToLoad != 0)
                    SceneManager.LoadScene(levelToLoad);
                else 
                    OpenMenu(MenuState.NoSaveDialog, noSaveDialog, loadGameDialog);
            } 
            else
                GoBackToMainMenu();
        }
        #endregion

        #region Localization
        public void ChangeLanguage(string languageCode) { // MM_F11
            Locale selectedLocale = LocalizationSettings.AvailableLocales.Locales
                .FirstOrDefault(locale => locale.Identifier.Code == languageCode);
            if (selectedLocale != null) 
                LocalizationSettings.SelectedLocale = selectedLocale;
            StartCoroutine(ShowConfirmationCoroutine());
        }
        #endregion

        #region Menu Navigation
        private void OpenMenu(MenuState newMenuState, GameObject newMenu, GameObject oldMenu) { // MM_F06
            if(oldMenu) 
                oldMenu.SetActive(false);
            newMenu.SetActive(true);
            _currentMenu = newMenuState;
        }

        public void GoBackToMainMenu() { // MM_F13
            menuDefaultCanvas.SetActive(true);
            generalSettingsCanvas.SetActive(false);
            soundMenu.SetActive(false);
            gameplayMenu.SetActive(false);
            confirmationMenu.SetActive(false);
            noSaveDialog.SetActive(false);
            newGameDialog.SetActive(false);
            loadGameDialog.SetActive(false);
            recordsDialog.SetActive(false);
            _currentMenu = MenuState.MainMenu;
        }
        #endregion

        #region Utility

        private IEnumerator ShowConfirmationCoroutine() {
            confirmationMenu.SetActive(true);
            yield return new WaitForSeconds(2);
            confirmationMenu.SetActive(false);
        }
        private void PlayClickSound() {
            if (clickSoundSource) 
                clickSoundSource.Play();
        }
        #endregion

        public void ShowInfo() {
            licenseInfo.SetActive(!licenseInfo.activeSelf);
        }
    }
}
