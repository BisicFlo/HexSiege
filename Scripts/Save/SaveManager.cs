using UnityEngine;
using System.IO;


[System.Serializable]
public class PlayerProgress {
    public int highestLevelReached = 1;
    public bool[] levelUnlocked;
    public int[] levelStars;       // 0–3 stars per level

    public PlayerProgress(int totalLevels) {
        levelUnlocked = new bool[totalLevels];
        levelStars = new int[totalLevels];
        levelUnlocked[0] = true; // first level always unlocked
    }

}

public class SaveManager : MonoBehaviour {
    public static SaveManager Instance { get; private set; }

    [Header("Config")]
    [SerializeField] private int totalLevels = 30;

    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public PlayerProgress Data { get; private set; }

    void Awake() {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadGame();
    }

    public void SaveGame() {
        string json = JsonUtility.ToJson(Data, prettyPrint: true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[SaveManager] Saved to {SavePath}");
    }

    public void LoadGame() {
        if (File.Exists(SavePath)) {
            string json = File.ReadAllText(SavePath);
            Data = JsonUtility.FromJson<PlayerProgress>(json);
            Debug.Log("[SaveManager] Save loaded.");
        }
        else {
            Data = new PlayerProgress(totalLevels);
            Debug.Log("[SaveManager] No save found, creating new.");

            UnlockFirstLevel(); 
        }
    }

    public void UnlockNextLevel(int justCompletedIndex, int starsEarned) {
        Data.levelStars[justCompletedIndex] = Mathf.Max(
            Data.levelStars[justCompletedIndex], starsEarned);

        int next = justCompletedIndex + 1;
        if (next < totalLevels) {
            Data.levelUnlocked[next] = true;
            Data.highestLevelReached = Mathf.Max(Data.highestLevelReached, next + 1);
        }

        SaveGame();
    }

    public void UnlockFirstLevel() {

            Data.levelUnlocked[1] = true;
            Data.highestLevelReached = 1;       

        SaveGame();
    }

    public void DeleteSave() {
        if (File.Exists(SavePath)) File.Delete(SavePath);
        Data = new PlayerProgress(totalLevels);
    }
}
