using System.Collections;
using UnityEngine;

/// <summary>
/// Curse System - Applies up to 4 curse marks to a target.
/// When all 4 marks are applied, triggers a dramatic falling curse object
/// that crushes the target and invokes a death callback.
/// </summary>
public class Curse : MonoBehaviour {

    // --------------------------------------------------------------
    //   Inspector Fields
    // -------------------------------------------------------------- 

    [HideInInspector] public System.Action OnCurseFallComplete; // Called when the curse fall animation completes.

    [Header("Curse Marks")]
    [Tooltip("Assign 4 curse mark visuals (GameObjects)")]
    [SerializeField] private GameObject[] curseMarks = new GameObject[4];

    [Header("Falling Curse Effect")]
    [Tooltip("The  curse object / Parent that falls on the enemy when 4 marks are applied")]
    [SerializeField] private GameObject curseObject;     

    [Tooltip("Target position where the curse object should land (usually the enemy's center")]
    [SerializeField] private Transform fallTarget;         // Usually the enemy itself or a point on top of it
    [SerializeField] private float fallDuration = 1.2f;

    [Header("Visual Effect")]
    [Tooltip("Particle system that plays while curse marks are active")]
    [SerializeField] private ParticleSystem curseEffect;   // Small particles when at least 1 mark is applied


    // --------------------------------------------------------------
    //   Private 
    // --------------------------------------------------------------

    private int activeCurses = 0;

    // --------------------------------------------------------------
    //   MonoBehaviour
    // --------------------------------------------------------------

    private void Start() {
        ResetCurses();  // All marks start deactivated
    }

    /// <summary>
    /// Applies one curse mark. Returns true when all 4 marks are applied (lethal).
    /// </summary>
    /// <returns>True if the target should die, false otherwise.</returns>
    public bool ApplyCurse() {

        if (activeCurses < 4) {
            curseEffect.Play();
            HideAllStages();
            curseMarks[activeCurses].SetActive(true);

            activeCurses++;

            if (activeCurses >= 4) {
                CrushTarget();
                return true;  // Enemy will die
            }
            else {
                return false; // Enemy will not die
            }
        }
        return false; // Enemy is already fully cursed
    }

    /// <summary>
    /// Resets the curse system to its initial state.
    /// Called when the enemy respawns or the curse is removed.
    /// </summary>
    public void ResetCurses() {
        curseEffect.Stop();
        activeCurses = 0;
        HideAllStages();
    }

    /// <summary>
    /// Deactivates all curse mark visuals.
    /// </summary>
    public void HideAllStages() {
        foreach (var mark in curseMarks) {
            if (mark != null)
                mark.SetActive(false);
        }
    }

    /// <summary>
    /// Triggers the final crushing curse animation.
    /// </summary>
    private void CrushTarget() {
        StartCoroutine(CurseFallSequenceV2(fallDuration, 1)); // Coroutine couldn't be started because the the game object 'Curse' is inactive!
    }

    /// <summary>
    /// Animates the curse object falling onto the target.
    /// </summary>
    private IEnumerator CurseFallSequenceV2(float lerpDuration, int mode) {

        yield return new WaitForSeconds(1);

        Vector3 startValueFrozen = curseObject.transform.position;
        Vector3 endValueFrozen = fallTarget.transform.position;

        float timeElapsed = 0;
        while (timeElapsed < lerpDuration) {
            float t = timeElapsed / lerpDuration;
            if (mode == 1) t = Mathf.Sin(t * Mathf.PI * 0.5f); // Ease-out
            if (mode == 2) t = Mathf.SmoothStep(0, 1, t); // Smooth acceleration + deceleration

            curseObject.transform.position = Vector3.Slerp(startValueFrozen, endValueFrozen, t);

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        curseObject.transform.position = endValueFrozen;

        CompleteCurse();
    }

    /// <summary>
    /// Called when the curse object has finished falling.
    /// Notifies the subscriber (usually the enemy) to die.
    /// </summary>
    private void CompleteCurse() {
        OnCurseFallComplete?.Invoke();     //This calls the Enemy's Death()
    }
}