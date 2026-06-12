
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TouchManager : MonoBehaviour {

    // --------------------------------------------------------------
    //   Inspector Fields
    // -------------------------------------------------------------- 

    public InputActionAsset InputActions;   

    [SerializeField] private float maxRayDistance = 100f;

    // --------------------------------------------------------------
    //   Private 
    // --------------------------------------------------------------

    private GameObject selectedObject;
    
    private InputAction clickAction;       // Click one finger
    private InputAction deltaAction;       // delta finger 1
    private InputAction delta2Action;      // Delta finger 2
    private InputAction positionAction;    // Position one finger
    private InputAction twoFingersAction;  // true if 2 fingers
   
    private  List<RaycastResult> uiResults = new List<RaycastResult>(); // For UI detection

    // --------------------------------------------------------------
    //   MonoBehaviour
    // --------------------------------------------------------------

    private void Awake() {
        clickAction = InputActions.FindAction("Click");
        deltaAction = InputActions.FindAction("Delta");
        //delta2Action = InputActions.FindAction("Delta2");
        positionAction = InputActions.FindAction("Position");         // needed
        //twoFingersAction = InputActions.FindAction("TwoFingers");
    }

    private void OnEnable() {
        clickAction.performed += ClickPerformed;
        clickAction.canceled += ClickCanceled;
        deltaAction.performed += DeltaPerformed;
    }

    private void OnDisable() {
        clickAction.performed -= ClickPerformed;
        clickAction.canceled -= ClickCanceled;
        deltaAction.performed -= DeltaPerformed;
    }

    private void ClickPerformed(InputAction.CallbackContext context) {
        Vector2 screenPos = positionAction.ReadValue<Vector2>(); // We can get the value at any time       
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
                
        if (IsPointerOverUI(screenPos)) {// Check if the touch is over ANY UI element
            //Debug.Log("UI Detected");  
            return;
        }
        
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance)) { // Check if the touch a tile // add hitLayers ?
            GameObject clickedObject = hit.collider.gameObject;

            selectedObject = clickedObject;

        } else {
            //Debug.Log("Clicked on nothing (ray didn't hit anything)");
        }
    }

    private void ClickCanceled(InputAction.CallbackContext context) {

        Vector2 screenPos = positionAction.ReadValue<Vector2>();

        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance)) {
            GameObject ReleaseObject = hit.collider.gameObject;

            if (ReleaseObject == selectedObject) {

                if (ReleaseObject.CompareTag("Ground")) {
                    Debug.Log("Ground");

                    if (ReleaseObject.TryGetComponent<Node>(out var node)) {

                        if (node.IsEmpty()) { // If node empty -> Build
                            OpenBuildMenu();
                            SetSelectedTile(ReleaseObject);
                        }
                        else { //  If node NOT empty -> Inspect Turret
                            InspectTurretManager.Instance.selectedTurret = node.TurretOnTop;
                            OpenTurretInspectorMenu();
                            InspectTurretManager.Instance.DisplayTurret();
                        }
                    }
                }
                else if (ReleaseObject.CompareTag("Turret")) {
                    // Redundant but sometime the turret is outside of the node collider : the user clics on the turret not the node 
                    Debug.Log("Turret");
                    InspectTurretManager.Instance.selectedTurret = ReleaseObject.GetComponent<Turret>();
                    OpenTurretInspectorMenu();
                    InspectTurretManager.Instance.DisplayTurret();
                }
                else if (ReleaseObject.CompareTag("Enemy")) {
                    //OpenEnemyInspectorMenu();
                    // ..... WorkInProgress
                }
                //Debug.Log($"Clicked on: {clickedObject.name}  (tag: {clickedObject.tag})", clickedObject);
            }
        }
        else {
            //Debug.Log("Clicked on nothing (ray didn't hit anything)");
        }
    }

    private void DeltaPerformed(InputAction.CallbackContext context) {
        // If we move outside of the Deadzone : 5 -> cancel everything by : selectedObject=null
        Vector2 delta = context.ReadValue<Vector2>();
        if (delta.sqrMagnitude < 5) return;
        selectedObject = null;
    }

    void OpenBuildMenu() {
        UIManager.Instance.ShowScreen(ScreenType.Build);
    }

    void OpenTurretInspectorMenu() {
        UIManager.Instance.ShowScreen(ScreenType.InspectTurret);
    }

    void OpenEnemyInspectorMenu() {
        UIManager.Instance.ShowScreen(ScreenType.InspectEnemy); // WorkInProgress
    }

    void SetSelectedTile(GameObject tile) {
        BuildManager.Instance.SelectedTile = tile;
    }

    void SetSelectedTurret(GameObject turret) {

    }

    private bool IsPointerOverUI(Vector2 screenPosition) {
        PointerEventData eventData = new PointerEventData(EventSystem.current) {
            position = screenPosition
        };

        uiResults.Clear();
        EventSystem.current.RaycastAll(eventData, uiResults);
        return uiResults.Count > 0;
    }
}


