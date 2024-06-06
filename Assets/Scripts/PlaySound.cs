using UnityEngine;

[ExecuteInEditMode]
public class PlaySound : MonoBehaviour 
{
    [Range(0.0f, 1.0f)] public float oneShotVolume = 1.0f;
    [Range(-3.0f, 3.0f)] public float oneShotPitch = 1.0f;
    public bool PlayOnStart;
    public bool loop = false;
    public bool oneAtATime = false;
    public AudioClip soundEffect;
    public AudioSource audioSource;

    void Start ()
    {
        if (Application.isEditor)
            return;

        if (audioSource == null)
            audioSource = AudioControl.instance.gameAudioSource;

        if (audioSource == null)
            return;

        audioSource.clip = soundEffect;
        audioSource.loop = loop;

        if (PlayOnStart && Application.isPlaying)
            Play();
	}

    private void Update()
    {
        if (!Application.isEditor)
            return;

        if (soundEffect != null && soundEffect.name != gameObject.name)
            gameObject.name = soundEffect.name;
    }

    public void Play()
    {
        if (audioSource == null)
            audioSource = AudioControl.instance.gameAudioSource;
        if (audioSource == null||soundEffect == null)
            return;

        audioSource.clip = soundEffect;
        audioSource.loop = loop;

        audioSource.Play();
    }

    public void PlayOneShot()
    {
        if (soundEffect == null)
            return;

        if (audioSource == null)
            audioSource = AudioControl.instance.gameAudioSource;

        if (audioSource == null)
            return;

        if (oneAtATime && audioSource.isPlaying)
            return;

        audioSource.pitch = oneShotPitch;
        audioSource.PlayOneShot(soundEffect, oneShotVolume);
    }

    public void Stop()
    {
        if (audioSource == null)
            audioSource = AudioControl.instance.gameAudioSource;

        if (audioSource == null)
            return;

        audioSource.Stop();
    }
}
