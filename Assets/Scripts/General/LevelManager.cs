using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour {
    public DimensionSwitcher dimensionSwitcher;
    public InputActionAsset inputActions; // Used for checking key binds
    public TransitionablePair[] transitionablePairs2D;
    public TransitionablePair[] transitionablePairs3D;
    
    public GameMenu gameMenu;
    public GameObject ide2D, ide3D, life3, life2, life1;

    public Text timeText;
    public Sprite lostLifeSprite;
    public static bool IsIn2D = false; //for now
    
    public static int SwitchesPressed = 0;
    public int lives = 3;
    public static int Language = 0; // 0 = English (default), 1 = Latvian, 2 = Russian
    private int _seconds, _minutes, _startSeconds;
    
    string _strMinutes = "00"; 
    string _strSeconds = "0";
    public static string InteractButton = "";
    
    public float timePassed = 0;
    
    void Start(){
        // Need to create the initial 2D space
        Vector3 planeRight = dimensionSwitcher.Slice3DWorld();
        SwitchTo2D(planeRight);
        
        // Applying user settings
        if (PlayerPrefs.HasKey("Language")){
            Language = PlayerPrefs.GetInt("Language");
            Debug.Log(Language);
        }
        if (!inputActions) {
            Debug.LogError("Interact action not found! Set it manually!");
            return;
        }
        var interactAction = inputActions.FindAction("Interact"); // Find the action by name
            
        // Retrieve the binding
        var binding = interactAction.bindings[0];
        InteractButton = InputControlPath.ToHumanReadableString( // Getting the exact key name out of the control path
            binding.effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );
    } 

    // Update is called once per frame
    void Update(){
        if (Input.GetKeyDown(KeyCode.T) && IsIn2D == false) { // Trigger dimension switch to 2D world
            // planeRight is passed to all paired objects to determine which way they should be moved in 3D to match the 2D counterpart
            Vector3 planeRight = dimensionSwitcher.Slice3DWorld();
            SwitchTo2D(planeRight);
        }
        else if (Input.GetKeyDown(KeyCode.T) && IsIn2D) { // Trigger dimension switch to 3D world
            SwitchTo3D();
            dimensionSwitcher.Clean2DWorld(); // Free scene of no longer needed 2D objects
        }
        
        #region TimeCalculator
        timePassed += Time.deltaTime; // Fetching time that has passed since the last frame and adding it to the sum
        // Using math to output the time in seconds using minutes:seconds format
        _seconds = (int)timePassed - (60 * _minutes);
        if(_seconds==60) {
            // Adding another minute to the timer
            _minutes++;
            _strMinutes = _minutes switch {
                >= 9 => _minutes.ToString(),
                < 9 => "0"
            };
        }
        _strSeconds = _seconds switch {
            <=9 => "0" + _seconds,
            _ => _seconds.ToString()
        };
        timeText.text = _strMinutes + ":" + _strSeconds;
        #endregion
    }

    #region Dimension Switching
    public void SwitchTo2D(Vector3 planeRight){
        ide2D.SetActive(true);
        foreach (var pair in transitionablePairs3D) { // Every object is moved if its counterpart has moved
            pair.BeginTransition(planeRight);
        }
        ide3D.SetActive(false);
        IsIn2D = true;
    }

    public void SwitchTo3D(){
        ide3D.SetActive(true);
        foreach (var pair in transitionablePairs2D) { // Every object is moved if its counterpart has moved
            pair.BeginTransition();
        }
        ide2D.SetActive(false);
        IsIn2D = false;
    }
    #endregion
    
    public void SwitchPressed(){
        SwitchesPressed++;
        Debug.Log(SwitchesPressed + " Pressed");
    }

    public void LostLife() {
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
                gameMenu.LevelFailed();
                return;
        }
        gameMenu.SetDeathMenuActive();
    }
    public static void LoadNextLevel() {
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex)+1);
    }
}
