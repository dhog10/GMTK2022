using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
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
    private GameObject _visualObject;

    private float _currentCameraYaw;

    private Rigidbody _rb;
    private Vector3 _currentVelocity;

    // Start is called before the first frame update
    void Start()
    {
        _rb = this.GetComponent<Rigidbody>();

        this.CameraPitch = _cameraStartPitch;
        this.CameraYaw = _cameraStartYaw;
        _currentCameraYaw = _cameraStartYaw;
    }

    public float CameraPitch { get; set; }

    public float CameraYaw { get; set; }

    // Update is called once per frame
    void Update()
    {
        var mouseX = Input.GetAxis("Mouse X");
        if (mouseX != 0f)
        {
            _currentCameraYaw += mouseX * _cameraRotationSpeed;
        }

        this.CameraYaw += (_currentCameraYaw - this.CameraYaw) * Time.deltaTime * _cameraSmoothSpeed;

        var cam = Camera.main;
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
    }

    void FixedUpdate()
    {
        var vel = _rb.velocity;
        vel.x = _currentVelocity.x;
        vel.z = _currentVelocity.z;

        _rb.velocity = vel;
    }
}
