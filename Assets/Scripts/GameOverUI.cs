using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour 
{
    public Game game;
    public GameLogic gameplayLogic;
    public TextMeshProUGUI newHighScoreTextMeshProUGUI;
    public TextMeshProUGUI highestScoreTextMeshProUGUI;
    public TextMeshProUGUI finalScoreTextMeshProUGUI;
    public TextMeshProUGUI successTextMeshProUGUI;
    public TextMeshProUGUI timeLeftTextMeshProUGUI;
    public TextMeshProUGUI levelTextMeshProUGUI;
    public TextMeshProUGUI timeLeftLabel;
    public Text nextButton;
    public MenuVisibility endMenu;

    public string newHighScoreString = "New High Score!";
    public string gameOverString = "Game Over";
    public string allClearedString = "All Cleared";
    public bool debugGameOver = false;

    internal GameOverUI instance;

    void Awake()
    {
        instance = this;
    }
    
	void Update () 
    {
        if (!gameplayLogic.gameOver)
            return;

        highestScoreTextMeshProUGUI.text = game.mode.HightestLevel().ToString();
        levelTextMeshProUGUI.text = game.mode.CurrentLevel().ToString();
        finalScoreTextMeshProUGUI.text = game.mode.CurrentScore().ToString();
       
        if(!gameplayLogic.meshTileManager.levelLayout.RoundOver())
        {
            if (newHighScoreTextMeshProUGUI != null)
                newHighScoreTextMeshProUGUI.text = game.mode.gameOverMessage;
            return;
        }

        successTextMeshProUGUI.text = allClearedString;
        string nextButtonString = "Next Level";

        if (newHighScoreTextMeshProUGUI != null)
            newHighScoreTextMeshProUGUI.text = gameOverString;

        if (nextButton != null)
            nextButton.text = nextButtonString;
	}
}
