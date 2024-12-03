using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LoadRecords : MonoBehaviour
{
    public Text level1Record;
    public Text level2Record;
    public Text level3Record;
    public Text level4Record;
    public Text loadGame;
    void Start() {
        level1Record.text = "Level 1: " + PlayerPrefs.GetString("Best time at level 1");
        level2Record.text = "Level 2: " + PlayerPrefs.GetString("Best time at level 2");
        // level3Record.text = "Level 3: " + PlayerPrefs.GetString("Best time at level 3");
        // level4Record.text = "Level 4: " + PlayerPrefs.GetString("Best time at level 4");
        if (PlayerPrefs.HasKey("Level where we stopped"))
        {
            loadGame.text = "You left at level " + PlayerPrefs.GetInt("Level where we stopped") + "\n" + "Are you sure you want to load?";
        }
        else{
            loadGame.text = "No saved level.";
        }
    }
}
