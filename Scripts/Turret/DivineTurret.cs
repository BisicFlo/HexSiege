using System.Collections;
using UnityEngine;

public class DivineTurret : Turret {

    [Header("Animation Settings")]
    public float liftHeight = 1.5f;
    public float totalDuration = 1.2f;

    [Range(0f, 1f)]
    public float liftPhaseRatio = 0.5f;   // % of time spent lifting + flipping
    [Range(0f, 1f)]
    public float apexPauseRatio = 0.1f;   // % of time held at apex
    // Strike phase gets the remainder

    [Header("Curves")]
    public AnimationCurve liftCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve strikeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 _startPos;
    private Quaternion _startRot;

    //[Header("Bobbing Settings")]
    //public Transform Artifact;
    //public Vector3 bobDirection = Vector3.up;
    //public float bobSpeed = 2f;      // Speed of up/down movement
    //public float bobHeight = 0.5f;   // Height amplitude of bobbing

    //[Header("Rotation Settings")]
    //public float rotateSpeed = 90f;  // Speed of constant rotation (degrees per second)
    //public Transform spinningChild;  // Assign the child model here in Inspector

    [Header("Projectile Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject ProjectilePrefab = null;

    [SerializeField] private bool salvo ; // NEw
    [SerializeField] private int salvoNumber; // NEw
    [SerializeField] private float salvoInterval; // NEw

    // --------------------------------------------------------------
    //   Private 
    // --------------------------------------------------------------

    private bool targetInSight = false;
    private Projectile[] ProjectileArray;
    private int ProjectileIndex = 0;
    private Transform artifactTransform;
    private Vector3 startLocalPos;
    private float phaseOffset;


    [ContextMenu("Play Strike")] private void PlayStrike() => StartCoroutine(StrikeCoroutine());

    protected override void Init() {
        base.Init();
        
        //artifactTransform = Artifact.transform;
        //startLocalPos = artifactTransform.localPosition;
        //phaseOffset = Random.Range(0f, Mathf.PI * 2);

        if (ProjectilePrefab != null) ProjectileArray = CreateStockOf<Projectile>(ProjectilePrefab, 5);
    }
    void Update() {

        //  bob up/down relative to start position

        //float bobOffset = Mathf.Sin((Time.time * bobSpeed) + phaseOffset) * bobHeight;

        //artifactTransform.localPosition = startLocalPos + bobDirection * bobOffset;

        ////  Rotate
        //if (spinningChild != null) {
        //    spinningChild.Rotate(0, rotateSpeed * Time.deltaTime, 0, Space.Self);
        //}

        if (target != null) {
            bool readyToFire = CheckIfReadyToFire();
            if (readyToFire) {
                Attack();
                StartCoroutine(StrikeCoroutine());
            }
        }
    }
    

    private IEnumerator ShootMultipleProjectiles(int projNumber, float  timeBetweenProj ) {
       WaitForSeconds time = new WaitForSeconds(timeBetweenProj);

        for (int i = 0; i < projNumber; i++) {
            ShootOneProjectile();
            yield return time;
        }
        
    }
    private void ShootOneProjectile() {
        bool isCritical = IsCritical();
        bool isCursed = IsCursed();
        int attackDamage = AttackDamage.Value;

        if (isCritical) attackDamage *= (int)CriticalDamage.Value;

        Projectile myProjectile = GetObjectFromIndex<Projectile>(ProjectileArray, ProjectileIndex);
        ProjectileIndex++;
        ProjectileIndex %= ProjectileArray.Length; // Redundant ?

        if (myProjectile.gameObject.activeSelf) {
            Debug.Log("Bullet Already In Used ");
            myProjectile.HitTarget();
        }

        if (target != null) {
            Vector3 directionTarget = (target.position - firePoint.position).normalized; // NullReferenceException: Obje...
            InstantiateAlternative(myProjectile.gameObject, firePoint.position, Quaternion.LookRotation(directionTarget, Vector3.up), Vector3.one, null); //firePoint.rotation

            myProjectile.Init(this, target, enemyTargetted, attackDamage, (int)ProjectileSpeed.Value, isCritical, isCursed);
            myProjectile.ActivateBulletAndDesactivateImpact();
        }
    }


    protected override void Shoot() {
        if (salvo) {
            StartCoroutine(ShootMultipleProjectiles(salvoNumber,salvoInterval));
        }
        else {
            ShootOneProjectile();
        }        
    }

    private IEnumerator StrikeCoroutine() {
        float liftDuration = totalDuration * liftPhaseRatio;
        float pauseDuration = totalDuration * apexPauseRatio;
        float strikeDuration = totalDuration * (1f - liftPhaseRatio - apexPauseRatio);

        Vector3 apexPos = _startPos + Vector3.up * liftHeight;
        Quaternion apexRot = _startRot * Quaternion.Euler(180f, 0f, 0f);
        Vector3 groundPos = _startPos + Vector3.down * liftHeight;
        Quaternion groundRot = apexRot; // already flipped

        // --- Phase 1: Lift & flip ---
        float t = 0f;
        while (t < liftDuration) {
            float n = t / liftDuration;                    // 0 -> 1 normalized
            float curve = liftCurve.Evaluate(n);

            transform.position = Vector3.Lerp(_startPos, apexPos, curve);
            transform.rotation = Quaternion.Slerp(_startRot, apexRot, curve);

            t += Time.deltaTime;
            yield return null;
        }

        transform.position = apexPos;
        transform.rotation = apexRot;

        // --- Phase 2: Apex pause ---
        yield return new WaitForSeconds(pauseDuration);

        // --- Phase 3: Strike (accelerates quickly) ---
        t = 0f;
        while (t < strikeDuration) {
            float n = t / strikeDuration;
            float curve = strikeCurve.Evaluate(n);

            transform.position = Vector3.Lerp(apexPos, groundPos, curve);
            transform.rotation = Quaternion.Slerp(apexRot, groundRot, curve);

            t += Time.deltaTime;
            yield return null;
        }

        transform.position = groundPos;
        transform.rotation = groundRot;

        // Optional: trigger impact VFX / sound here
        OnStrikeImpact();
    }

    private void OnStrikeImpact() {
        Debug.Log("Impact!");
        // Spawn particles, play sound, camera shake, etc.
    }


private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(Range.Value)); // Mathf.Sqrt because sqrMagnitude
    }
}
