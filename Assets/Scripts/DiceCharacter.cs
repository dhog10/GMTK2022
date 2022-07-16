using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCharacter : MonoBehaviour
{
    [SerializeField]
    protected Vector2 _bobAmount = new Vector2(0.1f, 0.15f);

    [SerializeField]
    protected Vector2 _bobSpeed = new Vector2(1f, 30f);

    [SerializeField]
    protected Vector2 _rotationBobAmount = new Vector2(5f, 14f);

    [SerializeField]
    protected Vector2 _rotationBobSpeed = new Vector2(1.3f, 20f);

    [SerializeField]
    protected float _velocityRotate = 0.5f;

    [SerializeField]
    protected GameObject _visualObject;

    [SerializeField]
    protected GameObject _visualCharacter;

    [SerializeField]
    private GameObject[] _footstepPrefabs;

    private Vector3 _characterLocalpos;
    private Vector3 _prevPos;
    private float _bobTime;
    private float _rotationBobTime;
    private float _lastFootstep;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        var rnd = Random.Range(0f, 20f);
        _bobTime += rnd;
        _rotationBobTime += rnd;

        _prevPos = transform.position;
        _characterLocalpos = _visualCharacter.transform.localPosition;
    }

    public virtual float SpeedP
        => Mathf.Clamp(new Vector3(this.CurrentVelocity.x, 0f, this.CurrentVelocity.z).magnitude / this.MaxSpeed, 0f, 1f);

    public virtual float MaxSpeed
        => 0.0000001f;

    public Vector3 CurrentVelocity { get; private set; }

    protected virtual void Update()
    {
        var velocity = this.CurrentVelocity;
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
            rotMovePos -= velocity.normalized * speedP;

            _visualCharacter.transform.rotation = Quaternion.Lerp(_visualCharacter.transform.rotation, Quaternion.FromToRotation(Vector3.up, (rotMovePos - transform.position).normalized), _velocityRotate);
        }

        var footstepInterval = 60f / bobSpeed / 10;

        if (speedP <= 0.01f)
        {
            _lastFootstep = Time.time - footstepInterval * 0.75f;
        }

        if (Time.time - _lastFootstep >= footstepInterval)
        {
            this.Footstep();
        }
    }

    private void Footstep()
    {
        _lastFootstep = Time.time;

        if (_footstepPrefabs == null || _footstepPrefabs.Length == 0)
        {
            return;
        }

        var prefab = _footstepPrefabs[Random.Range(0, _footstepPrefabs.Length)];
        var footstep = GameObject.Instantiate(prefab, this.transform.position, Quaternion.identity);
        var snds = footstep.GetComponents<AudioSource>();
        if (snds == null || snds.Length == 0)
        {
            Debug.LogError("Footstep has no audio sources");
            return;
        }

        var snd = snds[Random.Range(0, snds.Length)];

        snd.Play();
        GameObject.Destroy(footstep, snd.clip.length);
    }

    protected virtual void FixedUpdate()
    {
        var velocity = (transform.position - _prevPos) / Time.fixedDeltaTime;
        this.CurrentVelocity += (velocity - this.CurrentVelocity) * Mathf.Min(0.5f, Time.fixedDeltaTime * 25f);
        _prevPos = transform.position;
    }
}
