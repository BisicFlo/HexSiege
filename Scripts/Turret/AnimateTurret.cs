using System.Collections;
using UnityEngine;

/// <summary>
/// AnimateTurret - Provides a squash & stretch jiggle effect on a turret/object 
/// to give attacks a more dynamic and "alive" feeling.
/// 
/// Features:
/// - Pre-attack squash (flattening) or elongate
/// - Post-attack springy jiggle with exponential decay
/// - Configurable axis and direction
/// - Works with any attack speed
/// </summary>
public class AnimateTurret : MonoBehaviour {

    // --------------------------------------------------------------
    //   Inspector Fields
    // -------------------------------------------------------------- 

    [Header("Jiggle")]
    [SerializeField] float amplitude = 0.1f; // Initial strength of the jiggle
    [SerializeField] float decayRate = 4f;   // How quickly the jiggle fades
    [SerializeField] float frequency = 15f;

    [Tooltip("Axis in local space of the jiggle direction")]
    public enum AxisType { X, Y, Z };
    public AxisType Axis;

    [Tooltip("If true the object will elongate instead of flatten on the main axis")]
    [SerializeField] bool reversed = false;

    // --------------------------------------------------------------
    //   Private 
    // --------------------------------------------------------------

    private Vector3 originalScale;

    // --------------------------------------------------------------
    //   MonoBehaviour
    // --------------------------------------------------------------

    private void Start() {
        originalScale = this.transform.localScale;
    }

    /// <summary>
    /// Main public method to trigger the full attack animation sequence.
    /// </summary>
    /// <param name="onAttack">Callback invoked when the squash phase ends (usually fires the bullet/projectile).</param>
    /// <param name="attackSpeed">Higher value = faster animation (used to scale timing).</param>
    public IEnumerator Jiggle(System.Action onAttack, int attackSpeed) {

        // Squash/flatten before firing
        yield return StartCoroutine(FlattenBeforeJump(40f / attackSpeed));

        // Fire the actual attack
        onAttack?.Invoke();

        // Springy jiggle after firing
        StartCoroutine(JiggleWithoutBlendShape(40f / attackSpeed, -1)); // add attack Speed
    }

    /// <summary>
    /// Squashes the turret before the attack (pre-jump anticipation).
    /// </summary>
    private IEnumerator FlattenBeforeJump(float duration) {
        float timeElapsed = 0;

        while (timeElapsed < duration) {
            float jiggle = 2 * amplitude * timeElapsed / duration;// when timeElapsed==duration -> jiggle = aplitude
            AffectScale(jiggle);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Performs a decaying sinusoidal jiggle after the attack.
    /// </summary>
    /// <param name="duration">Length of the jiggle animation.</param>
    /// <param name="coefficient">Direction multiplier.</param>
    private IEnumerator JiggleWithoutBlendShape(float duration, float coefficient) {
        float timeElapsed = 0;

        while (timeElapsed < duration) {
            float jiggle = coefficient * amplitude * Mathf.Exp(-decayRate * timeElapsed) * Mathf.Sin(frequency * timeElapsed);
            AffectScale(jiggle);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Applies the calculated jiggle offset to the chosen axis.
    /// </summary>
    private void AffectScale(float jiggle) {
        if (reversed) jiggle = -jiggle;

        if (Axis == AxisType.X)
            this.transform.localScale = new Vector3(-jiggle, jiggle / 2, jiggle / 2) + originalScale; // + or * ?
        else if (Axis == AxisType.Y)
            this.transform.localScale = new Vector3(jiggle / 2, -jiggle, jiggle / 2) + originalScale; // + or * ?
        else if (Axis == AxisType.Z)
            this.transform.localScale = new Vector3(jiggle / 2, jiggle / 2, -jiggle) + originalScale; // + or * ?
    }

}
