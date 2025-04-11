using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _walkingSource;
    [SerializeField] private AudioSource _sfxSource;

    public bool IsMusicEnabled = true;
    public bool IsSfxEnabled = true;

    private const string MUSIC_ENABLED_KEY = "MusicEnabled";
    private const string SFX_ENABLED_KEY = "SfxEnabled";

    private Dictionary<string, AudioClip> soundEffects = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        InitializeSoundManager();
    }

    private void InitializeSoundManager()
    {
        LoadSettings();
        ApplySettings();
    }

    private void LoadSettings()
    {
        IsMusicEnabled = PlayerPrefs.GetInt(MUSIC_ENABLED_KEY, 1) == 1;
        IsSfxEnabled = PlayerPrefs.GetInt(SFX_ENABLED_KEY, 1) == 1;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt(MUSIC_ENABLED_KEY, IsMusicEnabled ? 1 : 0);
        PlayerPrefs.SetInt(SFX_ENABLED_KEY, IsSfxEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplySettings()
    {
        _musicSource.mute = !IsMusicEnabled;
        _sfxSource.mute = !IsSfxEnabled;
        _walkingSource.mute = !IsSfxEnabled;
    }

    public void PlayMusic(AudioClip musicClip)
    {
        _musicSource.clip = musicClip;
        _musicSource.Play();
    }

    public void LoadSoundEffect(string name, AudioClip clip)
    {
        soundEffects.TryAdd(name, clip);
    }

    public void PlaySoundEffect(string name)
    {
        if (soundEffects.TryGetValue(name, out var effect))
        {
            _sfxSource.PlayOneShot(effect);
        }
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }
    
    public void PlayStepEffect()
    {
        _walkingSource.Play();
    }

    public void ToggleMusic()
    {
        IsMusicEnabled = !IsMusicEnabled;
        _musicSource.mute = !IsMusicEnabled;
        SaveSettings();
    }

    public void ToggleSfx()
    {
        IsSfxEnabled = !IsSfxEnabled;
        _sfxSource.mute = !IsSfxEnabled;
        _walkingSource.mute = !IsSfxEnabled;
        SaveSettings();
    }
}