using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollapseDirection { None,  Down, Up , Right, Left }

// Handles Tile creation and collapsing
public class MeshTileManager : MonoBehaviour
{
    public MeshTile baseMeshTile;
    public LevelLayout levelLayout;
    public List<Material> materials = new List<Material>(5);
    [Header("")]
    // Default to ease out curve in the inspector.
    public AnimationCurve startScaleCurve = new AnimationCurve();
    public float startScaleSpeed = 1f;
    public float createTileDelay = 0.5f;
    public float popSpeed = 5f;
    public float popScale = 0.05f;
    public float popZOffset = 0.2f;
    [Header("Reference Colours")]
    public List<Color32> colors = new List<Color32>(5);

    internal Vector3 defaultMeshTileScale;
    internal CollapseDirection collapseDirection = CollapseDirection.Down;
    internal bool initialFill { get { return m_initialFill >= m_cellCount; } }
    internal int filledCells { get { return m_initialFill; } }
    //internal bool gridFilled { get { return m_numMeshTiles == levelLayout.levelGrid.totalSize; } }
    internal static MeshTileManager instance;


    enum ShiftDirection { Forward, Backward }
    GridLevel m_grid;
    int m_numberofTypes = 5;
    int m_initialFill = 0;
    int m_currentChipColumn;
    int m_currentChipRow;
    int m_cellCount;
    int m_numMeshTiles;

    public void Awake()
    {
        instance = this;

        m_grid = levelLayout.levelGrid;

        m_numberofTypes = materials.Count;

        defaultMeshTileScale = baseMeshTile.transform.localScale;

        m_numMeshTiles = 0;
    }

    void Start()
    {
        m_cellCount = levelLayout.cellCount;
    }

    void Update()
    {
        if (!initialFill)
            InitialMeshTileFill();
        else
            ShiftCells();
    }

    internal void InitialMeshTileFill()
    {
        for (int i = 0; i < m_cellCount; i++)
        {
            LevelTile _levelTile = levelLayout.levelCells[i];

            if (_levelTile.meshTile == null)
            {
                CreateNewTile(_levelTile);
                m_initialFill++;
                return;
            }
        }
    }

    internal bool HasValidMove()
    {
        for (int i = 0; i < m_cellCount; i++)
        {
            LevelTile _tile = levelLayout.levelCells[i];

            if (_tile.meshTile == null)
                return true;

            if (HasValidSidling(_tile.meshTile))
                return true;
        }
        return false;
    }

    internal List<LevelTile> GetColumn(int index)
    {
        List<LevelTile> _list = new List<LevelTile>();

        for (int y = 0; y < levelLayout.columnLength; y++)
            _list.Add(levelLayout.levelGrid[index, y]);

        return _list;
    }

    internal List<LevelTile> GetRow(int index)
    {
        List<LevelTile> _list = new List<LevelTile>();

        for (int x = 0; x < levelLayout.rowLength; x++)
            _list.Add(levelLayout.levelGrid[x, index]);

        return _list;
    }

    internal List<MeshTile> GetAllType(TileType type)
    {
        List<MeshTile> _list = new List<MeshTile>();

        foreach (LevelTile _tile in levelLayout.levelCells)
        {
            if (_tile.meshTile != null && _tile.meshTile.type == type)
                _list.Add(_tile.meshTile);
        }
        return _list;
    }

