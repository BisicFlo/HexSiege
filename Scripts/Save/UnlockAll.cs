using UnityEngine;

public class UnlockAll : MonoBehaviour {

    private int index = 0;

    public void UnlockNextLevel() {

        SaveManager.Instance.UnlockNextLevel(index, 1); // currentLevelIndex // starsEarned

        index++;

    }
}
