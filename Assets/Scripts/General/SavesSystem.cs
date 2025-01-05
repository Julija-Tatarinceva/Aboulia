using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace General
{
    public class PlayerData {
        public int LastLevel;
        public List<int> LevelRecords; // A list to store level records
        public bool Completed2DTutorial;
        public bool Completed3DTutorial;

        public PlayerData() {
            // Initialize the player data structure
            LastLevel = 0; 
            LevelRecords = new List<int>();
            Completed2DTutorial = false;
            Completed3DTutorial = false;
        }
    }

    public class SavesSystem : MonoBehaviour {
        private PlayerData _playerData;
        private string _saveFilePath;
        private void Start() { // SM_F03
            _playerData = new PlayerData();
            _saveFilePath = Application.persistentDataPath + "/PlayerData.json";
            Debug.Log(Application.persistentDataPath);
        
            // Load data if it exists
            LoadData();
            // Populate level records automatically based on the number of scenes
            InitializeLevelRecords();
        }

        private void InitializeLevelRecords() { // SM_F03
            int sceneCount = SceneManager.sceneCountInBuildSettings-1;

            // Ensure the list is the correct size for the number of levels
            while (_playerData.LevelRecords.Count < sceneCount)
                _playerData.LevelRecords.Add(0);
        }

        private void SaveData() { // SM_F04
            string json = JsonUtility.ToJson(_playerData);
            File.WriteAllText(_saveFilePath, json);
        }
        private void LoadData() { // SM_F03
            if (File.Exists(_saveFilePath)) {
                string json = File.ReadAllText(_saveFilePath);
                _playerData = JsonUtility.FromJson<PlayerData>(json);
            }
        }
        public void UpdateLevelRecord(int levelIndex, int record) { // SM_F04
            if (levelIndex <= 0 || levelIndex > _playerData.LevelRecords.Count) {
                Warning.ShowWarning("Index unknown");
                return;
            }
            _playerData.LevelRecords[levelIndex - 1] = record;
            SaveData();
        }

        public void UpdateLastSavedLevel(int levelIndex) { // SM_F05
            if (levelIndex <= 0 || levelIndex > _playerData.LevelRecords.Count) {
                Warning.ShowWarning("Index unknown");
                return;
            }
            
            _playerData.LastLevel = levelIndex;
            SaveData();
        }
        public int GetLevelRecord(int levelIndex) { // SM_F06
            if (levelIndex > 0 && levelIndex <= _playerData.LevelRecords.Count)
                return _playerData.LevelRecords[levelIndex-1];
        
            Warning.ShowWarning("Index unknown");
            return 0;
        }

        public int GetLastSavedLevel() { // SM_F07
            return _playerData.LastLevel;
        }

        public bool GetTutorialCompletion(string tutorialName) { // SM_F08
            if (!_playerData.Completed2DTutorial && tutorialName == "2D tutorial") {
                _playerData.Completed2DTutorial = true;
                SaveData();
                return true;
            }
            if (!_playerData.Completed3DTutorial && tutorialName == "3D tutorial") {
                _playerData.Completed3DTutorial = true;
                SaveData();
                return true;
            }
            if (tutorialName != "2D tutorial" && tutorialName != "3D tutorial")
                Warning.ShowWarning("Tutorial name unknown");
        
            return false;
        }

        public bool DeleteSaveFile() {
            if (File.Exists(_saveFilePath)) {
                File.Delete(_saveFilePath);
                _playerData = new PlayerData();
                InitializeLevelRecords();
                SaveData();
                return true;
            }
            else {
                return false;
            }
        }
    }
}