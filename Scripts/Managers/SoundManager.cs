using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager Instance { get; private set; } // Singleton


    private void Awake() {
        if (Instance != null) Debug.LogWarning("More than one BuildManager detected");
        Instance = this;
    }


}
