using UnityEngine;
using UnityEngine.UI;

// Remove quite button if web gl build. Calling Application.Quit doesn't do anything in web builds.
public class WebGL : MonoBehaviour 
{
	void Start () 
    {
#if UNITY_WEBGL

        Button[] _buttons = Resources.FindObjectsOfTypeAll<Button>();
        int _count = _buttons.Length;

        for(int i=0;i<_count;i++)
        {
            Text _text = _buttons[i].GetComponentInChildren<Text>();
            if (_text != null)
            {
                if (_text.text == "Quit")
                    _buttons[i].gameObject.SetActive(false);
            }
        }
#endif
	}
}
