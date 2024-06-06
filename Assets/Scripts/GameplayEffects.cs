using System.Collections;
using TMPro;
using UnityEngine;

public class GameplayEffects : MonoSingleton<GameplayEffects>
{
    public Game game;
    public LevelLayout levelLayout;
    public GameObject instantiatedObjectContainer;
    public ScoreDisplayLogic baseScoreDisplay;
    public ScoreDisplayLogic baseParsistantDisplay;
    public Vector3 scoreDisplayOffset = new Vector3(0.25f, 0.25f, 0.0f);
    public TextMeshPro newLevelObject;
    
    internal static GameplayEffects instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        newLevelObject.gameObject.SetActive(false);
    }
    
    // score that appears when a set of tiles are successfully scored
    internal ScoreDisplayLogic DisplayPopupScore(int x, int y, string score, float scale)
    {
        ScoreDisplayLogic _sdl = Instantiate(baseScoreDisplay);
        GameObject _go = _sdl.gameObject;

        _sdl.enabled = true;
        _sdl.textMeshPro.text = score;

        Vector3 _pos = levelLayout.GridPosition(x, y);
        _pos.z = -0.45f;

        _go.transform.position = _pos;
        _go.transform.localScale = new Vector3(scale, scale, scale);
        
        if (instantiatedObjectContainer != null)
            _go.transform.SetParent(instantiatedObjectContainer.transform);

        return _sdl;
    }

    internal ScoreDisplayLogic DisplayPersistantCount(int x, int y, string count, float scale,float scaleSpeed, float incrementOffset, float fadeDuration)
    {
        ScoreDisplayLogic _sdl = Instantiate(baseParsistantDisplay);
        GameObject _go = _sdl.gameObject;

        _sdl.incrementScale = incrementOffset;
        _sdl.enabled = true;
        _sdl.scaleSpeed = scaleSpeed;
        _sdl.textMeshPro.text = count;
        _sdl.easeOutAlpha = true;
        _sdl.fadeDuration = fadeDuration;

        Vector3 _pos = levelLayout.GridPosition(x, y);
        _pos.z = -0.45f;
        _go.transform.position = _pos;
        _go.transform.localScale = new Vector3(scale, scale, scale);
        
        if (instantiatedObjectContainer != null)
            _go.transform.SetParent(instantiatedObjectContainer.transform);

        return _sdl;
    }

    internal ScoreDisplayLogic GetScoreDisplay()
    {
        ScoreDisplayLogic _sdl = Instantiate(baseScoreDisplay);
        GameObject _go = _sdl.gameObject;
        _sdl.enabled = true;

        if (instantiatedObjectContainer != null)
            _go.transform.SetParent(instantiatedObjectContainer.transform);

        return _sdl;
    }

    internal void DisplayGameMessage(string text)
    {
        ScoreDisplayLogic _sdl = GetScoreDisplay();

        _sdl.transform.position = Vector3.zero;
        _sdl.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        _sdl.incrementScale = 0.1f;
        _sdl.fadeRate = 0.1f;
        _sdl.incrementPosition = Vector3.zero;
        _sdl.textMeshPro.text = text;
    }

    internal void DisplayNewLevel()
    {
        StartCoroutine(DisplayNewLevelRoutine());
    }

    IEnumerator DisplayNewLevelRoutine()
    {
        float _duration = 2f;
        float _time = 0f;
        float _targetScale = 2f;
        Vector3 _scale = newLevelObject.transform.localScale * _targetScale;
        Vector3 _startScale = newLevelObject.transform.localScale;

        var _newTex = Instantiate(newLevelObject);
        _newTex.gameObject.SetActive(true);
        _newTex.gameObject.transform.SetParent(newLevelObject.transform.parent);
        _newTex.gameObject.transform.position = newLevelObject.transform.position;
        _newTex.text = string.Concat("Level", "  ", game.mode.CurrentLevel().ToString());

        while(_time < _duration)
        {
            _time += Time.unscaledDeltaTime;
            float l = _time / _duration;
            _newTex.gameObject.transform.localScale = Vector3.Lerp(_startScale, _scale, l);
            _newTex.alpha = 1f - l;
            yield return null;
        }
    }
}
