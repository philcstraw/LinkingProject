using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwapTilePowerup : PowerupBase 
{
    public Image grabbedImage;
    public PlaySound grabSound;
    public PlaySound swapSound;
    public PlaySound ungrabSound;
    public GameObject grabIconObject;
    public float targetScaleOffset = 0.25f;
    public float timeBetweenScalePop = 2f;

    float m_time = 0f;
    bool m_grabbed = false;
    bool m_clicked = false;
    int m_lastChainCount = 0;

    void Update () 
    {
        if (meshTile == null)
            return;

        m_clicked = TileRaycaster.instance.isMouseClicked;

        if (TileRaycaster.instance.clickedTile == meshTile.tile && !m_grabbed && m_clicked && !meshTile.selected && m_lastChainCount <= 1)
        {
            if (grabSound != null)
                grabSound.PlayOneShot();

            m_grabbed = true;
            if (grabbedImage != null)
                grabbedImage.enabled = true;
            return;
        }

        // used to prevent accidently selecting a swap tile when dragging to chain tiles together and hovering over and releasing over a swap tile.
        // note that this has to come after the grab logic, otherwise the mouse up for the current frame will reset lastChainCount back to 0 and the bug still occurs.
        if (TileRaycaster.instance.isMouseDown)
            m_lastChainCount = TileChainer.instance.selectedTiles.Count;
        else
            m_lastChainCount = 0;

        if (m_clicked && TileRaycaster.instance.clickedTile == meshTile.tile)
        {
            if (ungrabSound != null)
                ungrabSound.PlayOneShot();

            m_grabbed = false;
            if (grabbedImage != null)
                grabbedImage.enabled = false;
            return;
        }

        if (m_clicked && m_grabbed && TileChainer.instance.selectedTiles.Count < 2)
        {
            LevelTile _next = TileRaycaster.instance.clickedTile;
            if (_next == null)
                return;

            if (_next.meshTile == null)
                return;

            if (_next.meshTile != meshTile)
            {
                if (swapSound != null)
                    swapSound.PlayOneShot();

                MeshTileManager.instance.SwapTiles(meshTile, _next.meshTile);
                meshTile.gameObject.transform.localScale = TileChainer.instance.startScale;
                _next.meshTile.gameObject.transform.localScale = TileChainer.instance.startScale;
                meshTile.tile.available = true;
                Destroy(this.gameObject);
            }
        }

        if (!m_grabbed)
        {
            m_time += Time.deltaTime;
            if(m_time >= timeBetweenScalePop)
            {
                m_time = 0f;
                StartCoroutine(PopScale());
            }
        }
        else
        {
            m_time = 0f;
        }
    }

    IEnumerator PopScale()
    {
        Vector3 _startScale = grabIconObject.transform.localScale;
        Vector3 _targetScale = _startScale - new Vector3(targetScaleOffset, targetScaleOffset, targetScaleOffset);
        float t = 0f;
        float _duration = 0.4f;
        while(t < _duration)
        {
            t += Time.deltaTime;
            float d = t / _duration;
            grabIconObject.transform.localScale = Vector3.Lerp(_startScale, _targetScale, Mathf.PingPong(d * 2f, 1f));
            yield return null;
        }
        grabIconObject.transform.localScale = _startScale;
    }
}
