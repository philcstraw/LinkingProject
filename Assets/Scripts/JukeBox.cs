using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MusicTrack
{
	[Range(0, 1)]
	public float volume = 1f;
	public AudioClip clip;
    public string trackName = "Untitled Track";
}

public class JukeBox : MonoBehaviour 
{
	public List<MusicTrack> tracks = new List<MusicTrack>();
    public int selectedTrack = 0;

    public MusicTrack CurrentTrack()
    {
        return tracks[selectedTrack];
    }

    public bool ShouldRandomise()
    {
        return tracks[selectedTrack].trackName == "Random";
    }

    public void RandomiseTrack()
    {
        List<MusicTrack> _tmp = new List<MusicTrack>();
        for(int i=0; i < tracks.Count; i++)
        {
            if (i != selectedTrack)
                _tmp.Add(tracks[i]);
        }

        int _random = UnityEngine.Random.Range(0, _tmp.Count);
        var _randomPick = _tmp[_random];

        // Assumes the current selected track is the slot for random track.
        // Therefore, we overwrite the selected track with whatever other track we selected.
        MusicTrack _track = tracks[selectedTrack];
        _track.clip = _randomPick.clip;
        _track.volume = _randomPick.volume;
    }
}