    internal List<MeshTile> GetRadial(LevelTile tile)
    {
        List<MeshTile> _list = new List<MeshTile>();

        //top
        LevelTile t = levelLayout.GetLevelCell(tile.x, tile.y + 1);
        if (t != null)
            _list.Add(t.meshTile);
        //top right
        LevelTile tr = levelLayout.GetLevelCell(tile.x + 1, tile.y + 1);
        if (tr != null)
            _list.Add(tr.meshTile);
        //right
        LevelTile r = levelLayout.GetLevelCell(tile.x + 1, tile.y);
        if (r != null)
            _list.Add(r.meshTile);
        //bottom right
        LevelTile br = levelLayout.GetLevelCell(tile.x + 1, tile.y - 1);
        if (br != null)
            _list.Add(br.meshTile);
        //bottom
        LevelTile b = levelLayout.GetLevelCell(tile.x, tile.y - 1);
        if (b != null)
            _list.Add(b.meshTile);
        //bottom left
        LevelTile bl = levelLayout.GetLevelCell(tile.x - 1, tile.y - 1);
        if (bl != null)
            _list.Add(bl.meshTile);
        //left
        LevelTile l = levelLayout.GetLevelCell(tile.x - 1, tile.y);
        if (l != null)
            _list.Add(l.meshTile);
        //left top
        LevelTile lt = levelLayout.GetLevelCell(tile.x - 1, tile.y + 1);
        if (lt != null)
            _list.Add(lt.meshTile);

        return _list;
    }

    internal void ShiftCells()
    {
        for (int i = 0; i < m_cellCount; i++)
        {
            LevelTile _tile = levelLayout.levelCells[i];

            if (_tile.meshTile == null)
            {
                switch (collapseDirection)
                {
                    case CollapseDirection.Down:
                        ShiftTiles(GetColumn(_tile.x), ShiftDirection.Forward);
                        break;
                    case CollapseDirection.Up:
                        ShiftTiles(GetColumn(_tile.x), ShiftDirection.Backward);
                        break;
                    case CollapseDirection.Left:
                        ShiftTiles(GetRow(_tile.y), ShiftDirection.Forward);
                        break;
                    case CollapseDirection.Right:
                        ShiftTiles(GetRow(_tile.y), ShiftDirection.Backward);
                        break;
                    default:
                        break;
                }
            }
        }

        if (IsGridMoving())
            return;

        for (int i = 0; i < m_cellCount; i++)
        {
            LevelTile _tile = levelLayout.levelCells[i];
            if (_tile.meshTile == null)
                CreateNewTile(_tile);
        }
    }

    internal bool IsGridMoving()
    {
        for (int i = 0; i < m_cellCount; i++)
        {
            LevelTile _tile = levelLayout.levelCells[i];

            if (_tile.meshTile == null)
                continue;

            if (_tile.meshTile.moving)
                return true;
        }
        return false;
    }

    internal bool IsValidSidling(MeshTile meshTile, MeshTile sidling)
    {
        List<MeshTile> _list = GetAdjacentCellsOfMyType(meshTile);

        if (_list.Contains(sidling))
            return true;

        return false;
    }

    internal LevelTile GetRandomTileForPersistence()
    {
        List<LevelTile> tiles = new List<LevelTile>();
        for (int i = 0; i < levelLayout.cellCount; i++)
        {
            LevelTile tile = levelLayout.levelCells[i];
            if (tile.meshTile == null)
                continue;
            if (tile.meshTile.persistent)
                continue;

            tiles.Add(tile);
        }

        if (tiles.Count == 0)
            return null;

        int pick = UnityEngine.Random.Range(0, tiles.Count - 1);
        return tiles[pick];
    }

    internal void SwapTiles(MeshTile meshTile1, MeshTile meshTile2)
    {
        SwapTileMaterials(meshTile1, meshTile2);
    }

    internal void ShiftAllTilesRoutine()
    {
        Vector3 _centre = levelLayout.levelGrid.CalculateCentre();

        StartCoroutine(ShiftAllTiles(levelLayout.WorldOffset + _centre));
    }

    IEnumerator ShiftAllTiles(Vector3 centre)
    {
        foreach (LevelTile _tile in levelLayout.levelCells)
        {
            if (_tile.meshTile == null)
                continue;

            Vector4 _op = _tile.transform.position;
            _op.w = popSpeed;

            Vector3 diff = _tile.transform.position - centre;
            diff.Normalize();

            Vector4 _np = new Vector4(_op.x + diff.x * popScale, _op.y + diff.y * popScale, _op.z - popZOffset, popSpeed);
            _tile.meshTile.PopPosition(_op, _np);

            yield return null;
        }
    }

