using UnityEngine;

[System.Serializable]
public class GridGameObject : GridObject<GameObject>
{
}

[System.Serializable]
public class GridLevel : GridObject<LevelTile>
{
}

[System.Serializable]
public class GridMeshTile : GridObject<MeshTile>
{
}

[System.Serializable]
public class GridObject<T> : GridGeneric<T>
{
}

[System.Serializable]
public class GridBool : GridGeneric<bool>
{
}

[System.Serializable]
public class GridInt : GridGeneric<int>
{
}

[System.Serializable]
public class GridGenericBase : MonoBehaviour
{
}

[System.Serializable]
public class GridGeneric<T>
{
    public GridGeneric()
    {
        UpdateRowsAndColumns();
    }

    public int totalSize;
    
    public int columnLength = 0; //=number of rows

    public int rowLength = 0; //=number of columns

    T[] m_data;

    public void Size(int ColumnLength, int RowLength)
    {
        columnLength = ColumnLength;
        rowLength = RowLength;
        UpdateRowsAndColumns();
    }

    public void UpdateRowsAndColumns()
    {
        totalSize = columnLength * rowLength;
        m_data = new T[totalSize];
    }

    public T this[int rowIndex, int columnIndex]
    {
        get
        {
            return m_data[rowIndex + (columnIndex * rowLength)];
        }
        set
        {
            m_data[rowIndex + (columnIndex * rowLength)] = value;
        }
    }
    
    public Vector2 CalculateCentre()
    {
        float _avgX = 0;
        float _avgY = 0;

        for (int i = 0; i < rowLength; i++)
            _avgX += i;
        for (int i = 0; i < columnLength; i++)
            _avgY += i;

        Vector2 offset = new Vector2(_avgX / rowLength, _avgY / columnLength);

        return offset;
    }
}