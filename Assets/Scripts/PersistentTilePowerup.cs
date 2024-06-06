using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

// powerup used to track persistant tiles
// using a powerup prevents other spawned power ups from taking the tile
public class PersistentTilePowerup : PowerupBase
{
    public TextMeshProUGUI textMeshProuGUI;
    public Image addLifeImage;
    public int persistenceCount = 10;
    public bool requireComboCount = true;
    public bool randomizeOnScore = true;
    int _initialCount = -1;

	// using the powerup updates the persistant state
    public override void OnUse(List<MeshTile> chain, int score)
    {
        if (requireComboCount)
        {
            if (chain.Count >= persistenceCount)
                ReducePersistence(score);
            else
                SoundBoard.instance.persistentScoreFail.PlayOneShot();
        }
        else
            ReducePersistence(score);
    }

    internal void ReducePersistence(int score)
    {
        SoundBoard.instance.depletePersistancePowerupSound.Play();

        ReduceCount(score);
    }

    internal void ReduceCount(int score)
    {
        int _newScore = persistenceCount - score;

        SetCount(_newScore);
    }

    internal void SetCount(int num)
    {
        int _previousCount = persistenceCount;
        persistenceCount = num;
        if (_initialCount < 0)
            _initialCount = num;

        textMeshProuGUI.text = persistenceCount.ToString();

        // when we've depleted the count, remove the persistant state by deleting this script and award the accummilated score (see ScoreGrid)
        if (persistenceCount <= 0)
        {
            meshTile.persistent = false;
            meshTile.tile.available = true;

            if (meshTile.addLife)
            {
                Game.instance.mode.AddMoves(_previousCount);
                meshTile.tile.SetAndDisplayScoreScaleUpEffect(_previousCount, _previousCount, 0.055f, 1.5f);
            }
            else
                meshTile.tile.SetAndDisplayScore(_previousCount, true);

            SoundBoard.instance.musicalSequence.PlaySequence();

            Destroy(gameObject);

            return;
        }
    }
}
