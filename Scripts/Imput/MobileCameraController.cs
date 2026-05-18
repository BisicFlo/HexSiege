using UnityEngine;
using UnityEngine.InputSystem;

public class MobileCameraController : MonoBehaviour {

    // --------------------------------------------------------------
    //   Inspector Fields
    // -------------------------------------------------------------- 

    public InputActionAsset InputActions; // New Input System

    [Header("Translation")]
    [SerializeField] private float translationSpeed = 0.25f; // screen space -> world space sensitivity

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 8f;
    [SerializeField] private float minFOV = 20f; // perspective camera
    [SerializeField] private float maxFOV = 90f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 3f; // Degrees per screen pixel dragged
    
    [SerializeField] private Transform cameraPivot;

    // --------------------------------------------------------------
    //   Private 
    // --------------------------------------------------------------
    
    private Camera cam;

    private InputAction clickAction;       // Click one finger
    private InputAction deltaAction;       // delta finger 1
    private InputAction delta2Action;      // Delta finger 2
    private InputAction positionAction;    // Position 1st finger
    private InputAction position2Action;   // Position 2nd finger
    private InputAction twoFingersAction;  // true if 2 fingers

    private Vector2 deltaFinger1;
    private Vector2 deltaFinger2;
    private Vector2 positionFinger1;
    private Vector2 positionFinger2;
    private Vector2 offsetFingers;
    private bool twoFingersOneScreen;
    private float sqrPreviousMagnitude;

    private float sqrCurrentMagnitude; // Used In Zoom

    private bool skipNextDelta2 = false; //new

    // --------------------------------------------------------------
    //   MonoBehaviour
    // --------------------------------------------------------------

    void Awake() {
        cam = Camera.main;

        clickAction = InputActions.FindAction("Click");
        deltaAction = InputActions.FindAction("Delta");
        delta2Action = InputActions.FindAction("Delta2");
        positionAction = InputActions.FindAction("Position"); // ?
        position2Action = InputActions.FindAction("Position2"); // ?
        twoFingersAction = InputActions.FindAction("TwoFingers");
    }
    void OnEnable() {
        //clickAction.performed += ClickPerformed;
        deltaAction.performed += DeltaPerformed;
        twoFingersAction.performed += TwoFingersPerformed;
        twoFingersAction.canceled += TwoFingersPerformed;
        delta2Action.performed += Delta2Performed;
        //clickAction.canceled += ClickCanceled;
        twoFingersAction.canceled += TwoFingersCanceled;
    }

    void OnDisable() {
        //clickAction.performed -= ClickPerformed;
        deltaAction.performed -= DeltaPerformed;
        twoFingersAction.performed -= TwoFingersPerformed;
        delta2Action.performed -= Delta2Performed;
        //clickAction.canceled -= ClickCanceled;
        twoFingersAction.canceled -= TwoFingersCanceled;
    }

    private void DeltaPerformed(InputAction.CallbackContext context) {
        deltaFinger1 = context.ReadValue<Vector2>();
        if (twoFingersOneScreen) return;
        sqrPreviousMagnitude = 0;
        // ------------------------------------------------
        //                     Translation
        // ------------------------------------------------
        Vector3 move = new Vector3(-deltaFinger1.x * translationSpeed * 0.1f , 0, -deltaFinger1.y * translationSpeed * 0.1f); // -> Removed "* Time.deltaTime"

        cameraPivot.transform.Translate(move, Space.Self);
    }

    private void TwoFingersPerformed(InputAction.CallbackContext context) {
        //float touchValue = context.ReadValue<float>();
        twoFingersOneScreen = true;
        skipNextDelta2 = true;
        sqrPreviousMagnitude = 0;// 180526
    }
    private void TwoFingersCanceled(InputAction.CallbackContext context) {
        //float touchValue = context.ReadValue<float>();
        twoFingersOneScreen = false;
        skipNextDelta2 = true;
        sqrPreviousMagnitude = 0; // new 060526

    }
    private void Delta2Performed(InputAction.CallbackContext context) {
        // No need to verify twoFingersOneScreen because Delta2Performed() is called when there are 2 fingers
        deltaFinger2 = context.ReadValue<Vector2>();
        // ------------------------------------------------
        //                     Rotation
        // ------------------------------------------------
        float avgDeltaX = (deltaFinger1.x + deltaFinger2.x) * 0.5f;

        float rotateAmount = avgDeltaX * rotationSpeed; 
        cameraPivot.transform.Rotate(0f, rotateAmount, 0f, Space.World);

        if (skipNextDelta2) {
            skipNextDelta2 = false;
            sqrPreviousMagnitude = 0;
            // Still update deltaFinger1 so it's fresh for next frame
            return;
        }
        // ------------------------------------------------
        //                      Zoom
        // ------------------------------------------------

        positionFinger1 = positionAction.ReadValue<Vector2>();
        positionFinger2 = position2Action.ReadValue<Vector2>();
        //positionFinger2 = new Vector2(0,0); // used to debug on pc
        offsetFingers = positionFinger2 - positionFinger1;
        //Debug.Log("- - positionFinger1 : " + positionFinger1 + "| positionFinger2 : " + positionFinger2);   

        sqrCurrentMagnitude = offsetFingers.SqrMagnitude();

        //Debug.Log("sqrPreviousMagnitude : " + sqrPreviousMagnitude + "| sqrCurrentMagnitude : " + sqrCurrentMagnitude);
        if (sqrPreviousMagnitude > 0.01f) { 

            float delta = sqrPreviousMagnitude - sqrCurrentMagnitude;

            if (true) {
                if (delta > 5000 || delta < -5000) Debug.Log("Delta : " + delta + "     [-!-]");
                else Debug.Log("Delta : " + delta);
                // Perspective camera
                delta = Mathf.Clamp(delta, -5000f, 5000f); // spike guard // New

                float newFOV = cam.fieldOfView + (delta * zoomSpeed * 0.001f);// -> Removed "* Time.deltaTime"
                cam.fieldOfView = Mathf.Clamp(newFOV, minFOV, maxFOV);
            }
        }

        sqrPreviousMagnitude = sqrCurrentMagnitude;
    }
}