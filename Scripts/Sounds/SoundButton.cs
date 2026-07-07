using UnityEngine;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour {

    [SerializeField] private SoundData clickSound; // New

    private Button button;

    private void OnEnable() {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => SoundManager.Instance.PlaySFX(clickSound));
    }

    private void OnDisable() {
        button.onClick.RemoveAllListeners();
    }
}
