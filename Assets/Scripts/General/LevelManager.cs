using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour {
    public DimensionSwitcher dimensionSwitcher;
    public InputActionAsset inputActions; // Used for checking key binds
    public TransitionablePair[] transitionablePairs2D;
    public TransitionablePair[] transitionablePairs3D;
    public GameObject ide2D, ide3D, Life3, Life2, Life1;
    public Text timeText;
    public Sprite lostLifeSprite;
    public bool isIn2D = false; //for now
    public int switchesPressed = 0;
    public int lives = 3;
    public static int language = 0; // 0 = English (default), 1 = Latvian, 2 = Russian
    public static string interactButton = "";
    public static GameObject mainCanvas;
    int seconds, minutes, startSeconds;
    string strMinutes = "0"; 
    string strSeconds = "0";
    public float TimePassed = 0;
    
    void Start(){
        if (PlayerPrefs.HasKey("Language")){
            language = PlayerPrefs.GetInt("Language");
            Debug.Log(language);
        }
        // Find the action by name
        var interactAction = inputActions.FindAction("Interact");
            
        if (interactAction == null) {
            Debug.LogError("Interact action not found!");
            return;
        }
            
        // Retrieve the binding
        var binding = interactAction.bindings[0];
        interactButton = InputControlPath.ToHumanReadableString( // Getting the exact key name out of the control path
            binding.effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );
    } 

    // Update is called once per frame
    void Update(){
        if (Input.GetKeyDown(KeyCode.T) && isIn2D == false) { // Trigger dimension switch to 2D world
            Vector3 planeRight = dimensionSwitcher.Slice3DWorld();
            SwitchTo2D(planeRight);
        }
        else if (Input.GetKeyDown(KeyCode.T) && isIn2D) { // Trigger dimension switch to 3D world
            SwitchTo3D();
            dimensionSwitcher.Clean2DWorld();
        }
        
        #region TimeCalculator
        TimePassed += Time.deltaTime; // Fetching time that has passed since the last frame and adding it to the sum
        // Using math to output the time in seconds using minutes:seconds format
        seconds = (int)TimePassed - (60 * minutes);
        if(seconds==60){ // Adding another minute to the timer
            minutes++;
            if(minutes>9) 
                strMinutes = minutes.ToString();
            else if (minutes < 9) 
                strMinutes = "0" + minutes;
            else if (minutes == 0)
                strMinutes = "00";
        }
        if(seconds<=9) 
            strSeconds = "0" + seconds;
        else 
            strSeconds = seconds.ToString();
        timeText.text = strMinutes + ":" + strSeconds;
        #endregion
    }

    #region Dimension Switching
    public void SwitchTo2D(Vector3 planeRight){
        ide2D.SetActive(true);
        foreach (var pair in transitionablePairs3D)
        {
            pair.BeginTransition(planeRight);
        }
        ide3D.SetActive(false);
        isIn2D = true;
    }

    public void SwitchTo3D(){
        ide3D.SetActive(true);
        foreach (var pair in transitionablePairs2D)
        {
            pair.BeginTransition();
        }
        ide2D.SetActive(false);
        isIn2D = false;
    }
    #endregion
    
    public void SwitchPressed(){
        switchesPressed++;
        Debug.Log(switchesPressed + " Pressed");
    }
    public void LoadNextLevel() {
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex)+1);
    }
}
