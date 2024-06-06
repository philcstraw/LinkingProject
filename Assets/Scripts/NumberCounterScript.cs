using TMPro;
using UnityEngine;
using UnityEngine.UI;

// menu element used to increment and decrement values
public enum NumberCounterMode { Clamp, Wrap, Unbounded };

public class NumberCounterScript : MonoBehaviour 
{
    public TextMeshProUGUI textMeshProUGUI;
    public NumberCounterMode numberCounterMode;
    public Button selectUpButton;
    public Button selectDownButton;
    public int initialValue;
    public int Max = 100;
    public int Min = 0;
    int m_number;
    public int Number { get { return m_number; } }
    string m_numberString;
    bool m_buttonState = false;

    void Start()
    {
        SetNumber(initialValue);
    }

    void Update()
    {
        if (textMeshProUGUI != null)
            textMeshProUGUI.text = m_numberString;
    }

    public void SetNumber(int value)
    {
        switch(numberCounterMode)
        {
            case NumberCounterMode.Clamp:
                m_number = ClampValue(value);
                break;
            case NumberCounterMode.Wrap:
                m_number = WrapValue(value);
                break;

            case NumberCounterMode.Unbounded:
                m_number = value;
                break;
        }
        
        m_numberString = m_number.ToString();
    }

    public void Increase()
    {
        int n = m_number + 1;

        SetNumber(n);
    }

    public void Decrease()
    {
        int n = m_number - 1;

        SetNumber(n);
    }

    public void EnableButtons(bool state)
    {
        m_buttonState = state;

        if (selectUpButton)
            selectUpButton.gameObject.SetActive(state);

        if (selectDownButton)
            selectDownButton.gameObject.SetActive(state);
    }

    public bool AreButtonsEnabled()
    {
        return m_buttonState;
    }

    int ClampValue(int value)
    {
        if (value > Max)
                value = Max;

        if (value < Min)
                value = Min;

        return value;
    }

    int WrapValue(int value)
    {
        if (value > Max)
            return Min;

        if (value < Min)
            return Max;

        return value;
    }
}
