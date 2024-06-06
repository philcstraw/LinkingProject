using UnityEngine;

public class AudioControl : MonoSingleton<AudioControl>
{
    public JukeBox jukeBox;
    public AudioSource buttonAudioSource;
    public AudioSource gameAudioSource;
    public AudioSource musicAudioSource;
    public bool playMusicOnStart = true;
    public bool muted = false;

    internal int globalVolume = 100;
    internal int musicVolume = 100;
    internal int gameVolume = 100;

    int m_MaxVolume = 100;
    float m_volumeScale = 0.01f;
    string m_globalVolumeSettingString = "global volume";
    string m_musicVolumeSettingString = "music volume";
    string m_gameVolumeSettingString = "game volume";
    string m_lastTrackSettingString = "last track";
    string m_muteSettingString = "mute";

    public override void Awake()
    {
        MakePersistant(true);

        base.Awake();
    }

    void Start()
    {
        LoadLastTrack();

        if (playMusicOnStart)
            PlayMusic();

        LoadSavedSettngs();
    }

    internal string CurrentTrackName()
    {
        return jukeBox.tracks[jukeBox.selectedTrack].trackName;
    }

    internal int CurrentTrackIndex()
    {
        return jukeBox.selectedTrack;
    }

    internal int MusicTrackCount()
    {
        return jukeBox.tracks.Count;
    }

    internal void NextMusicTrack()
    {
        jukeBox.selectedTrack += 1;

        if (jukeBox.selectedTrack >= jukeBox.tracks.Count)
            jukeBox.selectedTrack = 0;

        PlayMusic();
    }

    internal void PreviousMusicTrack()
    {
        jukeBox.selectedTrack -= 1;

        if (jukeBox.selectedTrack < 0)
            jukeBox.selectedTrack = jukeBox.tracks.Count - 1;

        PlayMusic();
    }

    internal void PlayMusic()
    {
        if (jukeBox.ShouldRandomise())
            jukeBox.RandomiseTrack();

        var _track = jukeBox.CurrentTrack();
        musicAudioSource.clip = _track.clip;
        musicAudioSource.volume = _track.volume;

        Utility.SetPlayerSetting(m_lastTrackSettingString, jukeBox.selectedTrack);
        musicAudioSource.Play();
    }

    internal void LoadLastTrack()
    {
        int _lastTrack = Utility.GetPlayerSettingInt(m_lastTrackSettingString, 0);

        jukeBox.selectedTrack = _lastTrack;

        if (jukeBox.ShouldRandomise())
            jukeBox.RandomiseTrack();
    }

    internal void LoadSavedSettngs()
    {
        int vl = Utility.GetPlayerSettingInt(m_globalVolumeSettingString, 100);
        globalVolume = vl;
        SetGlobalVolumeLevels(vl);
        
        int mvl = Utility.GetPlayerSettingInt(m_musicVolumeSettingString, 100);
        musicVolume = mvl;
        SetMusicVolumeLevel(mvl);

        int gvl = Utility.GetPlayerSettingInt(m_gameVolumeSettingString, 100);
        gameVolume = gvl;
        SetGameVolumeLevel(gvl);

        muted = Utility.GetPlayerSettingBool(m_muteSettingString, false);

        if(muted)
            MuteAllAudio(muted);
    }
    
    internal void ToggleMute()
    {
        muted = !muted;
        MuteAllAudio(muted);
    }

    internal void IncreaseGlobalVolume()
    {
        globalVolume += 1;

        if (globalVolume > m_MaxVolume)
            globalVolume = m_MaxVolume;

        SetGlobalVolumeLevels(globalVolume);
    }
    internal void DecreaseGlobalVolume()
    {
        globalVolume -= 1;

        if (globalVolume < 0)
            globalVolume = 0;

        SetGlobalVolumeLevels(globalVolume);
    }

    internal void IncreaseMusicVolume()
    {
        musicVolume += 1;

        if (musicVolume > m_MaxVolume)
            musicVolume = m_MaxVolume;

        SetMusicVolumeLevel(musicVolume);
    }
    internal void DecreaseMusicVolume()
    {
        musicVolume -= 1;

        if (musicVolume < 0)
            musicVolume = 0;

        SetMusicVolumeLevel(musicVolume);
    }

    internal void IncreaseGameVolume()
    {
        gameVolume += 1;

        if (gameVolume > m_MaxVolume)
            gameVolume = m_MaxVolume;

        SetGameVolumeLevel(gameVolume);
    }

    internal void DecreaseGameVolume()
    {
        gameVolume -= 1;

        if (gameVolume < 0)
            gameVolume = 0;

        SetGameVolumeLevel(gameVolume);
    }

    void SetGlobalVolumeLevels(int volumeLevel)
    {
        AudioListener.volume = volumeLevel * m_volumeScale;

        Utility.SetPlayerSetting(m_globalVolumeSettingString,volumeLevel);
    }

    void SetMusicVolumeLevel(int volumeLevel)
    {
        musicAudioSource.volume = volumeLevel * m_volumeScale * jukeBox.CurrentTrack().volume;

        Utility.SetPlayerSetting(m_musicVolumeSettingString, volumeLevel);
    }

    void SetGameVolumeLevel(int volumeLevel)
    {
        AudioSource[] _sources = FindObjectsOfType<AudioSource>();

        foreach(AudioSource _source in _sources)
        {
            if (_source == musicAudioSource)
                continue;

            _source.volume = volumeLevel * m_volumeScale;
        }
        Utility.SetPlayerSetting(m_gameVolumeSettingString, volumeLevel);
    }

    void MuteAllAudio(bool mute)
    {
        AudioSource[] _sources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource _source in _sources)
            _source.mute = mute;

        Utility.SetPlayerSetting(m_muteSettingString, mute);
    }
}
