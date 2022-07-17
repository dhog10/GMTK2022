using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceGun : MonoBehaviour
{
    [Header("Setup")]

    [SerializeField]
    private float _rpm;

    [SerializeField]
    private GameObject _projectilePrefab;

    [SerializeField]
    private float _orbitDistanceAdd;

    [SerializeField]
    private GameObject _visualObject;

    [Header("Recoil")]

    [SerializeField]
    private float _recoilDistance = 1f;

    [SerializeField]
    private float _recoilDecreaseSpeed = 12f;

    [SerializeField]
    private float _recoilSnapSpeed = 12f;

    [SerializeField]
    private float _recoilScale = 1.3f;

    [SerializeField]
    private float _recoilScaleDecreaseSpeed = 25f;

    [SerializeField]
    private float _recoilScaleSnapSpeed = 30f;

    [SerializeField]
    private bool _setDamage = true;

    [SerializeField]
    private float _damage;

    [SerializeField]
    private Team _team;

    private float _lastShoot;
    private float _recoilAmt;
    private float _recoilScaleAmt;
    private Vector3 _recoilDir;
    private Vector3 _currentRecoilPos;
    private float _currentRecoilScale = 1f;
    private Vector3 _visualObjectOriginalScale;

    // Start is called before the first frame update
    private void Start()
    {
        this.OrbitDistanceAdd = _orbitDistanceAdd;
        _visualObjectOriginalScale = _visualObject.transform.localScale;

        this.Damage = _damage;
        this.OriginalDamage = _damage;
        this.RPM = _rpm;
        this.OriginalRPM = _rpm;
    }

    public float OrbitDistanceAdd { get; set; }

    public Vector3 AimDirection { get; set; } = Vector3.one;

    public float Damage { get; set; }

    public float OriginalDamage { get; private set; }

    public float RPM { get; set; }

    public float OriginalRPM { get; private set; }

    public virtual bool AutoFire
        => false;

    private void Update()
    {
        var fire = this.FireInputPressed();
        if (fire && Time.time - _lastShoot >= 60f / _rpm)
        {
            _lastShoot = Time.time;
            this.Shoot();
        }

        if (_recoilAmt > 0f)
        {
            var recoilPos = _recoilDir * _recoilAmt * _recoilDistance;
            _currentRecoilPos += (recoilPos - _currentRecoilPos) * Mathf.Min(0.5f, Time.deltaTime * _recoilSnapSpeed);
            _recoilAmt -= _recoilAmt * Mathf.Min(0.5f, Time.deltaTime * _recoilDecreaseSpeed);

            _visualObject.transform.position = transform.position + _currentRecoilPos;
        }

        if (_recoilScaleAmt > 0f)
        {
            var recoilScale = Mathf.Lerp(1f, _recoilScale, _recoilAmt);
            _currentRecoilScale += (recoilScale - _currentRecoilScale) * Mathf.Min(0.5f, Time.deltaTime * _recoilScaleSnapSpeed);
            _recoilScaleAmt -= _recoilScaleAmt * Mathf.Min(0.5f, Time.deltaTime * _recoilScaleDecreaseSpeed);

            _visualObject.transform.localScale = _visualObjectOriginalScale * _currentRecoilScale;
        }
    }

    public virtual void Shoot()
    {
        _lastShoot = Time.time;

        _recoilDir = -this.AimDirection.normalized;
        _recoilAmt = 1f;
        _recoilScaleAmt = 1f;

        var projectileObject = GameObject.Instantiate(_projectilePrefab, this.transform.position, Quaternion.LookRotation(this.AimDirection.normalized));
        var projectile = projectileObject.GetComponent<Projectile>();
        projectile.Team = _team;
        if (_setDamage)
        {
            projectile.Damage = this.Damage;
        }

        projectile.Shoot(this.AimDirection);
    }

    protected virtual bool FireInputPressed()
    {
        return Input.GetAxis("Fire1") > 0f;
    }
}
