using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    /// <summary>
    /// This nested class holds all data related to the playback of a single AudioClip. Instances of this class are exposed to the Inspector through the SoundManager class.
    /// </summary>
    [Serializable]
    public class SoundEntity
    {
#if UNITY_EDITOR
        /// <summary>
        /// These members are part of the compilation only in the Unity Editor; i.e. they won't compile into your game.
        /// The string replaces the default 'Element' string displayed as the name of the entities inside the array.
        /// But, at the same time the field itself is hidden from editing.
        /// </summary>
        [HideInInspector]
        public string Name;
        public void SetName()
            => Name = soundType.ToString();
#endif
        public ChannelType channelType;
        public GameSound soundType;
        public AudioClip audioClip;
    }


    [SerializeField]
    SoundEntity[] _soundList = default;
    
    Dictionary<GameSound, SoundEntity> _soundMap = new Dictionary<GameSound, SoundEntity>();

    [SerializeField]
    AudioSource bgmSource,
                sfxSource;

    const string BGM_VOLUME_KEY = "bgmVolume";
    public float BGMVolume
    {
        get { return bgmSource.volume; }
        set {
            bgmSource.volume = value;
            PlayerPrefs.SetFloat(BGM_VOLUME_KEY, value);
        }
    }

    const string SFX_VOLUME_KEY = "sfxVolume";
    public float SFXVolume
    {
        get { return sfxSource.volume; }
        set
        {
            sfxSource.volume = value;
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);
        }
    }

    bool _initialized = false;


#if UNITY_EDITOR
    void OnValidate()
    {
        // We're only interested in changes made in the Inspector
        if (!GUI.changed)
            return;

        // Set / update the names of entities to replace the generic 'element' name in Unity Inspector's array view
        foreach (var s in _soundList)
            s.SetName();

        // If changes were made in the Inspector while the game is running, and we're past initialization,
        // process again the list of sounds to make sure our runtime representation is up to date.
        if (EditorApplication.isPlaying && _initialized)
            PopulateSoundMap();
    }
#endif


    /// <summary>
    /// Converts the Editor-compatible array into a fast-lookup dictionary map.
    /// Creates a list for each sound type, to support multiple sounds of the same type.
    /// </summary>
    void PopulateSoundMap()
    {
        foreach (var s in _soundList)
        {
            // Silently skip entries where 'None' is selected as soundtype
            if (s.soundType == GameSound.None)
                continue;

            else
            {
                _soundMap.Add(s.soundType, s);
                //Debug.Log(s.soundType);
            }
        }
    }


    protected override void Awake()
    {
        base.Awake();

        if (_initialized == true)
            return;

        PopulateSoundMap();
        _initialized = true;

        bgmSource.loop = true;


        if (PlayerPrefs.HasKey(BGM_VOLUME_KEY))
        {
            bgmSource.volume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY);
        }
        if (PlayerPrefs.HasKey(SFX_VOLUME_KEY))
        {
            sfxSource.volume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY);
        }
    }


    void Start()
    {
        Play(GameSound.MenuBGM);
    }


    public void Play(int s)
    {
        Play((GameSound)s);
    }
    public void Play(GameSound sound)
    {
        if (_soundMap.ContainsKey(sound))
        {
            ChannelType type = _soundMap[sound].channelType;
            if (type == ChannelType.BGM)
            {
                PlayBGMSound(_soundMap[sound].audioClip);
            }
            else if (type == ChannelType.UI)
            {
                PlayUISound(_soundMap[sound].audioClip);
            }
        }
    }

    public void PlayBGMSound(AudioClip clip)
    {
        StopBGM();
        if (clip != bgmSource.clip)
            bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlayUISound(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    public void StopBGM()
    {
        bgmSource.Stop();
    }
}


