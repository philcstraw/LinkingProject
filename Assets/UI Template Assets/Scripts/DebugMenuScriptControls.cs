using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MenuVisibility))]
public class DebugMenuScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MenuVisibility sc = (MenuVisibility)target;

        if (GUILayout.Button("Update All Button Fonts"))
            sc.UpdateAllButtonFonts();

        if (GUILayout.Button("Debug Show Menu"))
            sc.ShowMenu(true);

        if (GUILayout.Button("Debug Hide Menu"))
            sc.ShowMenu(false);

        if (GUILayout.Button("Debug Show Open Button"))
            sc.ShowOpenButton(true);

        if (GUILayout.Button("Debug Hide Open Button"))
            sc.ShowOpenButton(false);
    }
}
#endif