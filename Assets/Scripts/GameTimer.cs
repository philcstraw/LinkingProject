using System.Collections;
using UnityEngine;

// generic timer script
// TODO: Can probably deprecate this class
public class GameTimer : MonoBehaviour
{
    public int timeLeft = 60;
    public bool start = false;
    public bool countUp = false;
    public bool resetOnCompletion = false;
    public bool once = false;
    public bool pause = false;
    [ReadOnly] public bool timeUp = false;
    [ReadOnly] public float minutes;
    [ReadOnly] public int seconds;

    float _countdown = 60f;
    bool _completedOnce = false;

    void Start()
    {
        if(start)
            SetTime(timeLeft);
    }

    void Update () 
    {
        if (!start)
            return;

        if(_completedOnce)
        {
            if (timeUp)
                timeUp = false;

            return;
        }

        if (resetOnCompletion && timeUp)
            timeUp = false;

        UpdateTimer();

        if (_countdown <= 0f)
        {
            if (once)
                _completedOnce = true;

            if (resetOnCompletion)
                ResetTimer();

            timeUp = true;
        }
	}

    internal void Begin(float delay)
    {
        StartCoroutine(BeginRountine(delay));
    }

    internal void SetTime(int time)
    {
        _countdown = time;

        if(_countdown > 0)
            timeUp = false;
    }

    internal void AddTime(int time)
    {
        _countdown += time;

        if (_countdown > 0)
            timeUp = false;
    }

    internal string GetTimeAsString()
    {
        string sstring = seconds.ToString();

        if (seconds < 10)
            sstring = string.Concat("0", sstring);

        return string.Concat(minutes.ToString(), " : ", sstring);
    }

    internal void ResetTimer()
    {
        SetTime(timeLeft);
    }

    IEnumerator BeginRountine(float delay)
    {
        SetTime(timeLeft);

        yield return new WaitForSeconds(delay);

        start = true;
    }

    void UpdateTimer()
    {
        if (!countUp)
        {
            _countdown -= Time.deltaTime;

            if (_countdown < 0f)
                _countdown = 0f;
        }
        else
            _countdown += Time.deltaTime;

        UpdateMinutesAndSeconds();
    }

    void UpdateMinutesAndSeconds()
    {
        minutes = Mathf.Floor(_countdown / 60);
        seconds = Mathf.RoundToInt(_countdown % 60);
    }
}
