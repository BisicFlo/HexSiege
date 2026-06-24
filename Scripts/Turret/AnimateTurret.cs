using System.Collections;
using UnityEngine;

public class AnimateTurret : MonoBehaviour {

    // --------------------------------------------------------------
    //   Inspector Fields
    // -------------------------------------------------------------- 

    [Header("Jiggle")]
    [SerializeField] float amplitude = 0.1f; // Initial strength of the jiggle
    [SerializeField] float decayRate = 4f;   // How quickly the jiggle fades
    [SerializeField] float frequency = 15f;

    public enum AxisType { X, Y, Z };
    [Tooltip("Axis in local space of the jiggle direction")]
    public AxisType Axis;

    [SerializeField] bool reverse = false;

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

    public IEnumerator Jiggle(System.Action onAttack, int attackSpeed) {

        yield return StartCoroutine(FlattenBeforeJump(40f / attackSpeed));

        onAttack?.Invoke();

        StartCoroutine(JiggleWithoutBlendShape(40f / attackSpeed, -1)); // add attack Speed
    }

    private IEnumerator FlattenBeforeJump(float duration) {
        float timeElapsed = 0;

        while (timeElapsed < duration) {
            float jiggle = 2 * amplitude * timeElapsed / duration;// when timeElapsed==duration -> jiggle = aplitude
            AffectScale(jiggle);
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

    private void AffectScale(float jiggle) {
        if (reverse) jiggle = -jiggle;

        if (Axis == AxisType.X)
            this.transform.localScale = new Vector3(-jiggle, jiggle / 2, jiggle / 2) + originalScale; // + or * ?
        else if (Axis == AxisType.Y)
            this.transform.localScale = new Vector3(jiggle / 2, -jiggle, jiggle / 2) + originalScale; // + or * ?
        else if (Axis == AxisType.Z)
            this.transform.localScale = new Vector3(jiggle / 2, jiggle / 2, -jiggle) + originalScale; // + or * ?
    }

}
