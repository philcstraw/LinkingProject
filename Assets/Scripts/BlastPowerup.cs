using System.Collections.Generic;

public class BlastPowerup : PowerupBase 
{
    public override void OnUse(List<MeshTile> chain, int score)
    {
        List<MeshTile> tiles = MeshTileManager.instance.GetRadial(meshTile.tile);
        ScoreGrid.instance.ScoreChain(tiles);
        SoundBoard.instance.nukePowerupSound.Play();
    }
}
