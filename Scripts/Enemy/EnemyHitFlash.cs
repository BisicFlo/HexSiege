using System.Collections;
using UnityEngine;

public class EnemyHitFlash : MonoBehaviour {
    //[SerializeField] private Renderer _renderer;
    [SerializeField] private float _flashDuration = 0.1f;

    [SerializeField] private Color _flashColor = Color.white;

    private MaterialPropertyBlock _mpb;
    private Coroutine _flashRoutine;
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    private Renderer[] _renderers;

    void Awake() {
        _renderers = GetComponentsInChildren<Renderer>();

        _mpb = new MaterialPropertyBlock();

        //StartCoroutine(TEst());
    }

    public void TriggerFlash() {
        if (_flashRoutine != null) StopCoroutine(_flashRoutine);
        _flashRoutine = StartCoroutine(FlashRoutine());
    }

    //private IEnumerator TEst() {
    //    while (true) {
    //        yield return new WaitForSeconds(4f);
    //        if (_flashRoutine != null) StopCoroutine(_flashRoutine);
    //        _flashRoutine = StartCoroutine(FlashRoutine());
    //    }
    //}



private IEnumerator FlashRoutine() {

        foreach (var r in _renderers) {
            r.GetPropertyBlock(_mpb);
            _mpb.SetColor(EmissionColorID, _flashColor * 3f);
            r.SetPropertyBlock(_mpb);
        }

        yield return new WaitForSeconds(_flashDuration);

        foreach (var r in _renderers) {
            r.GetPropertyBlock(_mpb);
            _mpb.SetColor(EmissionColorID, Color.black);
            r.SetPropertyBlock(_mpb);
        }
        _flashRoutine = null;
    }
}
