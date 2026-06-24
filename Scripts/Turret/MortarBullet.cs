using UnityEngine;

public class MortarBullet : Projectile {

    //[Header("Mortar Settings")]
    private float flightTime = 1f;      // total duration of flight (tune this)
    private float maxHeight = 10f;        // peak height above highest of start/end

    //private Vector3 _lauchDirection;
    private Vector3 startPosition;
    private float timer;


    public void Init(Turret turret,Transform target, Enemy enemy, int damage, int speed, float flightTime , float maxHeight) {
        this.turret = turret;
        this.target = target;
        this.enemy = enemy;
        this.damage = damage;
        this.speed = speed;       

        this.flightTime = flightTime;
        this.maxHeight = maxHeight;

        startPosition = this.transform.position;
        timer = 0f;
    }

    private void Start() {
        impact = Instantiate(impactPrefab);
        impact.SetActive(false);
    }

    private void Update() {
        if (!bulletIsActive) return;

        if (target == null) {
            gameObject.SetActive(false);
            bulletIsActive = false;
            return;
        }

        timer += Time.deltaTime;
        float t = timer / flightTime;   // 0 ? 1

        if (t >= 1f) {
            HitTarget();
            return;
        }

        // 1) Horizontal movement
        Vector3 currentFlat = Vector3.Lerp(startPosition, target.position, t);

        // 2) Vertical parabola
        float height = maxHeight * 4f * t * (1f - t);

        // 3) NEW position
        Vector3 newPosition = currentFlat + Vector3.up * height;

        // === Fix: Calculate direction before applying the position ===
        Vector3 velocityDir = (newPosition - transform.position).normalized;

        transform.position = newPosition;

        // Optional: rotate to face movement direction
        if (velocityDir.sqrMagnitude > 0.01f) {
            transform.forward = velocityDir;
        }

        //// 1) Horizontal movement: lerp flat positions
        //Vector3 currentFlat = Vector3.Lerp(startPosition, target.position, t);

        //// 2) Vertical: simple upside-down parabola (peaks in middle)
        //// 4t(1-t) gives nice 0 -> 1 -> 0 curve
        //float height = maxHeight * 4f * t * (1f - t);

        //// 3) Final position
        //transform.position = currentFlat + Vector3.up * height;

        //// Optional: rotate to face motion direction
        //Vector3 velocityDir = (currentFlat + Vector3.up * height - transform.position).normalized;
        //if (velocityDir.sqrMagnitude > 0.01f) {
        //    transform.forward = velocityDir;
        //}

        
    }
    private void OnDestroy() {
        Destroy(impact);
    }
}