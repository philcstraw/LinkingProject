using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour 
{
    [Header("Settings")]
    public bool limitMoves = true;
    public bool carryOverMovesLeft = true;
    public bool addMovesByRank = true;
    public bool addBaseNumMovesOnRankUp = false;
    public int numberOfMoves = 10;
    public bool useLifePowerUp = false;
    public bool setMovesByBracket = false;
    public bool requireCombo = true;
    public bool spreadOnScore = false;
    public bool spreadExponential = false;
    public bool distributePersistence = false;
    public string gameOverMessage = "Game Over";
    public string saveIdentifier = "Contagion";
    public bool resetLevelOnStart = false;
    public bool allowLevelSelect = false;
    public bool countDown = false;
    public int baseSecondsPerRound = 60;

    [Header("Difficulty Scaling")]
    public int basePersistence = 0;
    public int startLevel = 1;
    public int numberOfLevels = 100;

    [SerializeField]
    public List<PowerupSlot> powerups = new List<PowerupSlot>();

    [Header("Debug")]
    public int debugStartLevel = -1;
    public int currentBracket;
    [SerializeField]
    public List<int> rounds = new List<int>();
    public bool generate = false;

    const int m_minPersistence = 1;

    string m_currentLevelSettingString;
    string m_highestLevelSettingString;

    string m_highestScoreSettingString;

    int m_currentLevel = m_minPersistence;
    int m_highestLevel = m_minPersistence;

    int m_currentScore = 0;
    int m_highestScore = 0;

    int m_movesLeft = 5;
    int m_countDownTimer = 0;
    float m_seconds = 0f;
    bool m_newLevel = false;
    int m_previousLevel = 0;

    public void Init()
    {
        TileChainer.instance.onChainedTiles.AddListener(DecreaseNumberOfMoves);

        m_movesLeft = numberOfMoves;

        m_currentLevel = startLevel;

        m_highestLevel = m_currentLevel;

        GenerateRounds();

        m_currentLevelSettingString = saveIdentifier + "_" + "CurrentLevel";

        m_highestLevelSettingString = saveIdentifier + "_" + "HighestLevel";

        m_currentLevelSettingString = saveIdentifier + "_" + "CurrentScore";

        m_highestLevelSettingString = saveIdentifier + "_" + "HighestScore";

        LoadLastLevel();

        if (debugStartLevel > -1)
        {
            m_currentLevel = Mathf.Clamp(debugStartLevel, m_minPersistence, numberOfLevels);
        }

        currentBracket = rounds[CurrentIndex()];

        ResetCountDownTimer();

        if (setMovesByBracket)
            LevelUpMoves();
    }


    void GenerateRounds()
    {
        rounds.Clear();
        for (int i = startLevel; i < numberOfLevels; i++)
        {
            int _round;
            int _persistLevel = i;
            int _persistence = basePersistence + _persistLevel;
            if (_persistence < m_minPersistence)
                _persistence = m_minPersistence;
            _round = _persistence;
            rounds.Add(_round);
        }
    }

    void OnValidate()
    {
        if (generate)
        {
            generate = false;
            GenerateRounds();
        }

        if (startLevel < 1)
            startLevel = 1;
    }


    public void SaveCurrentLevel()
    {
        Utility.SetPlayerSetting(m_currentLevelSettingString, CurrentLevel());
    }

    public void SaveHighestLevel()
    {
        Utility.SetPlayerSetting(m_highestLevelSettingString, m_highestLevel);
    }

    public void SaveHighestScore()
    {
        Utility.SetPlayerSetting(m_highestScoreSettingString, HighScore());
    }

    public void LoadLastLevel()
    {
        m_highestLevel = Utility.GetPlayerSettingInt(m_highestLevelSettingString, startLevel);

        if (!resetLevelOnStart)
            m_currentLevel = Utility.GetPlayerSettingInt(m_currentLevelSettingString, startLevel);

        m_highestScore = Utility.GetPlayerSettingInt(m_highestScoreSettingString, 0);
    }

    public void ResetLevel()
    {
        Utility.SetPlayerSetting(m_currentLevelSettingString, startLevel);
        Utility.SetPlayerSetting(m_highestLevelSettingString, startLevel);
        Utility.SetPlayerSetting(m_highestScoreSettingString, 0);
    }

    public int HighScore()
    {
        return m_highestScore;
    }

    public int CurrentScore()
    {
        return m_currentScore;
    }

    void DecreaseNumberOfMoves()
    {
        if (limitMoves)
        {
            m_movesLeft--;
            if (m_movesLeft <= 0)
                m_movesLeft = 0;

            // TODO: make this coroutine run in this monobahviour after we make GameModeConfigs into monobehaviours
            if (_checkPersistenceRoutine != null)
                ScoreGrid.instance.StopCoroutine(_checkPersistenceRoutine);

            ScoreGrid.instance.StartCoroutine(_checkPersistenceRoutine = CheckPersistence());
        }
    }


    IEnumerator _checkPersistenceRoutine;
    IEnumerator CheckPersistence()
    {
        while (ScoreGrid.instance.mostRecentScoreRoutine != null && ScoreGrid.instance.mostRecentScoreRoutine.isRunning)
            yield return null;

        if (m_movesLeft <= 0 && m_previousLevel == m_currentLevel)
            GameLogic.instance.GameOver();

        m_previousLevel = m_currentLevel;
    }

    void ResetCountDownTimer()
    {
        m_countDownTimer = baseSecondsPerRound;
        m_seconds = 0f;
    }

    public void CountDownTimer()
    {
        m_seconds += Time.deltaTime;
        if (m_seconds >= 1f)
        {
            m_seconds = 0f;
            m_countDownTimer--;
            if (m_countDownTimer <= 0)
            {
                m_countDownTimer = 0;

                GameLogic.instance.GameOver();
            }
        }
    }

    public int TimeLeft()
    {
        return m_countDownTimer;
    }

    public int CurrentLevel()
    {
        return m_currentLevel;
    }

    public int HightestLevel()
    {
        return m_highestLevel;
    }

    public void SetLevel(int level)
    {
        m_currentLevel = Mathf.Clamp(level, startLevel, numberOfLevels);
    }

    public void SetScore(int score)
    {
        m_currentScore = score;

        if (m_currentScore > m_highestScore)
        {
            m_highestScore = m_currentScore;

            SaveHighestScore();
        }
    }

    public int MinLevel()
    {
        return m_minPersistence;
    }

    public int MovesLeft()
    {
        return m_movesLeft;
    }

    internal void AddMoves(int numMoves)
    {
        if (useLifePowerUp)
            m_movesLeft += numMoves;
    }

    void LevelUpMoves()
    {
        if (!setMovesByBracket)
        {
            m_movesLeft = (addBaseNumMovesOnRankUp ? numberOfMoves : 0)
                + (carryOverMovesLeft ? m_movesLeft : 0)
                + (this.addMovesByRank ? m_currentLevel - 1 : 0);
        }
        else
        {
            // Levels start at 1, so we have to -1 before dividing then adding one back to make our bracket go from 1 - 10
            int _bracket = ((m_currentLevel - 1) / 10) + 1;
            m_movesLeft = _bracket * numberOfMoves;
        }
    }

    int CurrentIndex()
    {
        return m_currentLevel - startLevel;
    }

    public bool HasNewLevel()
    {
        return m_newLevel;
    }

    public void LevelUp()
    {
        m_currentLevel = Mathf.Min(m_currentLevel + 1, numberOfLevels);

        if (m_currentLevel > m_highestLevel)
        {
            m_highestLevel = m_currentLevel;

            SaveHighestLevel();

            GameplayEffects.instance.DisplayNewLevel();

            m_newLevel = true;
        }

        if (limitMoves)
        {
            LevelUpMoves();
        }

        if (countDown)
            ResetCountDownTimer();

        if (CurrentIndex() < 0)
            Debug.LogError("Index is less than 0");

        if (CurrentIndex() >= rounds.Count)
            Debug.LogError("Index is greater then bracket count");

        currentBracket = rounds[CurrentIndex()];

        SaveCurrentLevel();
    }
}
