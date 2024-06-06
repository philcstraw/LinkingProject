using System.Collections.Generic;

public class NukePowerup : PowerupBase 
{
    public override void OnUse(List<MeshTile> chain, int score)
    {
        List<MeshTile> tiles = MeshTileManager.instance.GetAllType(meshTile.type);
        ScoreGrid.instance.ScoreChain(tiles);
        SoundBoard.instance.nukePowerupSound.Play();
    }
}
