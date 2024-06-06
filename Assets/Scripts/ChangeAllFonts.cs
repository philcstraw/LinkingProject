using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Used to change all the font in the scene from the inspector
[ExecuteInEditMode]
public class ChangeAllFonts : MonoBehaviour
{
    public Font font;
    public TMP_FontAsset textMeshProFont;
    public Sprite buttonImage;
    
    // update all unity text fonts in the game
    public void UpdateAllNormalFonts()
    {
        if (font == null)
        {
            Debug.LogError("Assign a Font");
            return;
        }

        Text[] _texts = Resources.FindObjectsOfTypeAll<Text>();
        foreach (Text _text in _texts)
            _text.font = font;
    }

    // update all TextMeshProUGUI objects with a compatible font
    public void UpdateAllTextMeshProUGUIFonts()
    {
        if (textMeshProFont == null)
        {
            Debug.LogError("Assign a Text Mesh Pro compatible Font");
            return;
        }

        TextMeshProUGUI[] _textMeshesGUI = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        foreach (TextMeshProUGUI _text in _textMeshesGUI)
            _text.font = textMeshProFont;
    }

    // update all TextMeshPro objects with a compatible font
    public void UpdateAllTextMeshProFonts()
    {
        if (textMeshProFont == null)
        {
            Debug.LogError("Assign a Text Mesh Pro compatible Font");
            return;
        }

        TextMeshPro[] _textMeshes = Resources.FindObjectsOfTypeAll<TextMeshPro>();
        foreach (TextMeshPro _text in _textMeshes)
            _text.font = textMeshProFont;
    }

    public void UpdateAllButtonImages()
    {
        if (buttonImage == null)
            return;

        ButtonSound[] _buttonSounds = FindObjectsOfType<ButtonSound>();
        foreach(ButtonSound _buttonSound in _buttonSounds)
        {
            if (!_buttonSound.menuButton)
                continue;

            Button _button = _buttonSound.gameObject.GetComponent<Button>();
            _button.image.sprite = buttonImage;
        }
    }

    // update all unity and TextMeshPro fonts to text objects
    public void UpdateAll()
    {
        UpdateAllNormalFonts();

        UpdateAllTextMeshProFonts();

        UpdateAllTextMeshProUGUIFonts();

        UpdateAllButtonImages();
    }
}
