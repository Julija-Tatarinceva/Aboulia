using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadRecords : MonoBehaviour
{
    public Text Level1Record, Level2Record, Level3Record, Level4Record;
    public Text LoadGame;
    void Start()
    {
        Level1Record.text = "Level 1: " + PlayerPrefs.GetString("Best time at level 1");
        Level2Record.text = "Level 2: " + PlayerPrefs.GetString("Best time at level 2");
        Level3Record.text = "Level 3: " + PlayerPrefs.GetString("Best time at level 3");
        Level4Record.text = "Level 4: " + PlayerPrefs.GetString("Best time at level 4");
        if (PlayerPrefs.HasKey("Level where we stopped"))
        {
            LoadGame.text = "You left at level " + PlayerPrefs.GetInt("Level where we stopped") + "\n" + "Are you sure you want to load?";
        }
        else{
            LoadGame.text = "No saved level.";
        }
    }
}
