using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

public class LoadRecords : MonoBehaviour {
    public Text level1Record;
    public Text level2Record;
    public Text level3Record;
    public Text level4Record;
    
    public Text loadGame;
    void Start() {
        Text[] levelRecords = { level1Record, level2Record, level3Record };
        LocalizeStringEvent localizeEvent;
        for (int i = 0; i < levelRecords.Length; i++) {
            int levelIndex = i + 1; // Levels start from 1
            string playerPrefKey = $"BestTimeLVL {levelIndex}";

            if (PlayerPrefs.HasKey(playerPrefKey)) {
                localizeEvent = levelRecords[i].GetComponent<LocalizeStringEvent>();
                if (localizeEvent != null) {
                    IntVariable lvl = new IntVariable { Value = levelIndex };
                    StringVariable record = new StringVariable { Value = PlayerPrefs.GetString(playerPrefKey) };

                    localizeEvent.StringReference.Add("lvl", lvl); // Set the key placeholder with the dynamic value
                    localizeEvent.StringReference.Add("record", record); // Set the key placeholder with the dynamic value
                    localizeEvent.RefreshString();
                }
            }
        }
        
        // Get the LocalizeStringEvent and bind the placeholder value
        localizeEvent = loadGame.GetComponent<LocalizeStringEvent>();
        if (PlayerPrefs.HasKey("Level where we stopped")) {
            localizeEvent.StringReference.TableEntryReference = "are_you_sure_to_load_text";
            // Add the "lvl" parameter to the LocalizeStringEvent
            if (localizeEvent != null) {
                IntVariable lvl = new IntVariable();
                lvl.Value = PlayerPrefs.GetInt("SavedLevel");
                localizeEvent.StringReference.Add("lvl", lvl); // Set the key placeholder with the dynamic value
            }
        }
        else
            localizeEvent.StringReference.TableEntryReference = "no_saved_level_text";
        localizeEvent.RefreshString(); // Refresh the localized text to apply the changes
    }
}
