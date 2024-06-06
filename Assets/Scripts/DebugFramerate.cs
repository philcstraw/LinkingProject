using TMPro;
using UnityEngine;

public class DebugFramerate : MonoSingleton<DebugFramerate> 
{
    public TextMeshProUGUI debugText;
    public bool showFPS;
    float m_deltaTime = 0f;
    
	void Update () 
    {
        m_deltaTime += (Time.unscaledDeltaTime - m_deltaTime) * 0.1f;

        if (showFPS && debugText != null)
        {
            if (!debugText.transform.parent.gameObject.activeSelf)
                debugText.transform.parent.gameObject.SetActive(true);

            debugText.text = (1f/m_deltaTime).ToString();
        }
    }
}
