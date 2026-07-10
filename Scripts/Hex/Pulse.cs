using UnityEngine;

public class Pulse : MonoBehaviour {

    [Header("Color Variation")]
    public bool pulseColor = true;
    public Color highlightColor = Color.yellow;
    [Range(0f, 1f)]
    public float colorIntensity = 0.75f;
    public float pulseSpeed = 1.3f;

    [Header("Emission Glow")]
    public bool pulseEmission = true;
    public Color emissionColor = Color.cyan;
    [Range(0f, 10f)]
    public float emissionIntensity = 3f;           // How strong the glow is
    [Range(0f, 1f)]
    public float emissionPulseAmount = 0.7f;       // How much it pulses (0 = no pulse)


    private Color originalColor;
    private Renderer rendererComp;
    private MaterialPropertyBlock propBlock;
    private bool isActive = true;

    private static readonly int EmissionProperty = Shader.PropertyToID("_EmissionColor"); // or "_BaseColor" for URP/HDRP

    void Awake() {
        rendererComp = GetComponent<Renderer>();
        if (rendererComp == null) {
            Debug.LogWarning("No Renderer found on " + gameObject.name);
            return;
        }

        propBlock = new MaterialPropertyBlock();

        // Store original color
        rendererComp.GetPropertyBlock(propBlock);
    }

    void Update() {
        if (!isActive || rendererComp == null) return;

        float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);


        // Color Pulse
        if (pulseColor) {
            rendererComp.GetPropertyBlock(propBlock);

            float intensity = emissionIntensity * Mathf.Lerp(1f - emissionPulseAmount, 1f + emissionPulseAmount, t);


            Color emissive = emissionColor * intensity;
            propBlock.SetColor(EmissionProperty, emissive);

            rendererComp.SetPropertyBlock(propBlock);
        }
    }

    public void StopPulsing() {
        isActive = false;

        // Reset color
        if (rendererComp != null && propBlock != null) {
            rendererComp.GetPropertyBlock(propBlock);
            propBlock.SetColor(EmissionProperty, Color.black);
            rendererComp.SetPropertyBlock(propBlock);
        }
    }

    public void StartPulsing() {
        isActive = true;
    }
    private void OnDestroy() {
        if (propBlock != null)
            propBlock.Clear();
    }
}