    void ShiftTile(LevelTile current, LevelTile nextTile)
    {
        nextTile.meshTile.tile = current;
        current.meshTile = nextTile.meshTile;
        current.meshTile.MoveTo(levelLayout.GridPosition(current.x, current.y));
        nextTile.meshTile = null;
    }

    bool ShiftTiles(List<LevelTile> list, ShiftDirection direction)
    {
        int _count = list.Count;
        int i = 0;
        int _limit = list.Count - 1;
        int _inc = 1;

        if (direction == ShiftDirection.Backward)
        {
            _count = -1;
            i = list.Count - 1;
            _limit = 0;
            _inc = -1;
        }

        while (i != _limit)
        {
            LevelTile _current = list[i];
            if (_current.meshTile == null)
            {
                int j = i + _inc;
                while (j != _count)
                {
                    LevelTile _nextTile = list[j];
                    // Stops the tile from falling under gravity and acts are a support for any falling blocks above.
                    if (_nextTile.fixedIfPersistent && _nextTile.meshTile != null && _nextTile.meshTile.persistent)
                        break;

                    if (_nextTile.meshTile != null)
                    {
                        if (_nextTile.meshTile.moving)
                            return false;

                        ShiftTile(_current, _nextTile);
                        break;
                    }
                    j += _inc;
                }
            }
            i += _inc;
        }
        return true;
    }

    MeshTile CreateNewTile(LevelTile levelTile)
    {
        MeshTile _meshTile = Instantiate(baseMeshTile);
        GameObject _go = _meshTile.gameObject;
        _go.SetActive(true);

        levelTile.meshTile = _meshTile;
        _meshTile.tile = levelTile;
        _meshTile.type = (TileType)(UnityEngine.Random.Range(0, m_numberofTypes));
        _meshTile.startSize = defaultMeshTileScale;

        UpdateColor(_meshTile, _meshTile.type);

        _meshTile.transform.localScale = Vector3.zero;
        _go.transform.SetParent(levelTile.gameObject.transform, false);
        _go.transform.position = levelTile.gameObject.transform.position;
        _meshTile.ScaleToDelayed(defaultMeshTileScale, startScaleSpeed, startScaleCurve, createTileDelay);

        //m_numMeshTiles = Mathf.Min(m_numMeshTiles + 1, levelLayout.levelGrid.totalSize);

        return _meshTile;
    }

    internal bool IsGridFilled()
    {
        for (int i = 0; i < m_cellCount; i++)
        {
            LevelTile _tile = levelLayout.levelCells[i];
            if (_tile.meshTile == null)
                return false;
        }
        return true;
    }

    internal int NumMeshTiles()
    {
        int count = 0;
        for (int i = 0; i < m_cellCount; i++)
        {
            LevelTile _tile = levelLayout.levelCells[i];
            if (_tile.meshTile != null)
                count++;
        }
        return count;
    }

    internal void DestoryMeshTile(MeshTile meshTile)
    {
        meshTile.tile.available = true;
        Destroy(gameObject);
        m_numMeshTiles = Mathf.Max(m_numMeshTiles - 1, 0);
    }

    void UpdateColor(MeshTile tile, TileType type)
    {
        tile.type = type;
        tile.UpdateMaterialFromType( );
    }

    void SwapTileMaterials(MeshTile meshTile1, MeshTile meshTile2)
    {
        TileType _oldType = meshTile2.type;

        meshTile2.type = meshTile1.type;

        meshTile1.type = _oldType;

        meshTile1.UpdateMaterialFromType();

        meshTile2.UpdateMaterialFromType();
    }

