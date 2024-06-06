using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileChainer : MonoBehaviour
{
    public TileRaycaster tileRaycaster;
    public MeshTileManager meshTileManager;
    public ScoreGrid scoreGrid;
    // Default to ease out curve in the inspector.
    public AnimationCurve selectedScaleCurve = new AnimationCurve();
    public float selectedScaleOffset = -0.12f;
    public float selectedScaleSpeed = 2f;
    public int minimumChainScore = 2;
    public int minimumChainCount = 2;
    public bool chipOnDelete = false;

    internal UnityEvent onChainedTiles = new UnityEvent();
    internal Vector3 startScale;
    internal List<MeshTile> selectedTiles = new List<MeshTile>();
    internal static TileChainer instance;

    Vector3 m_selectedScale;
    MeshTile m_prevtile;
    bool m_chip = false;

    void Awake()
    {
        instance = this;
    }

    void Start () 
    {
        startScale = meshTileManager.baseMeshTile.transform.localScale;
        m_selectedScale = startScale + new Vector3(selectedScaleOffset, selectedScaleOffset, selectedScaleOffset);
	}
    
	void Update () 
    {
        if (chipOnDelete && m_chip)
        {
            meshTileManager.ChipTile();
            m_chip = false;
            return;
        }

        if (tileRaycaster.isMouseDown)
        {
            if (tileRaycaster.hoveredTile != null)
            {
                MeshTile _currentTile = tileRaycaster.hoveredTile.meshTile;
                if (_currentTile != null)
                {
                    if (!selectedTiles.Contains(_currentTile))
                    {
                        if (m_prevtile != null && meshTileManager.IsValidSidling(_currentTile, m_prevtile))
                        {
                            AddTile(_currentTile);
                            m_prevtile = _currentTile;
                        }

                        if (m_prevtile == null)
                        {
                            AddTile(_currentTile);
                            m_prevtile = _currentTile;
                        }
                    }
                }
            }
        }
        else
        {
            if (selectedTiles.Count >= minimumChainCount)
            {
                if (selectedTiles.Count >= minimumChainScore)
                {
                    scoreGrid.ScoreChain(selectedTiles);
                    selectedTiles.Clear();
                }else{
                    foreach (MeshTile _tile in selectedTiles)
                    {
                        _tile.tile.Delete();
                        m_chip = true;
                    }
                    selectedTiles.Clear();
                }
                onChainedTiles.Invoke();
            }
            else
                ResetSelectedTiles();

            if (m_prevtile != null)
                m_prevtile = null;
        }
    }

    void ResetSelectedTiles()
    {
        if (selectedTiles.Count > 0)
        {
            foreach (MeshTile _meshTile in selectedTiles)
            {
                if (_meshTile != null)
                {
                    _meshTile.selected = false;
                    _meshTile.CancelScaling();
                    _meshTile.ScaleTo(startScale, selectedScaleSpeed, selectedScaleCurve);
                }
            }
            if (m_prevtile != null)
                m_prevtile = null;

            selectedTiles.Clear();
        }
    }

    bool shouldScale(MeshTile lt)
    {
        if (lt != null)
            return true;
        return false;
    }

    void AddTile(MeshTile lt)
    {
        selectedTiles.Add(lt);
        lt.selected = true;
        if (shouldScale(lt))
        {
            SoundBoard.instance.selectionSound.PlayOneShot();
            lt.CancelScaling();
            lt.ScaleTo(m_selectedScale, selectedScaleSpeed, selectedScaleCurve);
        }
    }
}
