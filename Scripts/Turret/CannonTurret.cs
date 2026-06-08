using System.Collections;
using UnityEngine;

public class CannonTurret : Turret {

    // --------------------------------------------------------------
    //   Inspector Fields
    // -------------------------------------------------------------- 

    [Header("Setup")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform yawTransform; // The base that rotates left/right (Y axis)
    [SerializeField] private Transform pitchTransform; // The barrel/pivot that rotates up/down (local X axis)
    [SerializeField] private Transform barrelTransform;
    [SerializeField] private GameObject bulletPrefab = null;

    [Header("Attributes")]
    [SerializeField] private float yawSpeed = 180f;      // degrees per second (horizontal)
    [SerializeField] private float pitchSpeed = 180f;    // degrees per second (vertical)
    [SerializeField] private float maxPitchUp = 60f;     // max elevation (positive = up)
    [SerializeField] private float maxPitchDown = 30f;   // max depression (positive value = how far down)

    [Header("Configs")]
    [SerializeField] private bool needReloadAnimation; 
    [SerializeField] private GameObject Config1;
    [SerializeField] private GameObject Config2;
    [SerializeField] private GameObject Config3;

    // --------------------------------------------------------------
    //   Private 
    // --------------------------------------------------------------

    //private float countdown = 2f;  
    private int waveIndex = 0;
    private int bulletIndex = 0;
    private int impactIndex = 0;
    //private float fireCountdown = 0;
    private Projectile[] bulletArray;
    private GameObject[] impactArray;
    private WaitForSeconds waitBetweenUpdateTarget;
    private Vector3 BarrelOffset;
    [SerializeField] private bool targetInSight;
    [SerializeField] private bool targetAlmostInSight;
    [SerializeField] private float dotProductdotProduct;
    bool reloading = false;

    // --------------------------------------------------------------
    //   MonoBehaviour
    // --------------------------------------------------------------


    protected override void Init() {
        base.Init();
        BarrelOffset = barrelTransform.position - pitchTransform.position; // Cannon
        if (bulletPrefab != null) bulletArray = CreateStockOf<Projectile>(bulletPrefab, 5);
    }

    private void Update() {
        if (target == null) return;

        AimAtTarget();

        if (CheckIfReadyToFire()) {

            CheckIfTargetIsInSight();

            if (targetInSight) {

                Attack();
            }
        }
    }

    private void CheckIfTargetIsInSight() { // GetDotProductWithTarget
        if (target == null) return;

        float dotProduct = Vector3.Dot(firePoint.forward, (target.position - firePoint.position).normalized); // normalized !

        dotProductdotProduct = dotProduct;

        //return dotProduct;

        if (dotProduct > 0.99f) targetInSight = true;
        else targetInSight = false;

        if (dotProduct > 0.8f) targetAlmostInSight = true;
        else targetAlmostInSight = false;

    }

    private void AimAtTarget() {
        // _______________________________________
        // 1. Yaw (horizontal rotation) - rotate whole base toward target (flatten Y)
        // _______________________________________
        Vector3 directionToTarget = target.position - yawTransform.position;
        directionToTarget.y = 0; // flatten ? ignore height difference

        if (directionToTarget.sqrMagnitude > 0.001f) // avoid zero vector
        {
            Quaternion targetYawRot = Quaternion.LookRotation(directionToTarget, Vector3.up);
            yawTransform.rotation = Quaternion.RotateTowards(
                yawTransform.rotation,
                targetYawRot,
                yawSpeed * Time.deltaTime
            );
        }

        // ______________________________________
        // 2. Pitch (vertical rotation) - now in local space of the barrel pivot
        // ______________________________________

        if (!targetAlmostInSight) return; // 

        Vector3 toTarget = target.position - pitchTransform.position - BarrelOffset;

        // Project direction onto the plane perpendicular to yaw axis (usually world Y)
        Vector3 flatForward = yawTransform.forward;
        flatForward.y = 0;
        flatForward.Normalize();

        Vector3 right = Vector3.Cross(Vector3.up, flatForward); // usually world right-ish

        // Now get pitch angle in local space
        float forwardDist = Vector3.Dot(toTarget.normalized, flatForward);
        float upDist = Vector3.Dot(toTarget.normalized, Vector3.up);

        float targetPitch = Mathf.Atan2(upDist, forwardDist) * Mathf.Rad2Deg;
        targetPitch = -targetPitch;

        // Clamp
        //targetPitch = Mathf.Clamp(targetPitch, -maxPitchDown, maxPitchUp);

        Quaternion targetRot = Quaternion.Euler(targetPitch, 0, 0);

        pitchTransform.localRotation = Quaternion.RotateTowards(
            pitchTransform.localRotation,
            targetRot,
            pitchSpeed * Time.deltaTime
        );

        //// ????????????????????????????????????????
        //// 2. Pitch – aim the BARREL, not the pivot
        //// ????????????????????????????????????????
        //// Define barrel muzzle / fire point in world space
        //// (you can also make this a public Transform if you want to tweak it in editor)
        //Vector3 barrelMuzzlePosition = pitchTransform.position + pitchTransform.forward * barrelOffsetForward + pitchTransform.up * barrelOffsetUp;

        //// Or simpler – if the offset is mostly just upward in local space:
        //// Vector3 barrelMuzzlePosition = pitchTransform.TransformPoint(new Vector3(0, barrelHeightOffset, barrelForwardOffset));

        //Vector3 toTargetFromBarrel = target.position - barrelMuzzlePosition;

        //// Project onto the plane perpendicular to yaw axis (for clean pitch)
        //Vector3 flatForward = yawTransform.forward;
        //flatForward.y = 0;
        //flatForward.Normalize();

        //// Local pitch components
        //float forwardDist = Vector3.Dot(toTargetFromBarrel.normalized, flatForward);
        //float upDist = Vector3.Dot(toTargetFromBarrel.normalized, Vector3.up);

        //float targetPitch = Mathf.Atan2(upDist, forwardDist) * Mathf.Rad2Deg;
        //targetPitch = -targetPitch; // ? keep your sign convention

        //// Optional: clamp
        //// targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);

        //Quaternion targetRot = Quaternion.Euler(targetPitch, 0, 0);

        //pitchTransform.localRotation = Quaternion.RotateTowards(
        //    pitchTransform.localRotation,
        //    targetRot,
        //    pitchSpeed * Time.deltaTime
        //);
    }



    protected override void Shoot() {
        bool isCritical = IsCritical();
        bool isCursed = IsCursed();
        int attackDamage = AttackDamage.Value;
        if (isCritical) attackDamage *= CriticalDamage.Value;


        if (!reloading && needReloadAnimation) { // For Crossbows / bows
            StartCoroutine(ReloadAnimation());
        }      

        Projectile myBullet = GetObjectFromIndex<Projectile>(bulletArray, bulletIndex);
        bulletIndex++;
        bulletIndex %= bulletArray.Length; 

        if (myBullet.gameObject.activeSelf) { // NullReferenceException x2
            Debug.Log("Bullet Already In Used ");
            myBullet.HitTarget();
        }

        if (target != null) {
            InstantiateAlternative(myBullet.gameObject, firePoint.position, firePoint.rotation, Vector3.one, null);

            myBullet.Init(this, target, enemyTargetted, attackDamage, ProjectileSpeed.Value, isCritical, isCursed);
            myBullet.ActivateBulletAndDesactivateImpact();
        }
    }


    private IEnumerator ReloadAnimation() {
        reloading = true;
        Config1.SetActive(true);
        Config2.SetActive(false);
        Config3.SetActive(false);

        yield return new WaitForSeconds(40f / AttackSpeed.Value); // time between two attacks : 100f/ AttackSpeed.Value

        Config1.SetActive(false);
        Config2.SetActive(true);
        Config3.SetActive(false);

        yield return new WaitForSeconds(20f / AttackSpeed.Value);
        Config1.SetActive(false);
        Config2.SetActive(false);
        Config3.SetActive(true);

        reloading = false;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(Range.Value)); // Mathf.Sqrt because we use sqrMagnitude
    }
}
