using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundManagerMusic
{
    None,
    Menu,
    Action,
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public float _transitionSpeed = 0.5f;

    public AudioSource _ambienceSound;

    public AudioSource _menuMusic;

    public AudioSource _actionMusic;

    private float _ambienceVolume;
    private float _menuMusicVolume;
    private float _actionMusicVolume;

    public SoundManagerMusic Music { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            GameObject.DestroyImmediate(this.gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        GameObject.DontDestroyOnLoad(this);

        _ambienceSound.Play();
        _menuMusic.Play();
        _actionMusic.Play();

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

        var menuVolume = this.Music == SoundManagerMusic.Menu ? 1f : 0f;
        _menuMusicVolume += (menuVolume - _menuMusicVolume) * Mathf.Min(0.5f, Time.deltaTime * _transitionSpeed);
        _menuMusic.volume = _menuMusicVolume;

        var actionVolume = this.Music == SoundManagerMusic.Action ? 1f : 0f;
        _actionMusicVolume += (actionVolume - _actionMusicVolume) * Mathf.Min(0.5f, Time.deltaTime * _transitionSpeed);
        _actionMusic.volume = _actionMusicVolume;

    }
}
