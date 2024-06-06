using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameLogic : MonoBehaviour
{
    public PowerupSpawner swapTileSpawner;
    public GoogleAdmobManager adManager;
    public ScoreGrid scoreGrid;
    public MeshTileManager meshTileManager;
    public float gameOverDelay = 2f;
    public float gameOverTimeScale = 0.5f;
    public UnityEvent OnGameOver;
    public bool paused { get { return instance.m_paused; } } //used by other scripts
    public bool gameOver{ get { return m_gameOver; } }

    internal static GameLogic instance;

    bool m_paused = false;
    bool m_gameOver = false;
    
    void Awake()
    {
        instance = this;

        Time.timeScale = 1f;
    }

    void Start()
    {
        adManager.RequestBannerAd();
        adManager.RequestFullScreenAd();
    }
    
    public void PauseGame(bool state)
    {
        m_paused = state;
        Time.timeScale = m_paused ? 0f : 1f;
    }

    internal void GameOver()
    {
        m_gameOver = true;
    }
    
    void Update()
    {
        if (m_gameOver && !m_paused)
        {
            StartCoroutine(GameOverRoutine(gameOverDelay));
            m_paused = true;
        }
    }
    
    IEnumerator GameOverRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        SoundBoard.instance.gameOver.PlayOneShot();
        GameplayEffects.instance.DisplayGameMessage("Game Over");

        yield return new WaitForSeconds(1f);

        scoreGrid.CalculateScoreAndRating();

        yield return new WaitForSeconds(2f);

        m_gameOver = true;

        if (OnGameOver != null)
            OnGameOver.Invoke();
    }
}