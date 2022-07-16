using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public float _transitionSpeed = 0.5f;

    public AudioSource _ambienceSound;

    private float _ambienceVolume;

    private void Start()
    {
        _ambienceSound.Play();

        this.UpdateSounds();
    }

    private void Update()
    {
        this.UpdateSounds();
    }

    private void UpdateSounds()
    {
        var targetAmbienceVolume = 1f;
        _ambienceVolume += (targetAmbienceVolume - _transitionSpeed) * Mathf.Min(0.5f, Time.deltaTime * _transitionSpeed);

        _ambienceSound.volume = _ambienceVolume;
    }
}
