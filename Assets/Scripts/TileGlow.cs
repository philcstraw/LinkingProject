using System.Collections;
using UnityEngine;

public class TileGlow : MonoBehaviour
{
    public Renderer _renderer;

    internal bool glowing;

    IEnumerator m_glowEnumerator;
    float m_emission;

    void Start ()
    {
        DisableGlow();
    }
    
    internal void SetMaterial(Material m)
    {
        _renderer.material = m;
    }

    internal void DoGlowRoutine(float rateOfGlow, float target)
    {
        if (this == null)
            return;

        StartCoroutine(m_glowEnumerator = DoGlow(rateOfGlow, target));
    }

    internal void StopGlowRoutine(float rateOfGlow)
    {
        if (this == null)
            return;

        if (m_glowEnumerator != null)
            StopCoroutine(m_glowEnumerator);

        StartCoroutine(StopGlow(rateOfGlow));
    }

    void SetEmission(float value)
    {
        _renderer.material.SetFloat("_emissive", value);
    }

    IEnumerator DoGlow(float rateOfGlow, float target)
    {
        EnableGlow();

        while (m_emission < target)
        {
            m_emission += rateOfGlow * Time.deltaTime;
            SetEmission(m_emission);
            yield return null;
        }
        glowing = false;
    }

    IEnumerator StopGlow(float rateOfGlow)
    {
        glowing = true;
        while (m_emission > -0.5f)
        {
            m_emission -= rateOfGlow * Time.deltaTime;
            SetEmission(m_emission);
            yield return null;
        }
        DisableGlow();
    }
    
    void EnableGlow()
    {
        m_emission = 0f;
        SetEmission(m_emission);
        _renderer.enabled = true;
        glowing = true;
    }

    void DisableGlow()
    {
        _renderer.enabled = false;
        m_emission = -0.5f;
        SetEmission(m_emission);
        glowing = false;
    }
}
