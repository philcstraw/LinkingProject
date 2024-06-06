using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour 
{
    public Button levelSelectButton;
    public NumberCounterScript levelCounterUIScript;
    public NumberCounterScript musicCounterUIScript;
    public TextMeshProUGUI musicTrackLabel;
    public GameObject muteImage;

    internal AudioControlVolumeUI volumeUI;

    int m_currentRank = 1;
    bool m_allowLevelSelect = false;

    void Awake()
    {
        // the settings menu exists in the root menu as well, so we'll make an exception and allow it to find the volume control.
        volumeUI = FindObjectOfType<AudioControlVolumeUI>();

        if (!m_allowLevelSelect)
            levelCounterUIScript.EnableButtons(false);
    }

    void Start()
    {
        if (Game.instance != null)
        {
            m_allowLevelSelect = Game.instance.mode.allowLevelSelect;

            if (!m_allowLevelSelect)
            {
                if (levelCounterUIScript.AreButtonsEnabled())
                    levelCounterUIScript.EnableButtons(false);
            }
            else
            {
                if (!levelCounterUIScript.AreButtonsEnabled())
                    levelCounterUIScript.EnableButtons(true);

                UpdateLevelSelector();
            }
        }
        else
        {
            levelSelectButton.gameObject.SetActive(false);
        }
        UpdateMusicSelector();
    }

    public void ToggleMute()
    {
        volumeUI.audioControl.ToggleMute();
    }
    
    public void UpdateLevelSelector()
    {
        if (levelCounterUIScript != null && Game.instance != null)
        {
            int _levelNumber = Game.instance.mode.CurrentLevel();
            m_currentRank = _levelNumber;
            levelCounterUIScript.initialValue = _levelNumber;
            levelCounterUIScript.SetNumber(_levelNumber);
            levelCounterUIScript.Min = Game.instance.mode.MinLevel();
            levelCounterUIScript.Max = Game.instance.mode.numberOfLevels;
        }
    }

    public void OnMenuVisible()
    {
        if(m_allowLevelSelect)
            UpdateLevelSelector();
    }

    public void ReloadIfRankChanged()
    {
        if (m_allowLevelSelect && Game.instance.mode.CurrentLevel() != m_currentRank)
        {
            SetCurrentRank();

            SceneControl.instance.ReloadCurrentScene();
        }
    }

    public void SetCurrentRank()
    {
        Game.instance.mode.SetLevel(levelCounterUIScript.Number);

        Game.instance.mode.SaveCurrentLevel();
    }

    public void NextRank()
    {
        levelCounterUIScript.Increase();
        m_currentRank = levelCounterUIScript.Number;
    }

    public void PreviousRank()
    {
        levelCounterUIScript.Decrease();
        m_currentRank = levelCounterUIScript.Number;
    }

    public void UpdateMusicSelector()
    {
        if (musicCounterUIScript != null)
        {
            int _currentTrack = AudioControl.instance.CurrentTrackIndex() + 1;
            musicCounterUIScript.initialValue = _currentTrack;
            musicCounterUIScript.SetNumber(_currentTrack);
            musicCounterUIScript.Min = 1;
            musicCounterUIScript.Max = AudioControl.instance.MusicTrackCount();
            musicTrackLabel.text = AudioControl.instance.CurrentTrackName();
        }
    }

    public void NextTrack()
    {
        musicCounterUIScript.Increase();
        AudioControl.instance.NextMusicTrack();
        musicTrackLabel.text = AudioControl.instance.CurrentTrackName();
    }

    public void PreviousTrack()
    {
        musicCounterUIScript.Decrease();
        AudioControl.instance.PreviousMusicTrack();
        musicTrackLabel.text = AudioControl.instance.CurrentTrackName();
    }
}
