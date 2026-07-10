using UnityEngine;
using UnityEngine.UI;

public class PulseUI : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private Image image;

    [Header("Pulse Settings")]
    [SerializeField] private bool pulseScale = true;
    [SerializeField] private bool pulseColor = true;
    [SerializeField, Range(0.5f, 2f)] private float pulseSpeed = 1.2f;     // How fast it pulses
    [SerializeField, Range(0.05f, 0.5f)] private float pulseAmount = 0.15f;   // How strong (e.g. 15% bigger)

    [SerializeField] private Color highlightColor = Color.yellow;   // Color you want to pulse to    
    [SerializeField, Range(0f, 1f)] private float colorIntensity = 0.7f;           // How strong the color change is

    private Vector3 originalScale;
    private Color originalColor;
    private bool isActive = true;

    void Awake() {
        originalScale = transform.localScale;
        image = GetComponent<Image>();
        if (image != null) originalColor = image.color;
    }

    private void OnEnable() {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => OnClick());
    }

    private void OnDisable() {
        button.onClick.RemoveAllListeners();
    }

    void Update() {
        if (!isActive) return;

        float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f); // Smooth back & forth
        float value = Mathf.Lerp(1f - pulseAmount, 1f + pulseAmount, pulse);

        if (pulseScale) {
            transform.localScale = originalScale * value;
        }

        if (pulseColor) {
            Color targetColor = Color.Lerp(originalColor, highlightColor, colorIntensity);
            image.color = Color.Lerp(originalColor, targetColor, pulse);
        }
    }

    public void OnClick() {
        isActive = false;
        // Reset to normal
        transform.localScale = originalScale;
        if (image != null) image.color = originalColor;

        // Optional: Destroy this script or disable pulsing
        // Destroy(this);
    }

    // Call this to re-enable pulsing (e.g. next tutorial step)
    public void Activate() {
        isActive = true;
    }
}