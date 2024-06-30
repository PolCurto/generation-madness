using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	[HideInInspector] public static AudioManager Instance;

	[SerializeField] private AudioMixerGroup _mixerGroup;

    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    public Sound[] _musicThemes;
    public Sound[] _sfxSounds;

	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	public bool IsPlayingMusic()
    {
		return _musicSource.isPlaying;
    }

	public void StopMusic()
    {
		_musicSource.Stop();
	}

	public void PlayMusic(string theme)
	{
        Sound s = Array.Find(_musicThemes, item => item.name == theme);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
		_musicSource.clip = s.clip;
        _musicSource.Play();
    }

	public void PlaySFX(string sound, float volume)
	{
		Sound s = Array.Find(_sfxSounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}
		_sfxSource.PlayOneShot(s.clip, volume);
	}

	public void SetMusicVolume(float volume)
	{
		_musicSource.volume = volume;
	}
}