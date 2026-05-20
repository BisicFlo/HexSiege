using UnityEngine;

/// <summary>
/// Used to change fps max of the game , because by default the limit is 30fps
/// </summary>
public class FpsManager : MonoBehaviour {
    public void SetFps(int fps) {
        QualitySettings.vSyncCount = 0;  // Disable VSync (ignores targetFrameRate if >0)
        Application.targetFrameRate = fps;  // Or -1 to uncap (not recommended for battery/heat)
    }
}
