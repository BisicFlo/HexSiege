using UnityEngine;

public class Projectile : MonoBehaviour {

    public GameObject impactPrefab = null;

    protected bool bulletIsActive = false;
    protected int damage;
    protected int speed ;

    protected bool isCritical; //new
    protected bool isCursed;  //new

    protected Vector3 direction = Vector3.zero;
    protected Transform target; 
    protected GameObject impact;
    protected Enemy enemy;          // a reference to the instance of Enemy so we dont use Getcomponent
    protected Turret turret;       // turret that created this bullet / used for events

    private void Awake() { // old Start()
        impact = Instantiate(impactPrefab);
        impact.SetActive(false);
    }

    public void Init(Turret turret, Transform target, Enemy enemy, int damage, int speed, bool isCritical , bool isCursed) {
        this.turret = turret;
        this.target = target;
        this.enemy = enemy;
        this.damage = damage;
        this.speed = speed;
        this.isCritical = isCritical;
        this.isCursed = isCursed;
    }

    public void ActivateBulletAndDesactivateImpact() {
        bulletIsActive = true;
        if (impact != null) impact.SetActive(false);
    }

    void Update() {             //Make bullet continue to last enemy position 
        if (bulletIsActive) {
            if (target == null) {

                this.gameObject.SetActive(false);
                bulletIsActive = false;

            } else {

                direction = target.position - this.transform.position;
                float distanceThisFrame = speed * Time.deltaTime;

                if (direction.sqrMagnitude <= distanceThisFrame * distanceThisFrame) {
                    HitTarget();
                    return;
                }
                this.transform.Translate(direction.normalized * distanceThisFrame, Space.World);
            }
        }
    }

    public void HitTarget() {      

        if (enemy != null) {
            GameEvents.EnemyHit(turret, enemy); // new
            enemy.TakeDamage(turret, damage); // passing _turret for OnKill event 
            if (isCritical) { 
            
            }
            if (isCursed) {
                enemy.TakeCurse(turret);
            }
        }
        target = null;
        this.gameObject.SetActive(false);
        bulletIsActive = false;

        impact.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
        impact.SetActive(true);
    }
}
