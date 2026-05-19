using System.Collections;
using UnityEngine;

/// <summary>
/// Updated HammerController script for Unity tower defense game.
/// Features:
/// - Accelerating swing speed: Uses AnimationCurves (EaseInQuad down for acceleration like gravity,
///   EaseOutQuad up for deceleration).
/// - Dynamic elongation: Hammer stretches along its local Y-axis (transform.up) to reach the target position
///   during the swing. The tip dynamically scales to "match" the target's world position.
/// 
/// Setup:
/// - Attach to hammer GameObject (child of tower).
/// - Orient hammer along local +Y (upright), pivot at base/center.
/// - Set 'hammerRestLength' to distance from pivot to tip at rest scale (measure in Scene view).
/// - Tower script: Rotate tower to face enemy, then call hammerController.Swing(enemy.transform.position);
/// - Collider (Trigger) on hammer will scale with elongation for better hit detection.
/// - Tweak durations, curves, scales in Inspector.
/// </summary>
public class HammerController : MonoBehaviour {
    [Header("Swing Settings")]
    [Tooltip("Duration of the down swing.")]
    public float downDuration = 0.5f;
    [Tooltip("Duration of the up swing.")]
    public float upDuration = 0.4f;
    [Tooltip("Pause at bottom for impact (sound/particles).")]
    public float impactDelay = 0.1f;

    [Header("Swing Curves (auto-set to EaseInQuad down / EaseOutQuad up)")]
    [SerializeField] public AnimationCurve downCurve;
    [SerializeField] public AnimationCurve upCurve;

    [Header("Elongation Settings")]
    [Tooltip("Distance from pivot to hammer tip at rest scale (measure in Scene).")]
    public float hammerRestLength = 2f;
    [Tooltip("Minimum scale Y (1 = no shrink, <1 allows squash).")]
    public float minElongationScale = 1f;
    [Tooltip("Maximum scale Y to prevent over-stretch.")]
    public float maxElongationScale = 3f;

    [Header("Rotation")]
    [Tooltip("Swing angle in degrees (positive X for down-forward).")]
    public float swingAngle = 90f;

    private Vector3 initialLocalScale;
    private Quaternion startRotation;
    private Vector3 swingTargetPos;
    private bool isSwinging = false;

    public bool StartSwing;
    public Transform TempTarget;

    void Awake() {
        // Default curves if not set in Inspector
        if (downCurve == null || downCurve.keys.Length == 0)
            downCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        if (upCurve == null || upCurve.keys.Length == 0)
            upCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    }
    

    void Start() {
        initialLocalScale = transform.localScale;
        startRotation = transform.localRotation;
    }

    private void Update() {
        if (StartSwing) {
            StartSwing = false;
            Swing(TempTarget.position);
        }
    }

    /// <summary>
    /// Trigger hammer swing towards a world target position (e.g., enemy.transform.position).
    /// Call from tower attack logic.
    /// </summary>
    public void Swing(Vector3 targetWorldPos) {
        if (!isSwinging) {
            StartCoroutine(SwingCoroutine(targetWorldPos));
        }
    }

    private IEnumerator SwingCoroutine(Vector3 targetPos) {
        isSwinging = true;
        swingTargetPos = targetPos;

        // Down swing
        Quaternion startRot = startRotation;
        Quaternion endRot = startRotation * Quaternion.Euler(swingAngle, 0f, 0f);
        float elapsed = 0f;
        while (elapsed < downDuration) {
            elapsed += Time.deltaTime;
            float t = elapsed / downDuration;
            float curveT = downCurve.Evaluate(t);
            transform.localRotation = Quaternion.Slerp(startRot, endRot, curveT);
            UpdateElongation();
            yield return null;
        }
        transform.localRotation = endRot;
        UpdateElongation();

        // Impact pause (add effects here)
        yield return new WaitForSeconds(impactDelay);

        // Up swing
        startRot = endRot;
        endRot = startRotation;
        elapsed = 0f;
        while (elapsed < upDuration) {
            elapsed += Time.deltaTime;
            float t = elapsed / upDuration;
            float curveT = upCurve.Evaluate(t);
            transform.localRotation = Quaternion.Slerp(startRot, endRot, curveT);
            UpdateElongation();
            yield return null;
        }

        // Reset precisely
        transform.localRotation = startRotation;
        transform.localScale = initialLocalScale;
        isSwinging = false;
    }

    private void UpdateElongation() {
        Vector3 pivotPos = transform.position;
        Vector3 deltaToTarget = swingTargetPos - pivotPos;
        Vector3 hammerDir = transform.up;
        float projection = Vector3.Dot(deltaToTarget, hammerDir);
        float targetScaleY = projection / hammerRestLength;
        float scaleY = Mathf.Clamp(targetScaleY, minElongationScale, maxElongationScale);
        transform.localScale = new Vector3(initialLocalScale.x, scaleY, initialLocalScale.z);
    }
}