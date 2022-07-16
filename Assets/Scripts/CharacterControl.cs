using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
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
    private Vector2 _bobAmount = new Vector2(0.1f, 0.15f);

    [SerializeField]
    private Vector2 _bobSpeed = new Vector2(1f, 30f);

    [SerializeField]
    private Vector2 _rotationBobAmount = new Vector2(5f, 14f);

    [SerializeField]
    private Vector2 _rotationBobSpeed = new Vector2(1.3f, 20f);

    [SerializeField]
    private float _velocityRotate = 0.5f;

    [SerializeField]
    private GameObject _visualObject;

    [SerializeField]
    private GameObject _visualCharacter;

    [SerializeField]
    private GameObject _gunsObject;

    private float _currentCameraYaw;
    private Rigidbody _rb;
    private Vector3 _currentVelocity;
    private Vector3 _characterLocalpos;
    private float _bobTime;
    private float _rotationBobTime;

    // Start is called before the first frame update
    void Start()
    {
        var rnd = Random.Range(0f, 20f);
        _bobTime += rnd;
        _rotationBobTime += rnd;

        _characterLocalpos = _visualCharacter.transform.localPosition;

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

    public float SpeedP
        => Mathf.Clamp(_currentVelocity.magnitude / _speed, 0f, 1f);

    public float GunOrbitDistance
        => Mathf.Lerp(_gunsOrbitDistance.x, _gunsOrbitDistance.y, this.SpeedP);

    public float GunOrbitHeight
        => Mathf.Lerp(_gunsOrbitHeight.x, _gunsOrbitHeight.y, this.SpeedP);

    public Camera Camera
        => Camera.main;

    public Vector3 AimDirection { get; set; } = Vector3.forward;

    // Update is called once per frame
    private void Update()
    {
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

        var speedP = this.SpeedP;

        var bobSpeed = Mathf.Lerp(_bobSpeed.x, _bobSpeed.y, speedP);
        var bobAmount = Mathf.Lerp(_bobAmount.x, _bobAmount.y, speedP);
        var rotationBobSpeed = Mathf.Lerp(_rotationBobSpeed.x, _rotationBobSpeed.y, speedP);
        var rotationBobAmount = Mathf.Lerp(_rotationBobAmount.x, _rotationBobAmount.y, speedP);

        _bobTime += Time.deltaTime * bobSpeed;
        _rotationBobTime += Time.deltaTime * rotationBobSpeed;

        var characterRotation = new Vector3(Mathf.Cos(_rotationBobTime), 0f, -Mathf.Cos(_rotationBobTime * 0.5f)) * rotationBobAmount;

        _visualCharacter.transform.rotation = Quaternion.Euler(characterRotation);
        _visualCharacter.transform.localPosition = _characterLocalpos + new Vector3(0f, Mathf.Cos(_bobTime), 0f) * bobAmount;

        if (speedP > 0f)
        {
            var rotMovePos = transform.position + new Vector3(0f, 1f, 0f);
            rotMovePos -= _currentVelocity.normalized * speedP;

            _visualCharacter.transform.rotation = Quaternion.Lerp(_visualCharacter.transform.rotation, Quaternion.FromToRotation(Vector3.up, (rotMovePos - transform.position).normalized), _velocityRotate);
        }

        this.UpdateGuns();
    }

    private void UpdateGuns()
    {
        var mouseAimDirection = this.AimDirection;
        var mousePos = Input.mousePosition;
        var mouseRay = this.Camera.ScreenPointToRay(mousePos);
        var mouseHitpoint = Vector3.zero;

        if (Physics.Raycast(mouseRay, out var hit))
        {
            mouseHitpoint = hit.point;
            mouseAimDirection = (hit.point - transform.position);
            mouseAimDirection.y = 0f;
            mouseAimDirection.Normalize();

            mouseHitpoint.y = Mathf.Max(mouseHitpoint.y, _visualCharacter.transform.position.y);
        }

        this.AimDirection = mouseAimDirection;

        foreach (var gun in _guns)
        {
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
    }

    private void FixedUpdate()
    {
        var vel = _rb.velocity;
        vel.x = _currentVelocity.x;
        vel.z = _currentVelocity.z;

        _rb.velocity = vel;
    }
}
