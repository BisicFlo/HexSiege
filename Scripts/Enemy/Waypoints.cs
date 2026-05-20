using UnityEngine;

/// <summary>
/// Manages a collection of waypoints for enemy pathfinding.
/// Attach this script to a parent GameObject that contains multiple child objects (waypoints).
/// The waypoints are stored in a static array for easy global access.
/// </summary>
public class Waypoints : MonoBehaviour {

    public static Transform[] points;

    private void Awake() {
 
        points = new Transform[transform.childCount];
        
        for (int i = 0; i < points.Length; i++) {
            points[i] = transform.GetChild(i);
        }

        if (points.Length == 0) {
            Debug.LogWarning($"No waypoints found under {gameObject.name}!", this);
        }
    }

    // Visual debugging in Scene view
    private void OnDrawGizmos() {
        if (transform.childCount < 2) return;

        Gizmos.color = Color.cyan;

        for (int i = 0; i < transform.childCount - 1; i++) {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }
    }
}
