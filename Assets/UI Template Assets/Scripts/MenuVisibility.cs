using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuVisibility : MonoBehaviour 
{
    public GameObject MainPanel;
    public GameObject OpenMenuButton;
    public bool DisableOpenButton = false;
    public bool VisibleOnStart = false;
    public UnityEvent OnShowEvent;
    public UnityEvent OnHideEvent;

    public Font buttonFont;

    private void Start()
    {
        if(Application.isPlaying)
        {
            if(VisibleOnStart)
                ShowMenu(true);

            if (DisableOpenButton)
                ShowOpenButton(false);
            else
                ShowOpenButton(true);
        }
    }
    

    private void Update()
    {
        if (OpenMenuButton != null)
        {
            if (DisableOpenButton)
            {
                if (OpenMenuButton != null && OpenMenuButton.activeSelf)
                    OpenMenuButton.SetActive(false);
            }
        }
    }
    
    public void ShowOpenButton(bool enable)
    {
        if(OpenMenuButton != null)
            OpenMenuButton.SetActive(enable);
    }

    public void ShowMenu(bool enabled)
    {
        if (MainPanel != null)
            MainPanel.SetActive(enabled);

        if (OpenMenuButton != null)
            OpenMenuButton.SetActive(!enabled);

        if (enabled)
        {
            if (OnShowEvent != null)
                OnShowEvent.Invoke();
        }
        else
        {
            if (OnHideEvent != null)
                OnHideEvent.Invoke();
        }
    }

    public void UpdateAllButtonFonts()
    {
        if (buttonFont == null)
            return;

        Button[] buttons = GetComponentsInChildren<Button>();

        foreach(Button button in buttons)
        {
            Text t = button.GetComponentInChildren<Text>();
            if (t == null)
                continue;

            t.font = buttonFont;
        }
    }
}
