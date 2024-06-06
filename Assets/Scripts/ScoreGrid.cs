using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

class ScoreRoutine
{
    public bool isRunning = false;
    public bool isFinished = false;
}

public class ScoreGrid : MonoBehaviour 
{
    public Game game;
    public LevelLayout levelLayout;
    public MeshTileManager meshTileManager;
    public PowerupManager powerupManager;
    public PersistenceManager persistenceManager;
    public ScoreTileFXControl scoreTileFXControl;
    public SoundBoard soundBoard;
    public AnimationCurve scaleCurve;
    public GameObject glowCam;
    public float scoreScaleOffset = 1.2f;
    public float scoreScaleSpeed = 1f;
    public float scoreChainDelay = 1f;
    public float rateOfGlow = 0.01f;
    public float glowLimit = 0.4f;// may not need this
    public bool chipTileOnScore = true;
    public bool addPowerupByCombo = true;
    public bool scoreParticles = true;

    internal ScoreRoutine mostRecentScoreRoutine;
    internal UnityEvent OnDeleteChain = new UnityEvent();
    internal static ScoreGrid instance;

    List<ScoreRoutine> m_scoreRoutines = new List<ScoreRoutine>();
    Vector3 m_scoreScaleOffsetVec;
    bool m_doGlow = false;
    int m_score;
    int m_glowreg = 0;

    void Awake()
    {
        instance = this;
    }

    void Start () 
    {
        InitializeScoreGrid();

        if(glowCam)
            glowCam.SetActive(false);

        m_scoreScaleOffsetVec = new Vector3(scoreScaleOffset, scoreScaleOffset, scoreScaleOffset);
    }

    internal void ScoreChain(List<MeshTile> chain, bool playSound = true)
    {
        int _score = chain.Count;
        List<MeshTile> _tileChain = new List<MeshTile>(chain.Count);

        foreach (MeshTile _tile in chain)
        {
            if (_tile == null)
                continue;

            _tileChain.Add(_tile);
        }

        // begin glowing before deleting the chain
        GlowScoredTiles(_tileChain);

        ScoreRoutine _newRoutine = new ScoreRoutine();
        mostRecentScoreRoutine = _newRoutine;

        StartCoroutine(DeleteChainRoutine(_newRoutine, _tileChain, _score, scoreChainDelay, playSound));
    }

    internal bool IsRunningScoreRoutine()
    {
        foreach (var r in m_scoreRoutines)
        {
            if (r.isRunning)
                return true;
        }
        return false;
    }

    internal bool IsFinishedRunningRoutines()
    {
        foreach (var r in m_scoreRoutines)
        {
            if (!r.isFinished)
                return false;
        }
        return true;
    }

    Vector3 GetScoreScale()
    {
        return meshTileManager.baseMeshTile.transform.localScale + m_scoreScaleOffsetVec;
    }
    
    void Update()
    {
        // Turn on or off the glow cam if we want glow. Slight optimisation.
        if (!m_doGlow && glowCam && glowCam.activeSelf)
            glowCam.SetActive(false);

        if (m_doGlow &&  glowCam && !glowCam.activeSelf)
            glowCam.SetActive(true);

        if (m_glowreg < 0)
            m_glowreg = 0;

        if (m_glowreg > 0)
            m_doGlow = true;
        else
            m_doGlow = false;
    }

    void InitializeScoreGrid()
    {
        foreach (LevelTile _levelTile in levelLayout.levelCells)
            _levelTile.score = 0;

        m_score = 0;
    }
    
    void GlowScoredTiles(List<MeshTile> chain)
    {
        int _score = chain.Count;

        soundBoard.glowSound.PlayOneShot();

        m_glowreg += 1;

        foreach (MeshTile _meshTile in chain)
        {
            if (_meshTile == null)
                continue;

            if(!_meshTile.persistent)
                _meshTile.delete = true;

            // reset any scaling before applying our own, eg if the tile has just spawned in
            _meshTile.CancelScaling();
            _meshTile.ScaleTo(GetScoreScale(), scoreScaleSpeed, scaleCurve);

            _meshTile.tileGlow.DoGlowRoutine(rateOfGlow, glowLimit);
        }
    }

    IEnumerator DeleteChainRoutine(ScoreRoutine routine, List<MeshTile> chain, int score, float delay,bool playSound)
    {
        routine.isRunning = true;

        yield return new WaitForSeconds(delay);

        DeleteChain(chain, score, playSound);

        routine.isRunning = false;
        routine.isFinished = true;

        if (mostRecentScoreRoutine == routine)
            mostRecentScoreRoutine = null;
    }
    
    void DeleteChain(List<MeshTile> chain, int score, bool playSound)
    {
        if(playSound)
            soundBoard.scoreSound.Play();
        
        foreach (MeshTile _meshTile in chain)
        {
            if (_meshTile == null)
                continue;

            // play score fx
            // may want a different effect for persistant tiles
            if (scoreParticles)
                scoreTileFXControl.Play(_meshTile.transform.position, _meshTile.type);

            if (_meshTile.persistantPowerup)
            {
                _meshTile.persistantPowerup.Use(chain, score);
                _meshTile.CancelScaling();
                _meshTile.ScaleTo(meshTileManager.defaultMeshTileScale, scoreScaleSpeed, scaleCurve);
                _meshTile.tileGlow.StopGlowRoutine(rateOfGlow);

                if(_meshTile.persistantPowerup.randomizeOnScore)
                    _meshTile.NextRandomType();

                if(_meshTile.persistantPowerup.persistenceCount > 0)
                    continue;
            }else
            {
                _meshTile.tile.SetAndDisplayScore(1, false);

                // use the powerup.
                if (_meshTile.currentPowerUp != null)
                    _meshTile.currentPowerUp.Use(chain,score);
            }
            _meshTile.Destory();
        }
        
        chain.Clear();
        
        if(chipTileOnScore)
            meshTileManager.ChipTile();

        if (addPowerupByCombo)
            powerupManager.SpawnByCombo(score);

        CalculateScoreAndRating();

        OnDeleteChain.Invoke();

        if (game.mode.spreadOnScore)
            persistenceManager.SpreadPersistenceRoutine();

        CheckRoundOver();
    }

    void CheckRoundOver()
    {
        if (levelLayout.RoundOver())
        {
            game.mode.LevelUp();
            persistenceManager.AddPersistentTiles();
            soundBoard.allPersistanceClearedSound.PlayOneShot();
            meshTileManager.ShiftAllTilesRoutine();
            levelLayout.allCleared.Invoke();
        }
    }

    public void CalculateScoreAndRating()
    {
        foreach (LevelTile _levelTile in levelLayout.levelCells)
        {
            if (_levelTile.score > 0)
                m_score += _levelTile.score;

            _levelTile.score = 0;
        }
        game.mode.SetScore(m_score);
    }
}
