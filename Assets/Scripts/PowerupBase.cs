using System;
using System.Collections.Generic;
using UnityEngine;

public enum PowerupType { Swap = 0, ChangeDirection = 1, ScoreRow = 2, ScoreColumn = 3, Blast = 4, Nuke = 5, Life = 6 };

[Serializable]
public class PowerupSlot
{
    public PowerupType type = PowerupType.Swap;
    public int comboOffset = 0;
}

[System.Serializable]
public class PowerupBase : MonoBehaviour
{
    public float zLocalOffset = -3.1f;

    internal MeshTile meshTile;

    public virtual void OnUse(List<MeshTile> chain, int score)
    {
    }

    public void Use(List<MeshTile> chain, int score)
    {
        OnUse(chain, score);
        meshTile.tile.available = true;
    }
}
