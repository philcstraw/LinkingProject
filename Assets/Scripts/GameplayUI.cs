using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Used to update in-game GUI
public class GameplayUI : MonoBehaviour 
{
    public Game game;
    public GameLogic gameplayLogic;
    public MeshTileManager meshTileManager;
    public TextMeshPro gameModeTitleText;
    public TextMeshPro gameModeProgressText;
    public TextMeshPro highScoreText;
    public TextMeshPro currentScoreText;
    public TextMeshPro highestLevelText;
    public TextMeshPro currentLevelText;
    public TextMeshPro debugText;
    public TextMeshPro gameModeLabel;
    public Image directionImage;
    public TextMeshPro debugLastComboText;
    
    [Header("Pause Menu")]
    public TextMeshProUGUI gameModePauseMenuText;
    public TextMeshProUGUI highestLevelPauseMenuText;
    public TextMeshProUGUI levelPauseMenuText;
    public TextMeshProUGUI highestScorePauseMenuText;
    public TextMeshProUGUI scorePauseMenuText;

    internal static GameplayUI instance;

    CollapseDirection m_direction;
    IEnumerator m_rotateEnumerator = null;
    Color m_movesLeftTextColor;
    Color m_movesLeftProgressTextColor;
    int m_prevMovesLeft = -1;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        m_movesLeftTextColor = gameModeLabel.color;
        m_movesLeftProgressTextColor = gameModeProgressText.color;
    }

    void Update()
    {
        UpdateGameModeUI();

        UpdatePauseMenu();

        if (gameModeTitleText != null && game != null)
            gameModeTitleText.text = game.gameMode.ToString();

        if (directionImage != null)
        {
            if (m_direction != meshTileManager.collapseDirection)
            {
                m_direction = meshTileManager.collapseDirection;

                switch (meshTileManager.collapseDirection)
                {
                    case CollapseDirection.Down:
                        Rotate(90f);
                        break;
                    case CollapseDirection.Up:
                        Rotate(-90f);
                        break;
                    case CollapseDirection.Left:
                        Rotate(0f);
                        break;
                    case CollapseDirection.Right:
                        Rotate(180f);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void NumberOfMovesChangedCallBack()
    {
        StartCoroutine(ScaleArounFontCentre());
    }

    IEnumerator ScaleArounFontCentre()
    {
        yield return new WaitForSeconds(0.5f);

        int _movesLeft = Game.instance.mode.MovesLeft();
        TextMeshPro _newTextMesh = Instantiate(gameModeProgressText);
        _newTextMesh.text = _movesLeft.ToString();

        var _newRect = _newTextMesh.GetComponent<RectTransform>();

        float t = 0f;
        float _duration = 1f;
        float _startScale = 3f;

        _newRect.SetParent(gameModeProgressText.transform.parent, false);

        while (t < _duration)
        {
            t += Time.deltaTime;
            float l = t / _duration;
            float _ease = Utility.EaseOut(l);
            float _ease2 = _ease * _ease;
            float s = Mathf.Lerp(_startScale, 1f, _ease2);
            _newRect.localScale = Vector3.one * s;
            _newTextMesh.alpha = _ease2;
            yield return null;
        }

        Destroy(_newRect.gameObject);
    }

    IEnumerator MovesLeftNearGameOver(int movesLeft)
    {
        Color _startCol = m_movesLeftTextColor;
        Color _progressCol = m_movesLeftProgressTextColor;
        Color _targetColor = Color.red;
        float t = 0f;
        float _warningDuration = 0.5f;

        while (t < _warningDuration)
        {
            t += Time.deltaTime;
            float l = t / _warningDuration;
            gameModeLabel.color = Color.Lerp(_startCol, _targetColor, movesLeft > 0f ? Mathf.PingPong(l * 2f, 1f) : l);
            gameModeProgressText.color = Color.Lerp(_progressCol, _targetColor, movesLeft > 0f ? Mathf.PingPong(l * 2f, 1f) : l);
            yield return null;
        }
        gameModeLabel.color = movesLeft > 0f ? _startCol : _targetColor;
        gameModeProgressText.color = movesLeft > 0f ? _progressCol : _targetColor;
    }

    void UpdateGameModeUI()
    {
        if (game.mode.limitMoves)
        {
            int _movesLeft = game.mode.MovesLeft();

            if(m_prevMovesLeft != _movesLeft)
            {
                if (_movesLeft < 5)
                {
                    StartCoroutine(MovesLeftNearGameOver(_movesLeft));
                }else if(gameModeLabel.color != m_movesLeftTextColor)
                {
                    gameModeLabel.color = m_movesLeftTextColor;

                    gameModeProgressText.color = m_movesLeftTextColor;
                }
                m_prevMovesLeft = _movesLeft;
            }

            string _movesleftString = _movesLeft.ToString();

            if (highestLevelPauseMenuText != null)
                highestLevelPauseMenuText.text = _movesleftString;

            if (gameModeLabel != null)
                gameModeLabel.text = "Moves Left ";

            if(gameModeProgressText != null)
                gameModeProgressText.text = _movesleftString;

        }
        else if(game.mode.countDown)
        {
            string _timeLeft = game.mode.TimeLeft().ToString();

            if (highestLevelPauseMenuText != null)
                highestLevelPauseMenuText.text = _timeLeft;

            if (gameModeLabel != null)
                gameModeLabel.text = string.Concat("Time Left ", _timeLeft);
        }
        else
        {
            if (gameModeLabel != null)
            {
                gameModeLabel.enabled = false;
                gameModeProgressText.enabled = false;
            }
        }

        int _score = game.mode.CurrentScore();
        int _highScore = game.mode.HighScore();

        if (currentScoreText != null)
            currentScoreText.text = string.Concat("Score  ", _score.ToString());

        if (highScoreText != null)
            highScoreText.text = _score == _highScore ? "New High Score!" : string.Concat("High Score  ", _highScore.ToString());

        if(highestLevelText != null)
            highestLevelText.text = string.Concat("Highest Level ", game.mode.HightestLevel().ToString());

        if (currentLevelText != null && currentLevelText.text != "null")
            currentLevelText.text = string.Concat("Level  ", game.mode.CurrentLevel().ToString());
    }

    void UpdatePauseMenu()
    {
        if (gameModePauseMenuText)
            gameModePauseMenuText.text = game.gameMode.ToString();

        if (highestLevelPauseMenuText)
            highestLevelPauseMenuText.text = string.Concat("Highest Level", "  ", game.mode.HightestLevel().ToString());

        if (levelPauseMenuText)
            levelPauseMenuText.text = string.Concat("Level", "  ", game.mode.CurrentLevel().ToString());

        int _score = game.mode.CurrentScore();
        int _highScore = game.mode.HighScore();

        if (scorePauseMenuText != null)
            scorePauseMenuText.text = string.Concat("Score  ", _score.ToString());

        if (highestScorePauseMenuText != null)
            highestScorePauseMenuText.text = _score == _highScore ? "New High Score!" : string.Concat("High Score  ", _highScore.ToString());
    }

    void Rotate(float angle)
    {
        if (m_rotateEnumerator != null)
            StopCoroutine(m_rotateEnumerator);

        StartCoroutine(m_rotateEnumerator = RotateRoutine(angle));
    }

    IEnumerator RotateRoutine(float angle)
    {
        float t = 0f;
        float _duration = 0.5f;
        Vector3 _start = directionImage.transform.eulerAngles;
        Vector3 _eulerAngles = new Vector3(0f, 0f, angle);
        Vector3 _startScale = directionImage.transform.localScale;
        Vector3 _targetScale = _startScale * 1.5f;

        while(t < _duration)
        {
            t += Time.deltaTime;
            float l = t / _duration;
            float d = l * l * l * (l * (6f * l - 15f) + 10f);
            directionImage.transform.eulerAngles = Vector3.Lerp(_start, _eulerAngles, d);
            directionImage.transform.localScale = Vector3.Lerp(_startScale, _targetScale, Mathf.PingPong(l * 2f, 1f));
            yield return null;
        }
        directionImage.transform.eulerAngles = _eulerAngles;
        directionImage.transform.localScale = _startScale;
    }
}
