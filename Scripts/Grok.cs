




//In my mobile unity game ,
//how can I add Camera Control , 
//like moving left/right , up/down , zoom-in / out , using touch screen ? I Already have this script : "


//    touchPositionAction = playerInput.actions.FindAction("Point");
//    touchPressAction = playerInput.actions.FindAction("Click");


//private void OnEnable() {
//    touchPressAction.performed += TouchePressed; // ".started" doesn't work
//}

//private void OnDisable() {
//    touchPressAction.performed -= TouchePressed;
//}

//private void TouchePressed(InputAction.CallbackContext context) {

//    float touchValue = context.ReadValue<float>();
//    if (touchValue > 0.5f) { // Pressing
//        Vector2 screenPos = touchPositionAction.ReadValue<Vector2>();

//        Ray ray = Camera.main.ScreenPointToRay(screenPos);

//        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, hitLayers)) {
//            GameObject clickedObject = hit.collider.gameObject;

//            if (clickedObject != lastObject) {
//                lastObject = clickedObject;

//                StartCoroutine(ChangeColor(clickedObject));
//                Debug.Log($"Clicked on: {clickedObject.name}  (tag: {clickedObject.tag})", clickedObject);

//            }
//        } else {
//            Debug.Log("Clicked on nothing (ray didn't hit anything)");
//        }
//    } else {  // Releasing
//              // Do Someting on clickedObject
//    }
//}

//"


