using UnityEngine;


public class SimpleMortarTurret : Turret {

    // --------------------------------------------------------------
    //   Inspector Fields
    // -------------------------------------------------------------- 

    [Header("Setup")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private bool faceEnemyWhenAttacking = false;  // Toggle to enable facing enemy
    [SerializeField] private bool blockY = false;                 // Block Y while rotating
    [SerializeField] private float rotationSpeed = 180f;         // Optional
    [SerializeField] private Transform rotationTransform;       // Optional / The base that rotates left/right (Y axis)

    [Header("Mortar")]
    [SerializeField] private GameObject bulletPrefab = null; // Prefab of the projectile 
    [SerializeField] private float flightTime = 1;   
    [SerializeField] private float maxHeight = 10;

    //[Header("Animation")]
    //[SerializeField] private AnimateTurret animateTurret;

    // --------------------------------------------------------------
    //   Private 
    // --------------------------------------------------------------

    private int bulletIndex = 0;
    private MortarBullet[] bulletArray;

    // --------------------------------------------------------------
    //   MonoBehaviour
    // --------------------------------------------------------------

    protected override void Init() {
        base.Init();
        if (bulletPrefab != null) bulletArray = CreateStockOf<MortarBullet>(bulletPrefab, 5);
    }

    void Update() {
        if (target == null) return;

        if (CheckIfReadyToFire()){

            if (faceEnemyWhenAttacking) {
                AimAtTarget();

                if (CheckIfTargetIsInSight()) {
                    Attack();
                }
            }
            else {
                Attack(); // Turret.cs
            }
        }
    }

    private bool CheckIfTargetIsInSight() {
        if (target == null) return false;

        float dotProduct = Vector3.Dot(rotationTransform.forward, (target.position - rotationTransform.position).normalized);

        if (dotProduct > 0.99f) return true;
        else return false;
    }
    private void AimAtTarget() {
        Vector3 dir = target.position - rotationTransform.transform.position;
        if (blockY) dir.y = 0;  // Avoid tilting if BlockY = true 
        if (dir != Vector3.zero) {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            rotationTransform.transform.rotation = Quaternion.Slerp(rotationTransform.transform.rotation, lookRot, Time.deltaTime * rotationSpeed);
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

        MortarBullet myBullet = GetObjectFromIndex<MortarBullet>(bulletArray, bulletIndex);
        bulletIndex++;
        bulletIndex %= bulletArray.Length; // Redundant ?

        InstantiateAlternative(myBullet.gameObject, firePoint.position, firePoint.rotation, Vector3.one, null);
        if (base.target != null) {
            myBullet.Init(this, base.target, enemyTargetted, AttackDamage.Value, ProjectileSpeed.Value, flightTime, maxHeight);
            myBullet.ActivateBulletAndDesactivateImpact();
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(Range.Value)); // Mathf.Sqrt because we use sqrMagnitude
    }
}


