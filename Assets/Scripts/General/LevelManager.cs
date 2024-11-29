using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour {
    public DimensionSwitcher dimensionSwitcher;
    public InputActionAsset inputActions; // Used for checking key binds
    public TransitionablePair[] transitionablePairs2D;
    public TransitionablePair[] transitionablePairs3D;
    
    public GameMenu gameMenu;
    public GameObject Ide2D, Ide3D, life3, life2, life1;
    
    public Text timeText;
    public Sprite lostLifeSprite;
    public static bool IsIn2D = false; //for now
    
    public static int SwitchesPressed = 0;
    public int lives = 3;
    public static int Language = 0; // 0 = English (default), 1 = Latvian, 2 = Russian
    int _seconds, _minutes, _startSeconds;
    
    string _strMinutes = "0"; 
    string _strSeconds = "0";
    public static string InteractButton = "";
    
    public float timePassed = 0;
    
    void Start(){
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
            Vector3 planeRight = dimensionSwitcher.Slice3DWorld();
            SwitchTo2D(planeRight);
        }
        else if (Input.GetKeyDown(KeyCode.T) && IsIn2D) { // Trigger dimension switch to 3D world
            SwitchTo3D();
            dimensionSwitcher.Clean2DWorld();
        }
        
        #region TimeCalculator
        timePassed += Time.deltaTime; // Fetching time that has passed since the last frame and adding it to the sum
        // Using math to output the time in seconds using minutes:seconds format
        _seconds = (int)timePassed - (60 * _minutes);
        if(_seconds==60){ // Adding another minute to the timer
            _minutes++;
            if(_minutes>9) 
                _strMinutes = _minutes.ToString();
            else if (_minutes < 9) 
                _strMinutes = "0" + _minutes;
            else if (_minutes == 0)
                _strMinutes = "00";
        }
        if(_seconds<=9) 
            _strSeconds = "0" + _seconds;
        else 
            _strSeconds = _seconds.ToString();
        timeText.text = _strMinutes + ":" + _strSeconds;
        #endregion
    }

    #region Dimension Switching
    public void SwitchTo2D(Vector3 planeRight){
        Ide2D.SetActive(true);
        foreach (var pair in transitionablePairs3D)
        {
            pair.BeginTransition(planeRight);
        }
        Ide3D.SetActive(false);
        IsIn2D = true;
    }

    public void SwitchTo3D(){
        Ide3D.SetActive(true);
        foreach (var pair in transitionablePairs2D)
        {
            pair.BeginTransition();
        }
        Ide2D.SetActive(false);
        IsIn2D = false;
    }
    #endregion
    
    public void SwitchPressed(){
        SwitchesPressed++;
        Debug.Log(SwitchesPressed + " Pressed");
    }

    public void LostLife() {
        lives--;
        if (lives == 2)
            life3.GetComponent<Image>().sprite = lostLifeSprite;
        else if (lives == 1)
            life2.GetComponent<Image>().sprite = lostLifeSprite;
        else{
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
