using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    public Game game;
    public MeshTileManager meshTileManager;
    public PowerupSpawner swapPowerUpSpawner;
    public List<PowerupSpawner> spawners = new List<PowerupSpawner>();

    internal static PowerupManager instance;

    List<PowerupSpawner> m_allowedPowerups = new List<PowerupSpawner>();
    Dictionary<PowerupType, int> m_map = new Dictionary<PowerupType, int>();

    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
        PickPowerups();
    }

    void Update()
    {
        if (!meshTileManager.HasValidMove())
            swapPowerUpSpawner.SpawnPowerup(1);
    }

    internal PowerupSpawner SpawnerFromType(PowerupType type)
    {
        return spawners[m_map[type]];
    }

    internal void SpawnByCombo(int chainCount)
    {
        int _count = m_allowedPowerups.Count;
        for (int i = 0; i < _count; i++)
        {
            var _spawner = m_allowedPowerups[i];
            if (_spawner.comboOffset <= chainCount)
                _spawner.SpawnPowerup(1);
        }
    }

    void PickPowerups()
    {
        for (int i = 0; i < spawners.Count; i++)
            m_map.Add(spawners[i].type, i);

        for (int i = 0; i < game.mode.powerups.Count; i++)
        {
            var _entry = game.mode.powerups[i];
            var _spawner = SpawnerFromType(_entry.type);
            _spawner.comboOffset = _entry.comboOffset;
            m_allowedPowerups.Add(_spawner);
        }
    }
}
