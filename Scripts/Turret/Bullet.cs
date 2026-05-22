using UnityEngine;

public class Bullet : MonoBehaviour {

    public GameObject _impactPrefab = null;

    protected bool _bulletIsActive = false;
    protected int _damage;
    protected int _speed ;
    protected Vector3 _direction = Vector3.zero;
    protected Transform _target; 
    protected GameObject _impact;
    protected Enemy _enemy; // a reference to the instance of Enemy so we dont use Getcomponent

    protected Turret _turret; // turret that created this bullet / used for events

    public Bullet(Turret turret) {
        _turret = turret;     
    }

    public void Init(Transform target, Enemy enemy, int damage, int speed) {
        this._target = target;
        this._enemy = enemy;
        this._damage = damage;
        this._speed = speed;
    }

    public void ActivateBulletAndDesactivateImpact() {
        _bulletIsActive = true;
        if (_impact != null) _impact.SetActive(false);
    }

    private void Awake() { // old Start()
        _impact = Instantiate(_impactPrefab);
        _impact.SetActive(false);
    }

    void Update() {
        if (_bulletIsActive) {
            if (_target == null) {

                this.gameObject.SetActive(false);
                _bulletIsActive = false;

            } else {

                _direction = _target.position - this.transform.position;
                float distanceThisFrame = _speed * Time.deltaTime;

                if (_direction.sqrMagnitude <= distanceThisFrame * distanceThisFrame) {
                    HitTarget();
                    return;
                }
                this.transform.Translate(_direction.normalized * distanceThisFrame, Space.World);
            }
        }
    }

    public void HitTarget() {

        GameEvents.EnemyHit(_turret, _enemy); // new

        if(_enemy!=null) _enemy.TakeDamage(_turret, _damage); // passing _turret for OnKill event 
        _target = null;
        this.gameObject.SetActive(false);
        _bulletIsActive = false;

        _impact.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
        _impact.SetActive(true);
    }

}
