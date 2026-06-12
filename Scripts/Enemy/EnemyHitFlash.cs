using System.Collections;
using UnityEngine;

/// <summary>
/// Change the color of an Object when hit , used with Enemies / Change the Emission Color
/// </summary>
public class EnemyHitFlash : MonoBehaviour {    

    [SerializeField] private Color _flashColor = Color.white;


    private MaterialPropertyBlock _mpb;
    private Coroutine _flashRoutine;
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    private Renderer[] _renderers;

    private static float _flashDuration = 0.1f;
    private WaitForSeconds waitDuration = new WaitForSeconds(_flashDuration); // Prevent allocations

    void Awake() {
        _renderers = GetComponentsInChildren<Renderer>();

        _mpb = new MaterialPropertyBlock();
    }

    public void TriggerFlash() {
        if (_flashRoutine != null) StopCoroutine(_flashRoutine);
        _flashRoutine = StartCoroutine(FlashRoutine()); // Coroutine couldn't be started because the the game object 'Enemy 39' is inactive!
    }

    private IEnumerator FlashRoutine() {

        foreach (var r in _renderers) {
            r.GetPropertyBlock(_mpb);
            _mpb.SetColor(EmissionColorID, _flashColor * 3f);
            r.SetPropertyBlock(_mpb);
        }

        yield return waitDuration;

        foreach (var r in _renderers) {
            r.GetPropertyBlock(_mpb);
            _mpb.SetColor(EmissionColorID, Color.black);
            r.SetPropertyBlock(_mpb);
        }
        _flashRoutine = null;
    }
}
