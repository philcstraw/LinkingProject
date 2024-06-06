using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GridInt))]
[CustomPropertyDrawer(typeof(GridBool))]
[CustomPropertyDrawer(typeof(GridGameObject))]
[CustomPropertyDrawer(typeof(GridMeshTile))]
public class GridGenericPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PrefixLabel(position, label);
        Rect _rect = position;

        float h = 18f;
        _rect.y += h;

        SerializedProperty _data = property.FindPropertyRelative("data");
        
        if (_data == null)
            return;

        int _length = _data.arraySize;
        int _rows = property.FindPropertyRelative("columnLength").intValue;
        int _columns = property.FindPropertyRelative("rowLength").intValue;
        _rect.height += h * _columns;

        //2D array is indexed from bottom to top
        //but the drawer will start at the top going down
        //so iterate backwards to reverse the y axis
        for (int i = _rows - 1; i > -1; i--)
        {
            _rect.height = h;
            _rect.width = (position.width / _columns);

            for (int j = 0; j < _columns; j++)
            {
                EditorGUI.PropertyField(_rect, _data.GetArrayElementAtIndex(i * _columns + j), GUIContent.none);
                _rect.x += _rect.width;
            }

            _rect.x = position.x;
            _rect.y += h;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float h = 18f;
        int _rows = property.FindPropertyRelative("columnLength").intValue;
        return base.GetPropertyHeight(property, label) + h * _rows;
    }
}

[CustomPropertyDrawer(typeof(GridLevel),true)]
public class LevelGridPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PrefixLabel(position, label);
        Rect _rect = position;

        float h = 18f;
        _rect.y += h;

        SerializedProperty _data = property.FindPropertyRelative("data");

        if (_data == null)
            return;

        int _length = _data.arraySize;
        int _rows = property.FindPropertyRelative("columnLength").intValue;
        int _columns = property.FindPropertyRelative("rowLength").intValue;

        _rect.height += h * _columns;

        //2D array is indexed from bottom to top
        //but the drawer will start at the top going down
        //so iterate backwards to reverse the y axis

        for (int i = _rows - 1; i > -1; i--)
        {
            _rect.height = h;

            _rect.width = (position.width / _columns);

            for (int j = 0; j < _columns; j++)
            {
                SerializedProperty _sp = _data.GetArrayElementAtIndex(i * _columns + j);
                LevelTile _levelTile = _sp.objectReferenceValue as LevelTile;
                if (_levelTile == null)
                    return;

                PowerupBase _pb = null;
                if (_levelTile.meshTile != null)
                    _pb = _levelTile.meshTile.currentPowerUp;

                if (LevelLayout.instance == null || !LevelLayout.instance.debugShowPowerupsOnly)
                {
                    if (_pb != null)
                        EditorGUI.ObjectField(_rect, _pb, typeof(PowerupBase), true);
                    else
                        EditorGUI.PropertyField(_rect, _sp, GUIContent.none);
                }
                else
                    EditorGUI.ObjectField(_rect, _pb, typeof(PowerupBase), true);

                _rect.x += _rect.width;
            }

            _rect.x = position.x;
            _rect.y += h;
        }
        EditorUtility.SetDirty(property.serializedObject.targetObject);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float h = 18f;
        int _rows = property.FindPropertyRelative("columnLength").intValue;
        return base.GetPropertyHeight(property, label) + h * _rows;
    }
}