using UnityEngine;

public enum GameModeType { None = 0, Survival = 1, Containment = 2, CountDown = 3, Debug = 100 }

public class Game : MonoBehaviour
{
	public GameModeType gameMode = GameModeType.Survival;
	public GameMode limitMoves;
	public GameMode containment;
	public GameMode countDown;
	public GameMode debug;
	[ReadOnly] public GameMode mode;

	public static Game instance;

	void Awake()
    {
		instance = this;

		if (LevelManager.instance.gameMode != GameModeType.None)
			gameMode = LevelManager.instance.gameMode;

		switch (gameMode)
		{
			case GameModeType.Survival:
				mode = limitMoves;
				break;
			case GameModeType.Containment:
				mode = containment;
				break;
			case GameModeType.Debug:
				mode = debug;
				break;
			case GameModeType.CountDown:
				mode = countDown;
				break;
			default:
				break;
		}
	}

	void Start()
    {
		mode.Init();
	}

	void Update()
    {
		if(gameMode == GameModeType.CountDown)
        {
			mode.CountDownTimer();
        }
    }

	public void ResetGameModeProgress()
    {
		mode.ResetLevel();
    }
}
