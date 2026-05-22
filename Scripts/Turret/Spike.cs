using System.Collections;
using UnityEngine;

public class Spike : MonoBehaviour {

    public GameObject _impactPrefab = null;

    [Header("Spike Settings")]

    [Tooltip("Full height the spike will reach (local Y scale)")]
    [SerializeField] private float maxHeight = 4f;

    [Tooltip("How long the spike takes to rise")]
    [SerializeField] private float riseTime = 0.3f;

    [Tooltip("How long the spike stays fully extended")]
    [SerializeField] private float holdTime = 0.8f;

    [Tooltip("How long the spike takes to retract")]
    [SerializeField] private float retractTime = 0.4f;

    private Vector3 initialScale;
    private bool isActive = false;

    protected bool spikeIsActive = false;
    protected int damage;
    protected int speed; // affects other speed variables
    protected Transform _target;
    private GameObject _impact; // instance of _impactPrefab
    protected Enemy _enemy;

    // need to know if enemy is grounded ?

    protected Turret _turret; // turret that created this Spike / used for events

    public Spike(Turret turret) {
        _turret = turret;
    }

    public void Init(Transform target, Enemy enemy, int damage, int speed) {
        this._target = target;
        this._enemy = enemy;
        this.damage = damage;
        this.speed = speed;

        initialScale = initialScale = transform.localScale;
    }


    public void ActivateBulletAndDesactivateImpact() {
        spikeIsActive = true;
        if (_impact != null) _impact.SetActive(false);
    }

    private void Awake() {  // Start() is not called if the object is instantiated Inactive 
        if (_impactPrefab != null) _impact = Instantiate(_impactPrefab);

        if (_impact != null) _impact.SetActive(false);
    }


    public void Erupt(Vector3 position) {
        //if (_spikeIsActive) return;

        // Position the spike at the target location 
        this.transform.position = new Vector3(position.x, 0, position.z);

        StartCoroutine(SpikeEruptionRoutine());
    }

    private IEnumerator SpikeEruptionRoutine() {
        spikeIsActive = true;

        // === DAMAGE PHASE ===
        HitTarget();

        // === RISE PHASE ===
        float elapsed = 0f;
        Vector3 startScale = new Vector3(initialScale.x, 0.01f, initialScale.z);
        Vector3 targetScale = new Vector3(initialScale.x, maxHeight, initialScale.z);

        while (elapsed < riseTime) {
            elapsed += Time.deltaTime;
            float t = elapsed / riseTime * speed; // new : * speed
            // Optional: add a little ease out
            t = 1 - Mathf.Pow(1 - t, 3);

            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;

        // === HOLD PHASE ===
        yield return new WaitForSeconds(holdTime / speed);  // new : speed

        // === RETRACT PHASE ===
        elapsed = 0f;
        startScale = transform.localScale;
        targetScale = new Vector3(initialScale.x, 0.01f, initialScale.z);

        while (elapsed < retractTime) {
            elapsed += Time.deltaTime;
            float t = elapsed / retractTime * speed; // new : * speed
            // Ease in for retract
            t = Mathf.Pow(t, 2);

            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;

        // === DESACTIVATE PHASE ===
        _target = null;
        spikeIsActive = false;
        this.gameObject.SetActive(false);
    }

    public void HitTarget() {

        if (_enemy != null) {
            GameEvents.EnemyHit(_turret, _enemy); // new
            _enemy.TakeDamage(_turret, damage);
        }

        if (_impact != null) {
            _impact.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
            _impact.SetActive(true);
        }
    }
}
