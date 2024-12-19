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
    private LocalizeStringEvent _localizeEvent;
    public Text loadGame;

    private void Start() {
        levelRecords = new Text[] { level1Record, level2Record, level3Record };
    }

    // Loading in the last save level from the JSON file
    public void PopulateLoadGameMenu() { // DM_F02
        // Get the LocalizeStringEvent and bind the placeholder value
        _localizeEvent = loadGame.GetComponent<LocalizeStringEvent>();
        int lastLevel = savesSystem.GetLastSavedLevel();
        if (lastLevel != 0) {
            _localizeEvent.StringReference.TableEntryReference = "are_you_sure_to_load_text";
            // Add the "lvl" parameter to the LocalizeStringEvent
            if (_localizeEvent != null) {
                IntVariable lvl = new IntVariable();
                lvl.Value = lastLevel;
                _localizeEvent.StringReference.Add("lvl", lvl); // Set the key placeholder with the dynamic value
            }
        }
        else
            _localizeEvent.StringReference.TableEntryReference = "no_saved_level_text";
        _localizeEvent.RefreshString(); // Refresh the localized text to apply the changes
    }
    
    // Loading in records from the JSON file
    public void PopulateRecordsMenu() { // DM_F01
        for (int i = 1; i <= SceneManager.sceneCountInBuildSettings-1; i++) {
            _localizeEvent = levelRecords[i-1].GetComponent<LocalizeStringEvent>();
            if (_localizeEvent != null) {
                IntVariable lvl = new IntVariable { Value = i };
                int recordInSeconds = savesSystem.GetLevelRecord(i);
                int minutes = recordInSeconds / 60;
                int seconds = recordInSeconds % 60;
                var timeText = minutes == 0 && seconds == 0 ? "-" : $"{minutes:D2}:{seconds:D2}";
                StringVariable record = new StringVariable { Value = timeText };

                _localizeEvent.StringReference.Add("lvl", lvl); // Set the key placeholder with the dynamic value
                _localizeEvent.StringReference.Add("record", record); // Set the key placeholder with the dynamic value
                _localizeEvent.RefreshString();
            }
        }
    }
}
