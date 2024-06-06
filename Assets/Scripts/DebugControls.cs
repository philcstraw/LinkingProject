using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR //Custom editor code won't compile in final builds, so use this if def
using UnityEditor;

[CustomEditor(typeof(ScoreGrid))]
public class DebugScoreGrid : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ScoreGrid sc = (ScoreGrid)target;

        if (GUILayout.Button("Reset All Scores"))
        {
            Debug.Log("Functionality removed");
        }
    }
}

[CustomEditor(typeof(ChangeAllFonts))]
public class DebugGlobalUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ChangeAllFonts sc = (ChangeAllFonts)target;

        if (GUILayout.Button("Update All Fonts"))
            sc.UpdateAll();

        if (GUILayout.Button("Update Normal Fonts"))
            sc.UpdateAllNormalFonts();

        if (GUILayout.Button("Update TextMeshPro Fonts"))
            sc.UpdateAllTextMeshProFonts();

        if (GUILayout.Button("Update TextMeshProUGUI Fonts"))
            sc.UpdateAllTextMeshProUGUIFonts();

        if (GUILayout.Button("Update All Button Images"))
            sc.UpdateAllButtonImages();
    }
}

[CustomEditor(typeof(PlaySound))]
public class DebugPlaySound : Editor
{
    //used to track whihc clips are playing.  probably don't need this, just use last clip instead but keeping this hack for now
    static List<PlaySound> clips = new List<PlaySound>();
    static PlaySound lastClip;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlaySound sc = (PlaySound)target;
        if (sc == null)
            return;

        if (GUILayout.Button("Preview"))
        {
            if (!clips.Contains(sc))
            {
                if (lastClip != null)
                {
                    Utility.StopClip(lastClip.soundEffect);
                    clips.Remove(lastClip);
                }
                Utility.PlayClip(sc.soundEffect);
                clips.Add(sc);
                lastClip = sc;
            }
        }

        if(clips.Contains(sc))
        {
            if (GUILayout.Button("Stop Preview"))
            {
                Utility.StopClip(sc.soundEffect);
                clips.Remove(sc);
                if (lastClip == sc)
                    lastClip = null;
            }

            //for some reason using just !IsPlayingClip returns false more often than we'd like
            if (Utility.IsPlayingClip(sc.soundEffect))
            {
            }
            else
            {
                Utility.StopClip(sc.soundEffect);
                clips.Remove(sc);
                if (lastClip == sc)
                    lastClip = null;
                EditorUtility.SetDirty(target);
            }
        }
    }
}

[CustomEditor(typeof(ChangeAllButtonSounds))]
public class DebugChangeMenuButtonSounds : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ChangeAllButtonSounds sc = (ChangeAllButtonSounds)target;

        if (GUILayout.Button("Update All Button Sounds"))
            sc.UpdateAllSounds();

        if (GUILayout.Button("Update Button Sound Volumes"))
            sc.UpdateAllOneShotVolumes();
    }
}

[CustomEditor(typeof(QuickScreenCapture))]
public class DebugScreenShotControls : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        QuickScreenCapture sc = (QuickScreenCapture)target;

        //need to run screenshot script from custom inspector so screenshots can be taken in edit mode
        sc.RunLogic(); 
    }
}
#endif