using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used to change canvas / need UIManager.cs
/// </summary>
public class ScreenButton : MonoBehaviour {
    [SerializeField] private string screenName;

    [SerializeField] private SoundData clickSound; // New


    private Button button;

    private void OnEnable() {
        //Debug.Log(" Added Listener Screen : " + screenName);
        button = GetComponent<Button>();
        button.onClick.AddListener(() => {
            UIManager.Instance.ShowScreen(screenName);
            SoundManager.Instance.PlaySFX(clickSound);
        });
    }



    private void OnDisable() {
        //Debug.Log(" Removed Listener Screen : " + screenName);
        button.onClick.RemoveAllListeners();
    }
}
