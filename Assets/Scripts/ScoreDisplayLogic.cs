using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreDisplayLogic : MonoBehaviour
{
    public TextMeshPro textMeshPro;
    public Vector3 incrementPosition = new Vector3(0f, 0.2f, 0f);
    public float incrementScale = 0.2f;
    public float positionSpeed = 1f;
    public float scaleSpeed = 1f;
    public float fadeRate = 0.2f;
    public float fadeMulttplier = 4f;
    public bool useUscaledTime;

    internal float fadeDuration = 1f;
    internal bool easeOutAlpha = false;

    float m_increment = 0;
    float m_time;
    bool m_beginFade = false;

    void Update ()
    {
        m_time = useUscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        gameObject.transform.position += incrementPosition * positionSpeed * m_time;

        float _scale = incrementScale * scaleSpeed * m_time;
        gameObject.transform.localScale += new Vector3(_scale, _scale, _scale);

        if (gameObject.transform.localScale.x < 0f)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        if (!easeOutAlpha)
        {
            m_increment = m_time * fadeRate;

            if (easeOutAlpha)
                m_increment = Utility.EaseOut(m_increment);

            textMeshPro.color -= new Color(0.0f, 0.0f, 0.0f, m_increment * fadeMulttplier);

            if (textMeshPro.color.a <= 0.0f)
                DestroyImmediate(this.gameObject);
        }
        else
        {
            if(!m_beginFade)
            {
                m_beginFade = true;
                StartCoroutine(FadeAlpha(fadeDuration));
            }
        }
    }

    IEnumerator FadeAlpha(float duration)
    {
        float startAlpha = textMeshPro.color.a;
        float t = 0f;
        Color _col = textMeshPro.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float _lerp = t / duration;
            _lerp = Utility.EaseIn(_lerp);
            _col.a = Mathf.Lerp(startAlpha, 0f, _lerp);
            textMeshPro.color = _col;

            yield return null;
        }
        DestroyImmediate(gameObject);
    }
}
