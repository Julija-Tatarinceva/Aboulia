using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

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
        MainMenu, OptionsMenu,
        SoundMenu, GameplayMenu,
        ControlsMenu, NewGameDialog,
        LoadGameDialog, RecordsDialog,
        NoSaveDialog
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
    }
    #endregion

    #region Update - Handle Escape Key
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            switch (_currentMenu) {
                case MenuState.OptionsMenu:
                case MenuState.RecordsDialog:
                case MenuState.LoadGameDialog:
                case MenuState.NewGameDialog:
                    GoBackToMainMenu();
                    break;
                case MenuState.SoundMenu:
                case MenuState.GameplayMenu:
                    GoBackToOptionsMenu();
                    break;
                case MenuState.ControlsMenu:
                    GoBackToGameplayMenu();
                    break;
            }
            PlayClickSound();
        }
    }
    #endregion

    #region Button Click Handlers
    public void OnMouseClick(string buttonType) {
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
            case "Records":
                FindObjectOfType<LoadRecords>().PopulateRecordsMenu();
                OpenMenu(MenuState.RecordsDialog, recordsDialog, menuDefaultCanvas);
                break;
            case "Exit":
                Application.Quit();
                break;
        }
    }
    #endregion

    #region Volume
    public void VolumeSlider(float volume) {
        AudioListener.volume = volume;
        volumeText.text = volume.ToString("0.0");
        _masterVolume = volume;
    }
    
    public void ApplyVolumeSettings() {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        PlayerPrefs.Save();
        ShowConfirmation();
    }
    public void TMP_InputField(string userInput) {
        // Convert the character to lowercase to handle case-insensitivity
        if(string.IsNullOrEmpty(userInput) || userInput.Length != 1) return;
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
    public void ApplyInteractionSettings() {
        // Find the "Interact" action
        var interactAction = inputActions.FindAction("Interact");
        if (interactAction == null) {
            Debug.LogError("Interact action not found.");
            return;
        }
        // Find the binding you want to change (e.g., the first one)
        var bindingIndex = 0; // Change this if you want to target a different binding
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
        ShowConfirmation();
    }
    #endregion

    #region Reset Settings
    public void ResetSettings(string resetType) {
        switch (resetType) {
            case "Audio":
                VolumeSlider(defaultVolume);
                volumeSlider.value = defaultVolume;
                _masterVolume = defaultVolume;
                ApplyVolumeSettings();
                break;
        }
    }
    #endregion

    #region Dialog Actions
    public void OnNewGameDialog(string action) {
        if (action == "Yes") 
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else 
            GoBackToMainMenu();
    }

    public void OnLoadGameDialog(string action) {
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
    public void ChangeLanguage(string languageCode) {
        Locale selectedLocale = LocalizationSettings.AvailableLocales.Locales
            .FirstOrDefault(locale => locale.Identifier.Code == languageCode);
        if (selectedLocale != null) 
            LocalizationSettings.SelectedLocale = selectedLocale;
    }
    #endregion

    #region Menu Navigation
    private void OpenMenu(MenuState newMenuState, GameObject newMenu, GameObject oldMenu) {
        if(oldMenu) 
            oldMenu.SetActive(false);
        newMenu.SetActive(true);
        _currentMenu = newMenuState;
    }

    public void GoBackToOptionsMenu() {
        OpenMenu(MenuState.OptionsMenu, generalSettingsCanvas, null);
        soundMenu.SetActive(false);
        gameplayMenu.SetActive(false);
    }

    public void GoBackToMainMenu() {
        menuDefaultCanvas.SetActive(true);
        DisableAllMenus();
        _currentMenu = MenuState.MainMenu;
    }

    public void GoBackToGameplayMenu() {
        OpenMenu(MenuState.GameplayMenu, gameplayMenu, null);
    }

    private void DisableAllMenus() {
        generalSettingsCanvas.SetActive(false);
        soundMenu.SetActive(false);
        gameplayMenu.SetActive(false);
        confirmationMenu.SetActive(false);
        noSaveDialog.SetActive(false);
        newGameDialog.SetActive(false);
        loadGameDialog.SetActive(false);
        recordsDialog.SetActive(false);
    }
    #endregion

    #region Utility
    private void ShowConfirmation() {
        StartCoroutine(ShowConfirmationCoroutine());
    }

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
}