    //does the cell at index X Y have the same type
    bool CheckSidling(TileType type, int indexX, int indexY)
    {
        if (indexX >= m_grid.rowLength || indexY >= m_grid.columnLength || indexX < 0 || indexY < 0)
            return false;

        LevelTile _levelTile = m_grid[indexX, indexY];

        MeshTile _sidling = _levelTile.meshTile;

        if (_sidling == null)
            return false;

        if (_sidling.type == type)
            return true;

        return false;
    }

    bool HasValidSidling(MeshTile meshTile)
    {
        //left
        if (CheckSidling(meshTile.type,meshTile.tile.x - 1, meshTile.tile.y))
            return true;
        //right
        if (CheckSidling(meshTile.type, meshTile.tile.x + 1, meshTile.tile.y))
            return true;
        //up
        if (CheckSidling(meshTile.type, meshTile.tile.x, meshTile.tile.y + 1))
            return true;
        //down
        if (CheckSidling(meshTile.type, meshTile.tile.x, meshTile.tile.y - 1))
            return true;

        return false;
    }

    void GetChainRecursive(MeshTile meshTile, List<MeshTile> chain)
    {
        if (meshTile == null)
            return;

        List<MeshTile> adjCells = GetAdjacentCellsOfMyType(meshTile);

        if (!chain.Contains(meshTile))
            chain.Add(meshTile);

        int count = adjCells.Count;
        for(int i=0;i<count;i++)
        {
            MeshTile m = adjCells[i];
            if (chain.Contains(m))
                continue;

            GetChainRecursive(m, chain);
        }
    }

    //get all adjacent cells of my type
    List<MeshTile> GetAdjacentCellsOfMyType(MeshTile meshTile)
    {
        return GetAdjacentCellsOfType(meshTile.tile,meshTile.type);
    }

    //get all adjacent cells of the specified type
    List<MeshTile> GetAdjacentCellsOfType(LevelTile cell, TileType checkType)
    {
        List<MeshTile> cells = new List<MeshTile>();

        MeshTile left = GetSameType(cell.x - 1, cell.y, checkType);
        if (left != null)
            cells.Add(left);

        MeshTile right = GetSameType(cell.x + 1, cell.y, checkType);
        if (right != null)
            cells.Add(right);

        MeshTile up = GetSameType(cell.x, cell.y + 1, checkType);
        if (up != null)
            cells.Add(up);

        MeshTile down = GetSameType(cell.x, cell.y - 1, checkType);
        if (down != null)
            cells.Add(down);

        return cells;
    }

    //get cell of the specified type
    MeshTile GetSameType(int indexX, int indexY, TileType checkType)
    {
        if (indexX >= m_grid.rowLength || indexY >= m_grid.columnLength || indexX < 0 || indexY < 0)
            return null;
        
        LevelTile lc = m_grid[indexX, indexY];
        
        MeshTile sid = lc.meshTile;

        if (sid == null || sid.gameObject == null)
            return null;

        if (sid.type == checkType)
            return sid;

        return null;
    }

