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
    //private bool isActive = false;

    protected bool spikeIsActive = false;
    protected int damage;
    protected int speed; // affects other speed variables
    protected bool isCritical; //new
    protected bool isCursed;  //new

    protected Transform target;
    private GameObject impact; // instance of _impactPrefab
    protected Enemy enemy;

    // need to know if enemy is grounded ?

    protected Turret turret; // turret that created this Spike / used for events
    public void Init(Turret turret, Transform target, Enemy enemy, int damage, int speed, bool isCritical, bool isCursed) {
        this.turret = turret;
        this.target = target;
        this.enemy = enemy;
        this.damage = damage;
        this.speed = speed;
        this.isCritical = isCritical;
        this.isCursed = isCursed;
        initialScale = initialScale = transform.localScale;
    }

    public void ActivateSpikeAndDesactivateImpact() {
        spikeIsActive = true;
        if (impact != null) impact.SetActive(false);
    }

    private void Awake() {  // Start() is not called if the object is instantiated Inactive 
        if (_impactPrefab != null) impact = Instantiate(_impactPrefab);

        if (impact != null) impact.SetActive(false);
    }

    public void Erupt(Vector3 position) {
        //if (_spikeIsActive) return;
        if (position == Vector3.zero) return;

        // Position the spike at the target location 
        this.transform.position = new Vector3(position.x, 0, position.z);

        // Start Animation + effects + damage
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
        target = null;
        spikeIsActive = false;
        this.gameObject.SetActive(false);
    }

    public void HitTarget() {
        if (enemy != null) {
            //if (turret == null) Debug.Log("Turret is Null in : " + this.gameObject.name);
            //else Debug.Log("Turret :" + turret.gameObject.name);

            GameEvents.EnemyHit(turret, enemy); // new
            enemy.TakeDamage(turret, damage);

            if (isCursed) {
                enemy.TakeCurse(turret);
            }
            if (isCritical) {

            }
        }
        if (impact != null) {
            impact.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
            impact.SetActive(true);
        }
    }
}
