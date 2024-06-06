using System.Collections.Generic;

public class ScoreRowPowerup : PowerupBase
{
    public bool scoreColumn = false;
    
    public override void OnUse(List<MeshTile> chain, int score)
    {
        MeshTile _meshTile = gameObject.GetComponentInParent<MeshTile>();

        if (_meshTile == null)
            return;
        
        LevelTile _tile = _meshTile.tile;
        List<LevelTile> _levelTiles;

        if (!scoreColumn)
            _levelTiles = MeshTileManager.instance.GetRow(_tile.y);
        else
            _levelTiles = MeshTileManager.instance.GetColumn(_tile.x);

        List<MeshTile> _tiles = new List<MeshTile>(_levelTiles.Count);
        foreach (LevelTile lt in _levelTiles)
            _tiles.Add(lt.meshTile);

        ScoreGrid.instance.ScoreChain(_tiles);

        SoundBoard.instance.rowPowerupSound.PlayOneShot();
    }
}
