using System.Collections;
using UnityEngine;

/// <summary>
/// Animate anything to make it look like a slime / affect scale 
///   
///  Hierachy :
///    Enemy (empty)
///     |> Visual (holding USlime.cs)
///       |> Renderer 
/// 
/// </summary>
public class USlime : MonoBehaviour {
    public bool activated;

    [SerializeField] private Enemy enemy; 

    [SerializeField] private Transform target; // The target (Position B) > Enemy
    [SerializeField] private float baseJumpTime = 1;
    [SerializeField] private float baseWaitTime = 1;
    [SerializeField] private float gravity = -9.81f; // Simulated gravity

     private float speedInfluence = 1f;
     private int referenceSpeed = 4;


    [Header("Jiggle")]
    [SerializeField] float amplitude = 0.03f; // Initial strength of the jiggle
    [SerializeField] float decayRate = 0.1f;   // How quickly the jiggle fades
    [SerializeField] float frequency = 15f;

    private Vector3 originalScale;

    private void Start() {
        originalScale = this.transform.localScale;
        if (enemy == null ) enemy = this.transform.root.GetComponent<Enemy>();

        StartCoroutine(UpdateSlime());
    }

    private IEnumerator UpdateSlime() {
        while (true) {
            if (activated) {

                // Option 1: Multiplicative (most common & intuitive)
                float speedRatio = enemy.currentSpeed / referenceSpeed;
                float adjustedWaitTime = baseWaitTime / (1f + speedInfluence * (speedRatio - 1f));
                float adjustedJumpTime = baseJumpTime / (1f + speedInfluence * (speedRatio - 1f));


                yield return StartCoroutine(FlattenBeforeJump(adjustedWaitTime));
                StartCoroutine(JiggleWithoutBlendShape(adjustedWaitTime*2, -1));
                StartCoroutine(LerpPosition(this.transform.position, target.position, adjustedJumpTime, 1));
                yield return StartCoroutine(LerpRotation(this.transform.rotation, target.rotation, adjustedJumpTime, 2));
                StartCoroutine(JiggleWithoutBlendShape(adjustedWaitTime * 2, 2));
                yield return StartCoroutine(DontMove(this.transform.position, this.transform.rotation, Random.Range(0.5f, adjustedWaitTime*2)));
                //ResetScale();

            } else {
                yield return new WaitForSeconds(1);
            }
        }
    }

    private IEnumerator LerpPosition(Vector3 startValue, Vector3 endValue, float lerpDuration, int mode) {

        float timeElapsed = 0;
        Vector3 startValueFrozen = startValue;
        Vector3 endValueFrozen = endValue;
        Vector3 jump = Vector3.zero;
        while (timeElapsed < lerpDuration) {
            float t = timeElapsed / lerpDuration;
            if (mode == 1) t = Mathf.Sin(t * Mathf.PI * 0.5f);
            if (mode == 2) t = Mathf.SmoothStep(0, 1, t);            

            this.transform.position = Vector3.Slerp(startValueFrozen, endValueFrozen, t);
            jump.y = 0.5f * gravity * lerpDuration * timeElapsed - 0.5f * gravity * timeElapsed * timeElapsed;
            this.transform.position -= jump;
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        this.transform.position = endValue;      
    }

    private IEnumerator LerpRotation(Quaternion startValue, Quaternion endValue, float lerpDuration, int mode) {

        float timeElapsed = 0;
        Quaternion startValueFrozen = startValue;
        Quaternion endValueFrozen = endValue;
        while (timeElapsed < lerpDuration) {
            float t = timeElapsed / lerpDuration;
            if (mode == 1) t = Mathf.Sin(t * Mathf.PI * 0.5f);
            if (mode == 2) t = Mathf.SmoothStep(0, 1, t);

            this.transform.rotation = Quaternion.Slerp(startValueFrozen, endValueFrozen, t);
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        this.transform.rotation = endValue;
    }

    private IEnumerator DontMove(Vector3 startPosition, Quaternion startRotation, float duration) {

        float timeElapsed = 0;
        Vector3 startPositionFrozen = startPosition;
        Quaternion startRotationFrozen = startRotation;

        while (timeElapsed < duration) {
            this.transform.SetPositionAndRotation(startPositionFrozen, startRotationFrozen);
            timeElapsed += Time.deltaTime;
            yield return null;
        }        
    }

    private IEnumerator FlattenBeforeJump( float duration) {

        float timeElapsed = 0;
        Vector3 startPositionFrozen = this.transform.position;
        Quaternion startRotationFrozen = this.transform.rotation;

        while (timeElapsed < duration) {
            float jiggle = 2 * amplitude * timeElapsed / duration;// when timeElapsed==duration -> jiggle = aplitude
            AffectScale(jiggle);
            this.transform.SetPositionAndRotation(startPositionFrozen, startRotationFrozen);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator JiggleWithoutBlendShape(float duration, float coefficient) {

        float timeElapsed = 0;

        while (timeElapsed < duration) {
            float jiggle = coefficient * amplitude * Mathf.Exp(-decayRate * timeElapsed) * Mathf.Sin(frequency * timeElapsed);

            AffectScale(jiggle);

            timeElapsed += Time.deltaTime;            
            yield return null;
        }
    }

    private void AffectScale ( float jiggle) {        
        this.transform.localScale = new Vector3(jiggle / 2, -jiggle, jiggle / 2) + originalScale; // + or * ?
    }

    private void AffectScaleOnTop(float jiggle) {
        //Debug.Log("jiggle : " + jiggle);
        this.transform.localScale += new Vector3(jiggle / 2, -jiggle, jiggle / 2) ; // + or * ?
    }

    private void ResetScale( ) {
        Debug.Log("Starting ResetScale");
        this.transform.localScale = originalScale; 
    }
}
