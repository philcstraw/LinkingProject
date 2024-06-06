using UnityEngine;

public class MobileScreenResolution : MonoBehaviour 
{
    public int targetFrameRate = 60;
    public int resolutionReduction = 2;
    public bool fixedFrameRate = true;

    void Start () 
    {
#if !UNITY_WEBGL
        RuntimePlatform platform = Application.platform;

        if (platform != RuntimePlatform.WebGLPlayer)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFrameRate;
        }

        if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
            Screen.SetResolution(Screen.width / resolutionReduction, Screen.height / resolutionReduction, true, targetFrameRate);
#endif
    }
}
