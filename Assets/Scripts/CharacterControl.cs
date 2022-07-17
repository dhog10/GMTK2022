using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : DiceCharacter
{
    [SerializeField]
    private List<DiceGun> _guns = new List<DiceGun>();

    [SerializeField]
    private Vector2 _gunsOrbitDistance = new Vector2(1f, 1.2f);

    [SerializeField]
    private Vector2 _gunsOrbitHeight = new Vector2(0.8f, 1f);

    [SerializeField]
    private float _speed = 10f;

    [SerializeField]
    private float _speedSmooth = 10f;

    [SerializeField]
    private float _cameraStartPitch = 45f;

    [SerializeField]
    private float _cameraStartYaw = -45f;

    [SerializeField]
    private float _cameraRotationSpeed = 0f;

    [SerializeField]
    private float _cameraSmoothSpeed = 8f;

    [SerializeField]
    private float _cameraDistance;

    [SerializeField]
    private GameObject _gunsObject;

    private float _currentCameraYaw;
    private Rigidbody _rb;
    private Vector3 _currentVelocity;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        _rb = this.GetComponent<Rigidbody>();

        this.CameraPitch = _cameraStartPitch;
        this.CameraYaw = _cameraStartYaw;
        _currentCameraYaw = _cameraStartYaw;

        for (var i = 0; i < _gunsObject.transform.childCount; i++)
        {
            var child = _gunsObject.transform.GetChild(i);
            var gun = child.GetComponent<DiceGun>();

            if (gun != null && !_guns.Contains(gun))
            {
                _guns.Add(gun);
            }
        }
    }

    public float CameraPitch { get; set; }

    public float CameraYaw { get; set; }

    public override float MaxSpeed
        => _speed;

    public float GunOrbitDistance
        => Mathf.Lerp(_gunsOrbitDistance.x, _gunsOrbitDistance.y, this.SpeedP);

    public float GunOrbitHeight
        => Mathf.Lerp(_gunsOrbitHeight.x, _gunsOrbitHeight.y, this.SpeedP);

    public Camera Camera
        => Camera.main;

    public Vector3 AimDirection { get; set; } = Vector3.forward;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        var mouseX = Input.GetAxis("Mouse X");
        if (mouseX != 0f)
        {
            _currentCameraYaw += mouseX * _cameraRotationSpeed;
        }

        this.CameraYaw += (_currentCameraYaw - this.CameraYaw) * Time.deltaTime * _cameraSmoothSpeed;

        var cam = this.Camera;
        if (cam == null)
        {
            return;
        }

        var rotation = Quaternion.Euler(this.CameraPitch, this.CameraYaw, 0f);
        cam.transform.rotation = rotation;
        cam.transform.position = transform.position - (rotation * Vector3.forward) * _cameraDistance;

        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        var right = cam.transform.right;
        var up = cam.transform.up;
        right.y = 0f;
        up.y = 0f;
        right.Normalize();
        up.Normalize();

        var targetVelocity = (Vector3.zero + right * horizontal + up * vertical) * _speed;
        _currentVelocity += (targetVelocity - _currentVelocity) * Time.deltaTime * _speedSmooth;

        this.UpdateGuns();
    }

    private void UpdateGuns()
    {
        var mouseAimDirection = this.AimDirection;
        var mousePos = Input.mousePosition;
        var mouseRay = this.Camera.ScreenPointToRay(mousePos);
        var mouseHitpoint = Vector3.zero;

        foreach (var gun in _guns)
        {
            if (new Plane(Vector3.up, _visualObject.transform.position).Raycast(mouseRay, out var enter))
            {
                mouseHitpoint = mouseRay.GetPoint(enter);
                mouseAimDirection = (mouseHitpoint - gun.transform.position).normalized;
            }

            var orbitOrigin = _visualObject.transform.position + Vector3.up * this.GunOrbitHeight;
            var orbitDistance = this.GunOrbitDistance + gun.OrbitDistanceAdd;
            var aimDirection = mouseAimDirection;

            var toGun = (gun.transform.position - orbitOrigin).normalized;
            var rotation = Quaternion.LookRotation(toGun, Vector3.up);
            var targetRotation = Quaternion.LookRotation(aimDirection, Vector3.up);
            rotation = Quaternion.Lerp(rotation, targetRotation, Time.deltaTime * 5f);

            var fwd = (rotation * Vector3.forward);
            gun.transform.position = orbitOrigin + fwd * orbitDistance;

            gun.AimDirection = (mouseHitpoint - gun.transform.position).normalized;
        }

        this.AimDirection = mouseAimDirection;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        var vel = _rb.velocity;
        vel.x = _currentVelocity.x;
        vel.z = _currentVelocity.z;

        _rb.velocity = vel;
    }
}
