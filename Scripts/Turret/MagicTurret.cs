using UnityEngine;

public class MagicTurret : Turret {

    [Header("Bobbing Settings")]
    public Transform Artifact;
    public Vector3 bobDirection = Vector3.up;
    public float bobSpeed = 2f;      // Speed of up/down movement
    public float bobHeight = 0.5f;   // Height amplitude of bobbing

    [Header("Rotation Settings")]
    public float rotateSpeed = 90f;  // Speed of constant rotation (degrees per second)
    public Transform spinningChild;  // Assign the child model here in Inspector

    [Header("Facing Settings")]
    public bool faceEnemyWhenAttacking = false;  // Toggle to enable facing enemy
    public bool BlockY = false;                  //

    [Header("Facing Smoothness")]
    public float faceSmoothSpeed = 10f;  // How quickly it rotates to face the enemy

    //[Header("Animation")]
    //[SerializeField] private AnimateTurret animateTurret;

    [Header("Projectile Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject ProjectilePrefab = null;

    // --------------------------------------------------------------
    //   Private 
    // --------------------------------------------------------------

    private bool targetInSight = false;
    private Bullet[] ProjectileArray;
    private int ProjectileIndex = 0;
    private Transform artifactTransform;
    private Vector3 startLocalPos;
    private float phaseOffset;

    protected override void Init() {
        base.Init();
        // Store initial local position relative to tower
        
        artifactTransform = Artifact.transform;
        startLocalPos = artifactTransform.localPosition;
        phaseOffset = UnityEngine.Random.Range(0f, Mathf.PI * 2);

        if (ProjectilePrefab != null) ProjectileArray = CreateStockOf<Bullet>(ProjectilePrefab, 5);
    }
    void Update() {
        // Always bob up/down relative to start position

        float bobOffset = Mathf.Sin((Time.time * bobSpeed) + phaseOffset) * bobHeight;

        artifactTransform.localPosition = startLocalPos + bobDirection * bobOffset;


        //if (axis == 0) {
        //    float newX = startLocalPos.x + (Mathf.Sin(Time.time * bobSpeed) * bobHeight / 20f);
        //    Artifact.transform.localPosition = new Vector3(newX, startLocalPos.y, startLocalPos.z);
        //} else if (axis == 1) {
        //    float newY = startLocalPos.y + (Mathf.Sin(Time.time * bobSpeed) * bobHeight / 20f);
        //    Artifact.transform.localPosition = new Vector3(startLocalPos.x, newY, startLocalPos.z);
        //} else if (axis == 2) {
        //    float newZ = startLocalPos.z + (Mathf.Sin(Time.time * bobSpeed) * bobHeight / 20f);
        //    Artifact.transform.localPosition = new Vector3(startLocalPos.x, startLocalPos.y, newZ);
        //}

        // Face enemy if enabled and target exists
        if (faceEnemyWhenAttacking && target != null) {

            AimAtTarget();

            bool readyToFire = CheckIfReadyToFire();

            if (readyToFire) {

                targetInSight = CheckIfTargetIsInSight();

                if (targetInSight) {
                    Attack();
                }
            }

        } else {

            if (spinningChild != null) {
                spinningChild.Rotate(0, rotateSpeed * Time.deltaTime, 0, Space.Self);
            }

            //if (faceEnemyWhenAttacking) Straighten(); // Avoid shifting rotation 

            if (target != null) {
                bool readyToFire = CheckIfReadyToFire();

                if (readyToFire) {
                    Attack();
                }
            }
        }
    }

    //private void Attack() {
    //    if (animateTurret != null) {
    //        StartCoroutine(animateTurret.Jiggle(Shoot, AttackSpeed.Value)); // Shoot Called after animation 
    //    }
    //    else {
    //        Shoot(); // no animation
    //    }
    //}

    protected override void Shoot() {
        bool isCritical = IsCritical();
        bool isCursed = IsCursed();
        int attackDamage = (int)AttackDamage.Value;

        if (isCritical) attackDamage *= (int)CriticalDamage.Value;
        //if (IsCursed()) attackDamage = 999; // add animation / visual

        Bullet myProjectile = GetObjectFromIndex<Bullet>(ProjectileArray, ProjectileIndex);
        ProjectileIndex++;
        ProjectileIndex %= ProjectileArray.Length; // Redundant ?

        if (myProjectile.gameObject.activeSelf) {
            Debug.Log("Bullet Already In Used ");
            myProjectile.HitTarget();
        }
        Vector3 directionTarget = (target.position - firePoint.position).normalized; // NullReferenceException: Obje...
        InstantiateAlternative(myProjectile.gameObject, firePoint.position, Quaternion.LookRotation(directionTarget, Vector3.up), Vector3.one, null); //firePoint.rotation
        if (target != null) {
            myProjectile.Init(this, target, enemyTargetted, attackDamage, (int)ProjectileSpeed.Value, isCritical, isCursed);
            myProjectile.ActivateBulletAndDesactivateImpact();
        }
    }

    private void AimAtTarget() {
        Vector3 dir = target.position - Artifact.transform.position;
        if (BlockY) dir.y = 0;  // Avoid tilting if BlockY = true 
        if (dir != Vector3.zero) {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            Artifact.transform.rotation = Quaternion.Slerp(Artifact.transform.rotation, lookRot, Time.deltaTime * faceSmoothSpeed);
        }
    }

    private void Straighten() { // Doesn't Work !
        if (Vector3.Angle(Artifact.transform.up, Vector3.up) > 1) {
            Artifact.transform.rotation = Quaternion.RotateTowards(Artifact.transform.rotation, Quaternion.LookRotation(Artifact.transform.forward, Vector3.up), 220f * Time.deltaTime);
        } 
    }

    private bool CheckIfTargetIsInSight() {
        if (target == null) return false;

        float dotProduct = Vector3.Dot(Artifact.forward, (target.position - Artifact.position).normalized); 

        if (dotProduct > 0.99f) return true;
        else return false;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(Range.Value)); // Mathf.Sqrt because sqrMagnitude
    }
}
