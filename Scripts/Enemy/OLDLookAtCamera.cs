using UnityEngine;

public class LookAtCameraOptimized : MonoBehaviour {
    private static Transform mainCamTransform;

    [SerializeField] private bool lockXAxis = true;

    private void OnEnable() {
        if (mainCamTransform == null) {
            mainCamTransform = Camera.main ? Camera.main.transform : FindAnyObjectByType<Camera>().transform;
        }

        Look();
    }

    private void LateUpdate() {
        Look();
    }

    private void Look() {
        if (mainCamTransform == null) return;

        Vector3 direction = mainCamTransform.position - transform.position;

        if (lockXAxis)
            direction.x = 0;

        transform.rotation = Quaternion.LookRotation(direction);
    }
}