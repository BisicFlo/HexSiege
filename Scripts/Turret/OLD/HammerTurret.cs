
//using System.Collections;
//using UnityEngine;

//public class HammerTurret : Turret {

//    [Tooltip("Speed of the swing animation.")]
//    public float swingSpeed = 5f;

//    [Tooltip("Angle to swing down (e.g., 90 degrees for a full downward crush).")]
//    public float downAngle = 90f;

//    [Tooltip("Delay between swinging down and swinging back up (e.g., for impact effect).")]
//    public float impactDelay = 0.2f;

//    private Quaternion startRotation;
//    private bool isSwinging = false;

//    [SerializeField] private bool StartSwing = false;

//    void Start() {
//        // Store the initial "up" rotation.
//        startRotation = transform.localRotation;
//    }
//    private void Update() {
//        if (StartSwing) {
//            StartSwing = false;
//            Swing();
//        }
//    }

//    /// <summary>
//    /// Public method to trigger the hammer swing. Call this from your tower script when attacking.
//    /// </summary>
//    public void Swing() {
//        if (!isSwinging) {
//            StartCoroutine(SwingCoroutine());
//        }
//    }

//    private IEnumerator SwingCoroutine() {
//        isSwinging = true;

//        // Calculate the target "down" rotation (assuming rotation around local X-axis).
//        Quaternion targetDown = startRotation * Quaternion.Euler(downAngle, 0, 0);

//        // Swing down smoothly.
//        while (Quaternion.Angle(transform.localRotation, targetDown) > 0.1f) {
//            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetDown, Time.deltaTime * swingSpeed);
//            yield return null;
//        }

//        // Optional: Pause at the bottom for impact (e.g., play sound or particle effect here).
//        yield return new WaitForSeconds(impactDelay);

//        // Swing back up smoothly.
//        while (Quaternion.Angle(transform.localRotation, startRotation) > 0.1f) {
//            transform.localRotation = Quaternion.Slerp(transform.localRotation, startRotation, Time.deltaTime * swingSpeed);
//            yield return null;
//        }

//        // Reset rotation precisely and allow next swing.
//        transform.localRotation = startRotation;
//        isSwinging = false;
//    }
//}

