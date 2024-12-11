using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadRecords : MonoBehaviour {
    public SavesSystem savesSystem;
    public Text level1Record;
    public Text level2Record;
    public Text level3Record;
    public Text level4Record;
    public Text[] levelRecords;
    LocalizeStringEvent localizeEvent;
    public Text loadGame;
    void Start() {
        levelRecords = new Text[] { level1Record, level2Record, level3Record };
    }

    public void PopulateLoadGameMenu() {
        // Get the LocalizeStringEvent and bind the placeholder value
        localizeEvent = loadGame.GetComponent<LocalizeStringEvent>();
        int lastLevel = savesSystem.LoadLastLevel();
        if (lastLevel != 0) {
            localizeEvent.StringReference.TableEntryReference = "are_you_sure_to_load_text";
            // Add the "lvl" parameter to the LocalizeStringEvent
            if (localizeEvent != null) {
                IntVariable lvl = new IntVariable();
                lvl.Value = lastLevel;
                localizeEvent.StringReference.Add("lvl", lvl); // Set the key placeholder with the dynamic value
            }
        }
        else
            localizeEvent.StringReference.TableEntryReference = "no_saved_level_text";
        localizeEvent.RefreshString(); // Refresh the localized text to apply the changes
    }

    public void PopulateRecordsMenu() {
        for (int i = 1; i <= SceneManager.sceneCountInBuildSettings-1; i++) {
            localizeEvent = levelRecords[i-1].GetComponent<LocalizeStringEvent>();
            if (localizeEvent != null) {
                IntVariable lvl = new IntVariable { Value = i };
                int recordInSeconds = savesSystem.GetLevelRecord(i);
                int minutes = recordInSeconds / 60;
                int seconds = recordInSeconds % 60;
                var timeText = minutes == 0 && seconds == 0 ? "-" : $"{minutes:D2}:{seconds:D2}";
                StringVariable record = new StringVariable { Value = timeText };

                localizeEvent.StringReference.Add("lvl", lvl); // Set the key placeholder with the dynamic value
                localizeEvent.StringReference.Add("record", record); // Set the key placeholder with the dynamic value
                localizeEvent.RefreshString();
            }
        }
    }
}
