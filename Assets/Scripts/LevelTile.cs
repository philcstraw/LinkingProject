using UnityEngine;

[System.Serializable]
public class LevelTile : MonoBehaviour
{
    public int x;
    public int y;
    public bool fixedIfPersistent = true;
    public PersistentTilePowerup persistentTilePowerupPrefab;
    [HideInInspector] public MeshTile meshTile;

    internal TileType type;
    internal int persistentCount = 10;
    internal int score;
    internal int accummilatedScore;
    internal bool available;
    internal bool selected;

    internal void SetAndDisplayScore(int newScore,bool showScore)
    {
        if(showScore)
            GameplayEffects.instance.DisplayPopupScore(x, y, newScore.ToString(), 0.075f);

        score = newScore;
    }

    internal void SetAndDisplayScoreScaleUpEffect(int newScore, int displayScore, float startScale, float scaleSpeed)
    {
        float _incrementOffset = 0.1f;

        GameplayEffects.instance.DisplayPersistantCount(x, y,
            displayScore.ToString(), 
            startScale, 
            scaleSpeed, 
            _incrementOffset, 1f);

        score = newScore;
    }

    internal void DeletePowerup()
    {
        if (meshTile.currentPowerUp != null)
            Utility.SafeDestroy(meshTile.currentPowerUp.gameObject);
        
        available = true;
    }

    internal void Delete(float delay = 0f)
    {
        if (meshTile.currentPowerUp != null)
            Destroy(meshTile.currentPowerUp.gameObject, delay);

        if (meshTile != null)
            Destroy(meshTile.gameObject, delay);

        available = true;
    }

    internal bool AvailableForPersistence()
    {
        if (meshTile != null && meshTile.persistantPowerup == null)
            return true;

        return false;
    }

    internal PersistentTilePowerup MakePersistent(int persistence)
    {
        if (meshTile == null)
            return null;

        PersistentTilePowerup _persistentTilePU = Instantiate(persistentTilePowerupPrefab);

        DeletePowerup();
        meshTile.AddPowerup(_persistentTilePU);
        meshTile.persistent = true;
        meshTile.persistantPowerup = _persistentTilePU;
        meshTile.persistantPowerup.SetCount(persistence);
        meshTile.persistantPowerup.randomizeOnScore = true;
        meshTile.persistantPowerup.requireComboCount = Game.instance.mode.requireCombo;
        _persistentTilePU.gameObject.name = "Persistent_" + name;

        return _persistentTilePU;
    }

    internal void MakeLifeTile()
    {
        meshTile.addLife = true;
        meshTile.persistantPowerup.addLifeImage.enabled = true;
    }
}