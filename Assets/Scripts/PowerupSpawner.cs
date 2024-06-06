using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour 
{
    public LevelLayout levelLayout;
    public PowerupBase powerUpPrefab;
    public PowerupType type = PowerupType.Swap;
    public int numberOnGrid = 1;
    public bool getRandomAdjacent = false;

    internal int comboOffset = 0;

    List<PowerupBase> m_powerups = new List<PowerupBase>();

    void Update() 
    {
        CleanList();
	}

    internal bool HasPowerupOnGrid()
    {
        return m_powerups.Count > 0;
    }

    internal void SpawnPowerup(int count)
    {
        for (int i = 0; i < count; i++)
            StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnPowerupRoutine()
    {
        PowerupBase _powerUp = null;
        while(_powerUp == null && m_powerups.Count < numberOnGrid)
        {
            _powerUp = CreateNew(powerUpPrefab);

            if (_powerUp != null)
            {
                m_powerups.Add(_powerUp);
                break;
            }
            yield return null;
        }
    }

    PowerupBase CreateNew(PowerupBase powerupPrefab)
    {
        LevelTile _levelTile = levelLayout.GetRandomAvailableTile();

        if (_levelTile == null)
            return null;

        MeshTile _meshTile = _levelTile.meshTile;

        if (_meshTile == null || _meshTile.delete)
            return null;

        PowerupBase _powerupBase = Instantiate(powerupPrefab);
        _meshTile.AddPowerup(_powerupBase);

        return _powerupBase;
    }

    void CleanList()
    {
        foreach(PowerupBase _powerUp in m_powerups)
        {
            if(_powerUp == null)
            {
                m_powerups.Remove(_powerUp);
                return;
            }
        }
    }
}
