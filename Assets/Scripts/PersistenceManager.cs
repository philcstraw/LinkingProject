using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistenceManager : MonoBehaviour
{
    public LevelLayout levelLayout;
    public MeshTileManager meshTileManager;
    public SoundBoard soundBoard;
    public Game game;
    public GameLogic gameLogic;
    public ScoreGrid scoreGrid;
    public GameTimer startTimer;
    public GameTimer timer;
    public float zLocalOffset = -3.1f;
    public int comboOffset = 0;
    public int persistenceNumber = 1;
    public int persistenceStartLevel = 10;
    public int maxPersistentLevel = 10;
    public int maxTilePersistence = 20;

    internal List<PersistentTilePowerup> powerups = new List<PersistentTilePowerup>();
    internal static PersistenceManager instance;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        // TODO: Move to coroutine
        if (startTimer.timeUp)
            AddPersistentTiles();
    }

    internal void SpreadPersistenceRoutine()
    {
        if(soundBoard.spread != null)
            soundBoard.spread.PlayOneShot();

        int _numSidlings = 1;
        if(game.mode.spreadExponential)
            _numSidlings = levelLayout.PersistentTileCount();

        StartCoroutine(SpreadPowerupRoutine(_numSidlings, 1000));
    }

    internal void AddPersistentTiles()
    {
        if (!game.mode.distributePersistence)
        {
            int _persistLevel = game.mode.currentBracket;
            int _numSidlings = 1;
            int _maxIterations = 200;
            StartCoroutine(SeedRound(_persistLevel, _numSidlings,false, _maxIterations));
        }
        else
        {
            int _persistLevel = game.mode.currentBracket;
            bool _isEven = (_persistLevel % 2) == 0;

            if (!_isEven)
                _persistLevel += 1;

            List<int> _factors = new List<int>();
            _factors.Add(_persistLevel);
            int _factorLimit = _persistLevel / 2;
            for (int _possibleFactor = 2; _possibleFactor <= _factorLimit; _possibleFactor++)
            {
                if (_persistLevel % _possibleFactor == 0)
                {
                    if(_possibleFactor <= levelLayout.maxPersistence)
                        _factors.Add(_persistLevel / _possibleFactor);
                }
            }

            int _randomiseIndex = Random.Range(0, _factors.Count);
            int _persistFactor = _factors[_randomiseIndex];

            int _numSidlings = _persistLevel / _persistFactor;

            if (_randomiseIndex == 0 && !_isEven)
                _persistFactor = _persistLevel -1;

            int _maxIterations = 200;
            StartCoroutine(SeedRound(_persistFactor, _numSidlings, _isEven, _maxIterations));
        }
    }

    IEnumerator SeedRound(int persistence, int numSidlings,bool isEven, int maxIterations)
    {
        while (!meshTileManager.IsGridFilled())
            yield return null;

        LevelTile _levelTile = GetRandomAvailableCell();
        List<LevelTile> _list = new List<LevelTile>();

        _list.Add(_levelTile);
        LevelTile _next = _list[0];
        List<LevelTile> _subList = new List<LevelTile>();

        int _iter = 0;

        while (_list.Count < numSidlings && _iter < maxIterations)
        {
            _iter++;

            var _sidlings = levelLayout.GetAdjacentForPersistence(_next);
            _subList.Clear();

            for (int j = 0; j < _sidlings.Count; j++)
            {
                if(!_list.Contains(_sidlings[j]))
                    _subList.Add(_sidlings[j]);
            }

            if(_subList.Count > 0)
            {
                int _pick = Random.Range(0, _subList.Count-1);
                _list.Add(_subList[_pick]);
                _next = _subList[_pick];
            }
            else
            {
                _next = _list[Random.Range(0, _list.Count - 1)];
            }

            yield return null;
        }

        int _oddPick = (!isEven && numSidlings > 1) ? Random.Range(0, _list.Count) : -1;

        for (int i = 0; i < _list.Count; i++)
        {
            var _tile = _list[i];
            _tile.MakePersistent( (i == _oddPick) ? persistence - 1 : persistence);
        }

        if (game.mode.useLifePowerUp)
            AddLifeTile();
    }

    void AddLifeTile()
    {
        List<LevelTile> _tiles = levelLayout.GetPersistentTiles();
        int _pick = Random.Range(0, _tiles.Count);
        if (_pick >= _tiles.Count)
        {
            Debug.LogError("Pick " + _pick + " persistent tile count " + _tiles.Count);
            return;
        }

        LevelTile _tile = _tiles[_pick];
        _tile.MakeLifeTile();
    }

    IEnumerator SpreadPowerupRoutine(int numSidlings, int maxIterations)
    {
        while (!meshTileManager.IsGridFilled())
            yield return null;

        for (int i = 0; i < numSidlings; i++)
        {
            LevelTile _newTile = null;
            int _iter = 0;
            int _persistence = 0;

            while (_newTile == null && _iter < maxIterations)
            {
                List<LevelTile> _tiles = levelLayout.GetPersistentTiles();
                if (_tiles.Count == 0)
                    break;

                var _random = _tiles[Random.Range(0, _tiles.Count - 1)];

                _iter++;
                _newTile = levelLayout.GetRandomSidlingAdjacentForPersistence(_random);

                if (_newTile == null)
                    yield return null;
                else
                {
                    _persistence = _random.meshTile.persistantPowerup.persistenceCount;
                    break;
                }

                yield return null;
            }

            if (_newTile != null)
            {
                while (!_newTile.MakePersistent(_persistence))
                    yield return null;

                if(game.mode.useLifePowerUp)
                    AddLifeTile();

                if (levelLayout.IsAllTilesPersistent())
                    gameLogic.GameOver();
            }
            yield return null;
        }
    }

    LevelTile GetRandomAvailableCell()
    {
        List<LevelTile> _list = new List<LevelTile>();

        foreach (LevelTile _cell in levelLayout.levelCells)
        {
            if (_cell.meshTile != null && !_cell.meshTile.persistent && _cell.meshTile.currentPowerUp == null)
                _list.Add(_cell);
        }

        if (_list.Count == 0)
        {
            // TODO: just pick a random tile instead of adding to list
            foreach (LevelTile _cell in levelLayout.levelCells)
                _list.Add(_cell);
        }

        int _index = _list.Count - 1;
        LevelTile _selectedTile = _list[Random.Range(0, _index)];

        if (_selectedTile.meshTile == null)
            return null;

        if (_selectedTile.meshTile.currentPowerUp != null)
            Destroy(_selectedTile.meshTile.currentPowerUp.gameObject);

        _selectedTile.available = false;

        return _selectedTile;
    }
}
