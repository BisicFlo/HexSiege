//using UnityEngine;

//public class MortarTurret : Turret {

//    // --------------------------------------------------------------
//    //   Inspector Fields
//    // -------------------------------------------------------------- 

//    [Header("Setup")]
//    [SerializeField] private bool calculateOrientation;
//    [SerializeField] private Transform yawTransform; // The base that rotates left/right (Y axis)
//    [SerializeField] private Transform pitchTransform; // The barrel/pivot that rotates up/down (local X axis)
//    [SerializeField] private Transform barrelTransform;
//    [SerializeField] private GameObject bulletPrefab = null;
//    [SerializeField] private Transform firePoint;

//    public Transform barrel; // The barrel that pitches up/down
//    public GameObject projectilePrefab; // Prefab of the projectile 
    
//    [Header("Attributes")]

//    [SerializeField] private float yawSpeed = 180f;      // degrees per second (horizontal)
//    [SerializeField] private float pitchSpeed = 180f;    // degrees per second (vertical)
//    [SerializeField] private float maxPitchUp = 60f;     // max elevation (positive = up)
//    [SerializeField] private float maxPitchDown = 30f;   // max depression (positive value = how far down)

//    [Header("Mortar")]
//    [SerializeField] private float flightTime = 1;   // max depression (positive value = how far down)
//    [SerializeField] private float maxHeight = 10;   // max depression (positive value = how far down)


//    // --------------------------------------------------------------
//    //   Private 
//    // --------------------------------------------------------------

//    private float nextFireTime = 0f;
//    private float gravity = 9.81f; // Gravity magnitude (matches Unity's default)

//    //private float countdown = 2f;  
//    private int waveIndex = 0;
//    private int bulletIndex = 0;
//    private int impactIndex = 0;
//    private float fireCountdown = 0;
//    private MortarBullet[] bulletArray;
//    private GameObject[] impactArray;
//    private WaitForSeconds waitBetweenUpdateTarget;
//    private Vector3 BarrelOffset;
//    [SerializeField] private bool targetInSight;
//    [SerializeField] private bool targetAlmostInSight;
//    [SerializeField] private float dotProductdotProduct;
//    // --------------------------------------------------------------
//    //   MonoBehaviour
//    // --------------------------------------------------------------


//    protected override void Init() {
//        base.Init();
//        if (bulletPrefab != null) bulletArray = CreateStockOf<MortarBullet>(bulletPrefab, 5);
//    }


//    void Update() {
//        if (target != null) {
//            // Calculate direction to target
//            Vector3 toTarget = target.position - barrel.position; // Use barrel position for accuracy
//            Vector3 horizontalToTarget = new Vector3(toTarget.x, 0, toTarget.z);
//            float x = horizontalToTarget.magnitude; // Horizontal distance
//            float y = toTarget.y; // Vertical difference

//            // Yaw: Rotate the base towards the target horizontally
//            if (horizontalToTarget != Vector3.zero) {
//                Quaternion yawRotation = Quaternion.LookRotation(horizontalToTarget);
//                yawTransform.transform.rotation = Quaternion.Slerp(yawTransform.transform.rotation, yawRotation, Time.deltaTime * 5f); // Smooth rotation
//            }

//            // Pitch: Calculate the launch angle for parabolic trajectory
//            float angle = CalculateLaunchAngle(x, y, ProjectileSpeed.Value);

//            if (!float.IsNaN(angle)) {
//                // Apply pitch to barrel (negative for upward pitch in Unity convention)
//                Quaternion pitchRotation = Quaternion.Euler(-angle, 0, 0);
//                barrel.localRotation = Quaternion.Slerp(barrel.localRotation, pitchRotation, Time.deltaTime * 5f); // Smooth rotation

//                bool readyToFire = CheckIfReadyToFire();

//                if (readyToFire) {

//                    Shoot();
//                }
//            }
//        }
//    }

//    /// <summary>
//    /// Calculates the launch angle (in degrees) to hit the target with a parabolic projectile.
//    /// Returns the high-arc angle if possible.
//    /// </summary>
//    private float CalculateLaunchAngle(float x, float y, float v) {
//        float g = gravity;
//        float discriminant = v * v * v * v - g * (g * x * x + 2 * y * v * v);

//        if (discriminant < 0) {
//            return float.NaN; // No solution (out of range)
//        }

//        float sqrtDisc = Mathf.Sqrt(discriminant);

//        // Two possible tan(theta): high arc (+) and low arc (-)
//        float tanThetaHigh = (v * v + sqrtDisc) / (g * x);
//        float tanThetaLow = (v * v - sqrtDisc) / (g * x);

//        // Prefer high arc for mortar (larger angle)
//        float tanTheta = tanThetaHigh;

//        // If high arc is invalid or too steep, fallback to low
//        if (float.IsNaN(tanTheta) || tanTheta < 0) {
//            tanTheta = tanThetaLow;
//        }

//        return Mathf.Atan(tanTheta) * Mathf.Rad2Deg;
//    }

//    /// <summary>
//    /// Fires the projectile.
//    /// </summary>
//    private void Shoot() {
//        //Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

//        MortarBullet myBullet = GetObjectFromIndex<MortarBullet>(bulletArray, bulletIndex);
//        bulletIndex++;
//        bulletIndex %= bulletArray.Length; // Redundant ?

//        InstantiateAlternative(myBullet.gameObject, firePoint.position, firePoint.rotation, Vector3.one, null);
//        if (base.target != null) {
//            myBullet.Init(base.target, enemyTargetted, AttackDamage.Value, ProjectileSpeed.Value, flightTime, maxHeight);
//            myBullet.ActivateBulletAndDesactivateImpact();
//        }
//    }

//    private void OnDrawGizmosSelected() {
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(Range.Value)); // Mathf.Sqrt because sqrMagnitude
//    }
//}