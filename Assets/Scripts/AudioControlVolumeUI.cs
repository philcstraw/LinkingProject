using TMPro;
using UnityEngine;
using UnityEngine.UI;

// used to update audio UI elements
public class AudioControlVolumeUI : MonoBehaviour 
{
    // UI elements to update
    public TextMeshProUGUI globalVolumeText;
    public TextMeshProUGUI musicVolumeText;
    public TextMeshProUGUI gameVolumeText;
    public Text muteButtonText;
    public Image muteImage;
    // text on mute button to switch to when mute is toggled
    public string muteButtonString = "Mute";
    public string unmuteButtonString = "Unmute";

    internal static AudioControlVolumeUI instance;
    
    internal AudioControl audioControl
    {
        get
        {
            return AudioControl.instance;
        }
    }

    void Awake()
    {
        instance = this;
    }

    public void ToggleMute()
    {
        audioControl.ToggleMute();
    }

    //global volume
    public void IncreaseGlobalVolume()
    {
        audioControl.IncreaseGlobalVolume();
    }

    public void DecreaseGlobalVolume()
    {
        audioControl.DecreaseGlobalVolume();
    }

    //music volume
    public void IncreaseMusicVolume()
    {
        audioControl.IncreaseMusicVolume();
    }

    public void DecreaseMusicVolume()
    {
        audioControl.DecreaseMusicVolume();
    }

    //game volume
    public void IncreaseGameVolume()
    {
        audioControl.IncreaseGameVolume();
    }

    public void DecreaseGameVolume()
    {
        audioControl.DecreaseGameVolume();
    }
    
    void UpdateMute(bool mute)
    {
        if (mute)
        {
            if (muteButtonText != null)
                muteButtonText.text = unmuteButtonString;

            if (muteImage != null)
                muteImage.enabled = true;
        }else{
            if (muteButtonText != null)
                muteButtonText.text = muteButtonString;

            if (muteImage != null)
                muteImage.enabled = false;
        }
    }

    void Update ()
    {
        if (audioControl == null)
            return;

        if (globalVolumeText != null)
            globalVolumeText.text = audioControl.globalVolume.ToString();

        if (musicVolumeText != null)
            musicVolumeText.text = audioControl.musicVolume.ToString();
        
        if (gameVolumeText != null)
            gameVolumeText.text = audioControl.gameVolume.ToString();
        
        UpdateMute(audioControl.muted);
	}
}
