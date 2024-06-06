using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;

public class AudioAutoplay : EditorWindow
{
    [MenuItem("Tools/Audio Autoplay")]
    static void Init()
    {
        var _window = EditorWindow.GetWindow(typeof(AudioAutoplay));
        _window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Audio files will now play on selection change.");
    }

    void OnSelectionChange()
    {
        UnityEngine.Object[] _clips = Selection.GetFiltered(typeof(AudioClip), SelectionMode.Unfiltered);

        if (_clips != null && _clips.Length == 1)
        {
            AudioClip _clip = (AudioClip)_clips[0];
            PlayClip(_clip);
        }
    }

    public static void PlayClip(AudioClip clip)
    {
        Assembly _unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type _audioUtilClass = _unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo _method = _audioUtilClass.GetMethod("PlayClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[] { typeof(AudioClip) },
            null);

        _method.Invoke(null, new object[] { clip });
    }
}