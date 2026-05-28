using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Curse : MonoBehaviour {
    // Callback - This is what we'll use
    [HideInInspector] public System.Action OnCurseFallComplete;

    [Header("Curse Marks")]
    [Tooltip("Assign your 4 curse mark GameObjects here ")]
    [SerializeField] private GameObject[] curseMarks = new GameObject[4];

    [Header("Falling Curse Effect")]
    [SerializeField] private GameObject curseObject;                      // The object that will "fall" on the enemy (can be a VFX, skull, rune, etc.)
    [SerializeField] private Transform fallTarget;                        // Usually the enemy itself or a point on top of it
    [SerializeField] private float fallDuration = 1.2f;
    //[SerializeField] private AnimationCurve fallCurve;                    // Controls non-linear speed
    //[SerializeField] private float initialBackwardDistance = 1.5f;
    //[SerializeField] private float initialBackwardDuration = 0.25f;
    //[SerializeField] private float gravity = -9.81f;    // Simulated gravity

    [SerializeField] private ParticleSystem curseEffect;


    private int activeCurses = 0;

    public bool CurseMe = false; // temp

    private void Update() {     // temp
        if (CurseMe) {
            CurseMe = false;
            ApplyCurse();
        }
    }

    private void Start() {
        // All marks start deactivated
        ResetCurses();
    }
    public bool ApplyCurse() {
        Debug.Log("ApplyCurse()");

        if (activeCurses < 4) {
            curseEffect.Play();
            HideAllStages();//New
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
        return false; // Enemy already dead
    }
    public void ResetCurses() {
        curseEffect.Stop();
        activeCurses = 0;
        HideAllStages();
    }

    public void HideAllStages() {
        foreach (var mark in curseMarks) {
            if (mark != null)
                mark.SetActive(false);
        }
    } 

    private void CrushTarget() {
        //StartCoroutine(CurseFallSequence());
        StartCoroutine(CurseFallSequenceV2(fallDuration,1));
    }

    //private IEnumerator CurseFallSequence() {
    //    if (curseObject == null) {
    //        Debug.LogWarning("No curseObject assigned. Enemy will die instantly.");
    //        yield break;
    //    }
    //    yield return new WaitForSeconds(1);

    //    // Activate the falling object
    //    curseObject.SetActive(true);
    //    Vector3 startPosition = curseObject.transform.position;
    //    Vector3 targetPosition = fallTarget.position;

    //    // 1. Slight backward movement at the beginning
    //    Vector3 backwardPos = startPosition + (startPosition - targetPosition).normalized * initialBackwardDistance;

    //    float timer = 0f;
    //    while (timer < initialBackwardDuration) {
    //        timer += Time.deltaTime;
    //        float t = timer / initialBackwardDuration;
    //        curseObject.transform.position = Vector3.Lerp(startPosition, backwardPos, t);
    //        yield return null;
    //    }

    //    // 2. Fall down with non-linear speed using AnimationCurve
    //    timer = 0f;
    //    Vector3 fallStartPos = curseObject.transform.position;

    //    while (timer < fallDuration) {
    //        timer += Time.deltaTime;
    //        float normalizedTime = timer / fallDuration;
    //        float curveValue = fallCurve.Evaluate(normalizedTime);

    //        curseObject.transform.position = Vector3.Lerp(fallStartPos, targetPosition, curveValue);
    //        yield return null;
    //    }

    //    // Ensure final position
    //    curseObject.transform.position = targetPosition;

    //    // Optional: Impact effect
    //    // ParticleSystem ps = curseObject.GetComponentInChildren<ParticleSystem>();
    //    // if (ps) ps.Play();

    //}

    private IEnumerator CurseFallSequenceV2( float lerpDuration, int mode) {

        yield return new WaitForSeconds(1);

        Vector3 startValueFrozen = curseObject.transform.position;
        Vector3 endValueFrozen = fallTarget.transform.position;

        float timeElapsed = 0;
        //Vector3 jump = Vector3.zero;
        while (timeElapsed < lerpDuration) {
            float t = timeElapsed / lerpDuration;
            if (mode == 1) t = Mathf.Sin(t * Mathf.PI * 0.5f);
            if (mode == 2) t = Mathf.SmoothStep(0, 1, t);

            curseObject.transform.position = Vector3.Slerp(startValueFrozen, endValueFrozen, t);
            //jump.y = 0.5f * gravity * lerpDuration * timeElapsed - 0.5f * gravity * timeElapsed * timeElapsed;
            //curseObject.transform.position -= jump;
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        curseObject.transform.position = endValueFrozen;

        CompleteCurse();
    }

    private void CompleteCurse() {
        OnCurseFallComplete?.Invoke();     //This calls the Enemy's Death()
    }
}