using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float oneShotVolume = 1.0f;
    public AudioClip sound;
    public Button button;
    public AudioSource ExternalAudioSource;
    public bool menuButton = true;

    void Start ()
    {
        SetUp();
	}

    void GetAudioControlSource()
    {
        AudioControl ac = FindObjectOfType<AudioControl>();

        ExternalAudioSource = ac.buttonAudioSource;
    }

    void SetUp()
    {
        GetAudioControlSource();

        button = GetComponent<Button>();

        if(button != null)
            button.onClick.AddListener(() => PlaySound());
    }

    public void PlaySound()
    {
        if (ExternalAudioSource == null)
            GetAudioControlSource();

        ExternalAudioSource.PlayOneShot(sound, oneShotVolume);
    }
    
}
