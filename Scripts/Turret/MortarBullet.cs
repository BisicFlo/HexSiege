using UnityEngine;

public class MortarBullet : Bullet {

    //[Header("Mortar Settings")]
    private float _flightTime = 1f;      // total duration of flight (tune this)
    private float _maxHeight = 10f;        // peak height above highest of start/end

    //private Vector3 _lauchDirection;
    private Vector3 _startPosition;
    private float _timer;

    public void Init(Transform target, Enemy enemy, int damage, int speed, float flightTime , float maxHeight) {
        this._target = target;
        this._enemy = enemy;
        this._damage = damage;
        this._speed = speed;       

        this._flightTime = flightTime;
        this._maxHeight = maxHeight;

        _startPosition = this.transform.position;
        _timer = 0f;

    }

    private void Start() {
        _impact = Instantiate(_impactPrefab);
        _impact.SetActive(false);
    }


    private void Update() {
        if (!_bulletIsActive) return;

        if (_target == null) {
            gameObject.SetActive(false);
            _bulletIsActive = false;
            return;
        }

        _timer += Time.deltaTime;
        float t = _timer / _flightTime;   // 0 ? 1

        if (t >= 1f) {
            HitTarget();
            return;
        }

        // Horizontal movement: lerp flat positions
        Vector3 currentFlat = Vector3.Lerp(_startPosition, _target.position, t);

        // Vertical: simple upside-down parabola (peaks in middle)
        // 4t(1-t) gives nice 0 -> 1 -> 0 curve
        float height = _maxHeight * 4f * t * (1f - t);

        // Final position
        transform.position = currentFlat + Vector3.up * height;

        // Optional: rotate to face motion direction
        Vector3 velocityDir = (currentFlat + Vector3.up * height - transform.position).normalized;
        if (velocityDir.sqrMagnitude > 0.01f) {
            transform.forward = velocityDir;
        }
    }


}