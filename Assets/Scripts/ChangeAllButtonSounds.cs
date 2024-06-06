using UnityEngine;

[ExecuteInEditMode]
public class ChangeAllButtonSounds : MonoBehaviour 
{    
    [Range(0.0f, 1.0f)]
    public float oneShotVolume = 1.0f;
    public AudioClip soundEffect;

    public void UpdateAllSounds()
    {
        ButtonSound[] _buttons = Resources.FindObjectsOfTypeAll<ButtonSound>();
        foreach (ButtonSound _button in _buttons)
            _button.sound = soundEffect;

        Debug.Log(string.Concat("ChangeAllButtonSounds script:  UpdateAllSounds method updated ", _buttons.Length, " scripts"));
    }

    public void UpdateAllOneShotVolumes()
    {
        ButtonSound[] _buttons = Resources.FindObjectsOfTypeAll<ButtonSound>();
        foreach (ButtonSound _button in _buttons)
            _button.oneShotVolume = oneShotVolume;

        Debug.Log(string.Concat("ChangeAllButtonSounds script:  UpdateAllOneShotVolumes method updated ", _buttons.Length, " scripts"));
    }
}
