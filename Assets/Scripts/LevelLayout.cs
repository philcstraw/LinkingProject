using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelLayout : MonoBehaviour 
{
    public LevelTile levelTilePrefab;
    public int rowLength = 5;
    public int columnLength = 7;
    [Header("Debug")]
    public bool debugShowPowerupsOnly = false;
    public static LevelLayout instance;

    internal GridLevel levelGrid = new GridLevel();
    internal UnityEvent allCleared = new UnityEvent();
    internal GridInt layoutGrid = new GridInt();
    internal List<LevelTile> levelCells = new List<LevelTile>();
    internal int cellCount { get { return m_cellCount; } }
    internal int maxPersistence { get { return m_maxPersistence; } }
    internal Vector3 WorldOffset {  get { return m_worldOffset; } }

    Vector3 m_worldOffset;
    int m_maxPersistence;
    int m_cellCount;

    public void Awake()
    {
        instance = this;

        InitLevelGrid();
    }

    void InitLevelGrid()
    {
        layoutGrid.Size(columnLength, rowLength);

        levelGrid.Size(columnLength, rowLength);

        m_maxPersistence = levelGrid.totalSize;

        m_worldOffset = levelGrid.CalculateCentre() * -1;

        for (int x = 0; x < rowLength; x++)
        {
            for (int y = 0; y < columnLength; y++)
            {
                LevelTile _levelTile = Instantiate(levelTilePrefab);
                _levelTile.x = x;
                _levelTile.y = y;
                _levelTile.available = true;
                levelCells.Add(_levelTile);
                levelGrid[x, y] = _levelTile;

                GameObject _go = _levelTile.gameObject;
                _go.transform.position = GridPosition(_levelTile);
                _go.name = "Tile_" + (_levelTile.x.ToString() + _levelTile.y.ToString()).ToUpper();
                _go.transform.SetParent(transform);
            }
        }
        m_cellCount = levelCells.Count;
    }

    internal List<LevelTile> GetPersistentTiles()
    {
        List<LevelTile> _tiles = new List<LevelTile>();
        for (int i = 0; i < levelCells.Count; i++)
        {
            if (levelCells[i].meshTile != null && levelCells[i].meshTile.persistent)
                _tiles.Add(levelCells[i]);
        }
        return _tiles;
    }

    internal int PersistentTileCount()
    {
        int _count = 0;
        for (int i = 0; i < levelCells.Count; i++)
        {
            if (levelCells[i].meshTile != null && levelCells[i].meshTile.persistent)
                _count++;
        }
        return _count;
    }

    internal bool IsAllTilesPersistent()
    {
        for (int i = 0; i < levelCells.Count; i++)
        {
            if (levelCells[i].meshTile != null && !levelCells[i].meshTile.persistent)
                return false;
        }
        return true;
    }

    internal LevelTile GetLevelCell(int x, int y)
    {
        if (x < 0 || x >= levelGrid.rowLength)
            return null;

        if (y < 0 || y >= levelGrid.columnLength)
            return null;

        LevelTile _levelTile = levelGrid[x, y];

        return _levelTile;
    }

    internal LevelTile GetRandomAvailableTile()
    {
        List<LevelTile> _list = new List<LevelTile>();

        foreach (LevelTile _cell in levelCells)
        {
            if (_cell.meshTile != null && !_cell.meshTile.persistent && _cell.meshTile.currentPowerUp == null)
                _list.Add(_cell);
        }

        if (_list.Count == 0)
            return null;

        LevelTile _levelTile = _list[UnityEngine.Random.Range(0, _list.Count - 1)];
        _levelTile.available = false;
        return _levelTile;
    }

    internal List<LevelTile> GetRadial(LevelTile tile)
    {
        List<LevelTile> _list = new List<LevelTile>();

        //top
        LevelTile t = GetLevelCell(tile.x, tile.y + 1);
        if (t != null && t.available)
            _list.Add(t);
        //top right
        LevelTile tr = GetLevelCell(tile.x + 1, tile.y + 1);
        if (tr != null && tr.available)
            _list.Add(tr);
        //right
        LevelTile r = GetLevelCell(tile.x + 1, tile.y);
        if (r != null && r.available)
            _list.Add(r);
        //bottom right
        LevelTile br = GetLevelCell(tile.x + 1, tile.y - 1);
        if (br != null && br.available)
            _list.Add(br);
        //bottom
        LevelTile b = GetLevelCell(tile.x, tile.y - 1);
        if (b != null && b.available)
            _list.Add(b);
        //bottom left
        LevelTile bl = GetLevelCell(tile.x - 1, tile.y - 1);
        if (bl != null && bl.available)
            _list.Add(bl);
        //left
        LevelTile l = GetLevelCell(tile.x - 1, tile.y);
        if (l != null && l.available)
            _list.Add(l);
        //left top
        LevelTile lt = GetLevelCell(tile.x - 1, tile.y + 1);
        if (lt != null && lt.available)
            _list.Add(lt);

        return _list;
    }

    internal List<LevelTile> GetAdjacentForPersistence(LevelTile tile)
    {
        List<LevelTile> _list = new List<LevelTile>();

        //top
        LevelTile t = GetLevelCell(tile.x, tile.y + 1);
        if (t != null && t.AvailableForPersistence())
            _list.Add(t);
        //right
        LevelTile r = GetLevelCell(tile.x + 1, tile.y);
        if (r != null && r.AvailableForPersistence())
            _list.Add(r);
        //bottom
        LevelTile b = GetLevelCell(tile.x, tile.y - 1);
        if (b != null && b.AvailableForPersistence())
            _list.Add(b);
        //left
        LevelTile l = GetLevelCell(tile.x - 1, tile.y);
        if (l != null && l.AvailableForPersistence())
            _list.Add(l);

        return _list;
    }

    internal LevelTile GetRandomSidling(LevelTile tile)
    {
        var _candidates = GetRadial(tile);
        int _count = _candidates.Count;

        if (_count == 0)
            return null;

        var _picked = Random.Range(0, _count - 1);

        return _candidates[_picked];
    }

    internal LevelTile GetRandomSidlingAdjacentForPersistence(LevelTile tile)
    {
        var _candidates = GetAdjacentForPersistence(tile);
        int _count = _candidates.Count;

        if (_count == 0)
            return null;

        var _picked = UnityEngine.Random.Range(0, _count - 1);

        return _candidates[_picked];
    }

    internal bool RoundOver()
    {
        foreach(LevelTile _tile in levelCells)
            if (_tile.meshTile == null || _tile.meshTile.persistent)
                return false;

        return true;
    }

    internal Vector3 GridPosition(LevelTile tile)
    {
        return GridPosition(tile.x, tile.y);
    }

    internal Vector3 GridPosition(int x, int y)
    {
        return m_worldOffset + new Vector3(x, y, 0);
    }
}
