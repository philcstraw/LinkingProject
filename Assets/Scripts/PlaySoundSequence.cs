using System.Collections.Generic;
using UnityEngine;

public class PlaySoundSequence : MonoBehaviour
{
    [Range(0, 1)]
    public float volume;
    public List<PlaySound> soundList;

    int _currentSound = 0;

    internal void PlaySequence()
    {
        if (soundList.Count == 0)
            return;

        soundList[_currentSound].oneShotVolume = volume;
        soundList[_currentSound].PlayOneShot();

        _currentSound++;

        if (_currentSound >= soundList.Count)
            _currentSound = 0;
    }
}