    internal void ChipTile()
    {
        if (collapseDirection == CollapseDirection.Down)
        {
            LevelTile _levelTile = null;

            m_currentChipRow = 0;
            _levelTile = GetChipColumn(m_currentChipColumn, m_currentChipRow, false, false);
            if (_levelTile == null)
                return;

            m_currentChipColumn = _levelTile.x + 1;
            if (_levelTile.meshTile == null || _levelTile.meshTile.delete)
                return;

            _levelTile.meshTile.MoveTo(_levelTile.meshTile.transform.position + Vector3.down * 5);
            _levelTile.Delete(0.5f);
        }

        if (collapseDirection == CollapseDirection.Up)
        {
            LevelTile _levelTile = null;
            m_currentChipRow = levelLayout.levelGrid.columnLength - 1;
            _levelTile = GetChipColumn(m_currentChipColumn, m_currentChipRow, false, true);
            if (_levelTile.meshTile == null || _levelTile.meshTile.delete)
                return;

            m_currentChipColumn = _levelTile.x + 1;
            _levelTile.meshTile.MoveTo(_levelTile.meshTile.transform.position - Vector3.down * 5);
            _levelTile.Delete(0.5f);
        }

        if (collapseDirection == CollapseDirection.Left)
        {
            LevelTile _levelTile = null;
            m_currentChipColumn = 0;
            _levelTile = GetChipRow(m_currentChipColumn, m_currentChipRow, false, false);

            if (_levelTile.meshTile == null || _levelTile.meshTile.delete)
                return;

            m_currentChipRow = _levelTile.y + 1;
            _levelTile.meshTile.MoveTo(_levelTile.meshTile.transform.position - Vector3.right * 5);
            _levelTile.Delete(0.5f);
        }

        if (collapseDirection == CollapseDirection.Right)
        {
            LevelTile _levelTile;
            m_currentChipColumn = levelLayout.levelGrid.rowLength - 1;
            _levelTile = GetChipRow(m_currentChipColumn, m_currentChipRow, true, false);

            if (_levelTile.meshTile == null || _levelTile.meshTile.delete)
                return;

            m_currentChipRow = _levelTile.y + 1;
            _levelTile.meshTile.MoveTo(_levelTile.meshTile.transform.position + Vector3.right * 5);
            _levelTile.Delete(0.5f);
        }
    }

    LevelTile GetChipColumn(int currentColumn, int currentRow, bool reverseX, bool reverseY)
    {
        int _columnLength = levelLayout.layoutGrid.columnLength;

        int _rowLength = levelLayout.layoutGrid.rowLength;

        if (!reverseX && currentColumn > _rowLength - 1)
            currentColumn = 0;

        if (reverseX && currentColumn < 0)
            currentColumn = _rowLength - 1;

        if (!reverseY && currentRow > _columnLength - 1)
            currentRow = 0;

        if (reverseY && currentRow < 0)
            currentRow = _columnLength - 1;

        int _incY = 1;
        int y = currentRow;
        int _limitY = _columnLength;

        int _incX = 1;
        int x = currentColumn;
        int _limitX = _rowLength;

        if (reverseY)
        {
            _incY = -1;
            _limitY = -1;
        }

        if (reverseX)
        {
            _incX = -1;
            _limitX = -1;
        }

        while (x != _limitX)
        {
            while (y != _limitY)
            {
                LevelTile t = levelLayout.levelGrid[x, y];
                if (!t.meshTile.persistent)
                    return t;

                y += _incY;
            }
            y = currentRow;
            x += _incX;
        }
        return null;
    }

    LevelTile GetChipRow(int currentColumn, int currentRow, bool reverseX, bool reverseY)
    {
        int _columnLength = levelLayout.layoutGrid.columnLength;

        int _rowLength = levelLayout.layoutGrid.rowLength;

        if (!reverseX && currentColumn > _rowLength - 1)
            currentColumn = 0;

        if (reverseX && currentColumn < 0)
            currentColumn = _rowLength - 1;

        if (!reverseY && currentRow > _columnLength - 1)
            currentRow = 0;

        if (reverseY && currentRow < 0)
            currentRow = _columnLength - 1;

        int _incY = 1;
        int y = currentRow;
        int _limitY = _columnLength;

        int _incX = 1;
        int x = currentColumn;
        int _limitX = _rowLength;

        if (reverseY)
        {
            _incY = -1;
            _limitY = -1;
        }

        if (reverseX)
        {
            _incX = -1;
            _limitX = -1;
        }

        while (y != _limitY)
        {
            while (x != _limitX)
            {
                LevelTile t = levelLayout.levelGrid[x, y];
                if (!t.meshTile.persistent)
                    return t;

                x += _incX;
            }
            x = currentColumn;
            y += _incY;
        }
        return null;
    }
}
