using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    internal static SceneControl instance;

    void Awake()
    {
        instance = this;
    }

    public void LoadNextScene()
    {
        int _index = SceneManager.GetActiveScene().buildIndex + 1;

        if (_index >= SceneManager.sceneCountInBuildSettings)
            _index = SceneManager.sceneCountInBuildSettings - 1;

        SceneManager.LoadScene(_index);
    }

    public void LoadScene(int index)
    {
        //if (index >= SceneManager.sceneCountInBuildSettings)
        //    index = SceneManager.sceneCountInBuildSettings -1;

        SceneManager.LoadScene(index);
    }

    //return to the scene 0, assumed to be the main menu
    public void ReturnToRoot()
    {
        SceneManager.LoadScene(0);
    }

    //reload the current scene
    public void ReloadCurrentScene()
    {
        Scene _scene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(_scene.name);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadGameModeLimitMoves()
    {
        LevelManager.instance.gameMode = GameModeType.Survival;

        LoadScene(1);
    }

    public void LoadGameModeContainment()
    {
        LevelManager.instance.gameMode = GameModeType.Containment;

        LoadScene(1);
    }
}
