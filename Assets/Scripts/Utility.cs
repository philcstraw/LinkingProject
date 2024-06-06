using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute { }

public class Utility : MonoBehaviour 
{
    public static float EaseOut(float t)
    {
        return Mathf.Sin(t * Mathf.PI * 0.5f);
    }

    public static float EaseIn(float t)
    {
        return 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
    }

    public static float Exponential(float t)
    {
        return t * t;
    }

    public static float SmoothStep(float t)
    {
        return t * t * (3f - 2f * t);
    }

    public static float SmootherStep(float t)
    {
        return t * t * t * (t * (6f * t - 15f) + 10f);
    }


#if UNITY_EDITOR
    public static void PlayClip(AudioClip clip)
    {
        Assembly _unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type _audioUtilClass = _unityEditorAssembly.GetType("UnityEditor.AudioUtil");
 
        MethodInfo _method = _audioUtilClass.GetMethod(
            "PlayClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[] {
             typeof(AudioClip)
        },
        null);

        _method.Invoke(null,new object[] {clip});
    }

    public static void StopClip(AudioClip clip)
    {
        Assembly _unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type _audioUtilClass = _unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo _stopMethod = _audioUtilClass.GetMethod("StopClip", BindingFlags.Static | BindingFlags.Public);

        _stopMethod.Invoke(null, new object[] {clip});
    }

    public static bool IsPlayingClip(AudioClip clip)
    {
        Assembly _unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type _audioUtilClass = _unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo _method = _audioUtilClass.GetMethod("IsClipPlaying", BindingFlags.Static | BindingFlags.Public);

        var rv = _method.Invoke(null, new object[] { clip });

        return (bool)rv;
    }

    public static void ExpandSceneHierarchy(UnityEngine.SceneManagement.Scene scene, bool expand)
    {
        foreach (var wnd in Resources.FindObjectsOfTypeAll<SearchableEditorWindow>())
        {
            if (wnd.GetType().Name != "SceneHierarchyWindow")
                continue;

            var _method = wnd.GetType().GetMethod("ExpandTreeViewItem", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new[] { typeof(int), typeof(bool) }, null);
            if (_method == null)
            {
                Debug.LogError("Could not find method 'UnityEditor.SceneHierarchyWindow.ExpandTreeViewItem(int, bool)'.");
                return;
            }

            var _field = scene.GetType().GetField("m_Handle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (_field == null)
            {
                Debug.LogError("Could not find field 'int UnityEngine.SceneManagement.Scene.m_Handle'.");
                return;
            }

            var _sceneHandle = _field.GetValue(scene);

            _method.Invoke(wnd, new System.Object[] { _sceneHandle, expand });
        }
    }
#endif

    public static int GetPlayerSettingInt(string settingName, int defaultValue)
    {
        return PlayerPrefs.GetInt(settingName, defaultValue);
    }

    public static int GetPlayerSettingInt(string settingName)
    {
        return PlayerPrefs.GetInt(settingName, 0);
    }

    public static bool GetPlayerSettingBool(string settingName,bool defaultValue)
    {
        return System.Convert.ToBoolean(PlayerPrefs.GetInt(settingName, System.Convert.ToInt32(defaultValue)));
    }

    public static bool GetPlayerSettingBool(string settingName)
    {
        return System.Convert.ToBoolean(PlayerPrefs.GetInt(settingName, 0));
    }

    public static float GetPlayerSettingFloat(string settingName,float defaultValue)
    {
        return PlayerPrefs.GetFloat(settingName, defaultValue);
    }

    public static float GetPlayerSettingFloat(string settingName)
    {
        return PlayerPrefs.GetFloat(settingName, 0);
    }
    
    public static string GetPlayerSettingString(string settingName,string defaultValue)
    {
        return PlayerPrefs.GetString(settingName, defaultValue);
    }

    public static string GetPlayerSettingString(string settingName)
    {
        return PlayerPrefs.GetString(settingName, "");
    }

    public static void SetPlayerSetting(string settingName, int value)
    {
        PlayerPrefs.SetInt(settingName, value);
    }

    public static void SetPlayerSetting(string settingName, bool value)
    {
        PlayerPrefs.SetInt(settingName, System.Convert.ToInt32(value));
    }

    public static void SetPlayerSetting(string settingName, float value)
    {
        PlayerPrefs.SetFloat(settingName, value);
    }

    public static void SetPlayerString(string settingName, string value)
    {
        PlayerPrefs.SetString(settingName, value);
    }

    public static void ResetPlayerSettingInt(string settingName)
    {
        PlayerPrefs.SetInt(settingName, 0);
    }

    public static void ResetPlayerSettingBool(string settingName)
    {
        PlayerPrefs.SetInt(settingName, 0);
    }

    public static void ResetPlayerSettingFloat(string settingName)
    {
        PlayerPrefs.SetFloat(settingName, 0);
    }

    public static void ResetPlayerSettingString(string settingName)
    {
        PlayerPrefs.SetString(settingName, "");
    }

    static GameObject FindInActiveObjectByTag(string tag)
    {
        Transform[] _objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];

        for (int i = 0; i < _objs.Length; i++)
        {
            if (_objs[i].hideFlags == HideFlags.None)
            {
                if (_objs[i].CompareTag(tag))
                    return _objs[i].gameObject;
            }
        }
        return null;
    }

    #region Safe Destroy
    public static void SafeDestroy(UnityEngine.Object obj)
    {
        if (Application.isEditor)
            UnityEngine.Object.DestroyImmediate(obj);
        else
            UnityEngine.Object.Destroy(obj);
    }
    
    public static T SafeDestroyGameObject<T>(T component) where T : Component
    {
        if (component != null)
            SafeDestroy(component.gameObject);

        return null;
    }
    #endregion
}