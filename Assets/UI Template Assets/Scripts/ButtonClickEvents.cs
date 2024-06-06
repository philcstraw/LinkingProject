using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ButtonClickEvents : MonoBehaviour 
{
    internal class EventLink
    {
        internal EventLink(Button b, UnityEvent e)
        {
            button = b;
            unityEvent = e;
            if(button != null)
            button.onClick.AddListener(() => InvokeEvent());
        }

        internal Button button;
        internal UnityEvent unityEvent;

        public void InvokeEvent()
        {
            if (button != null && unityEvent != null)
                unityEvent.Invoke();
        }
    }

    public List<Button> buttons = new List<Button>();
    public List<UnityEvent> OnClickEvents = new List<UnityEvent>();
    private List<EventLink> eventLinks = new List<EventLink>();

    void Start()
    {
        if (Application.isPlaying)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (i >= OnClickEvents.Count)
                    break;
                 if (buttons[i] == null)
                    continue;
                 if (OnClickEvents[i] == null)
                    continue;
                eventLinks.Add(new EventLink(buttons[i], OnClickEvents[i]));
            }
        }
    }
}
