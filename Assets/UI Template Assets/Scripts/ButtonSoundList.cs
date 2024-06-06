using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundList : MonoBehaviour 
{
    [Range(0.0f, 1.0f)]
    public float oneShotVolume = 1.0f;
    public AudioClip sound;
    public List<Button> buttons = new List<Button>();
    public AudioSource ExternalAudioSource;

    static AudioSource _uiAudioSource; 
    static bool init = false;

    void Start () 
    {
        SetUp();
	}

    AudioSource SetupUIAudioSource()
    {
        if (init)
            return _uiAudioSource;

        GameObject go = new GameObject();
        _uiAudioSource = go.AddComponent<AudioSource>();

        DontDestroyOnLoad(go);

        string uiTag = "_uiaudio";
        go.name = uiTag;
        init = true;
        _uiAudioSource.playOnAwake = false;

        return _uiAudioSource;
    }

    void SetUp()
    {
        if (ExternalAudioSource == null)
            ExternalAudioSource = SetupUIAudioSource();

        foreach (Button button in buttons)
        {
            if (button == null)
                continue;

            button.onClick.AddListener(() => PlaySound());
        }
    }

    public void PlaySound()
    {
        _uiAudioSource.PlayOneShot(sound, oneShotVolume);
    }
}
