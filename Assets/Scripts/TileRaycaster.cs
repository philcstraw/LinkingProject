using UnityEngine;

public class TileRaycaster : MonoBehaviour 
{
    public GameLogic gameLogic;
    public int mouseButton = 0;
    [ReadOnly] public bool isMouseDown;
    [ReadOnly] public bool isMouseClicked;

    internal LevelTile hoveredTile;
    internal LevelTile heldTile;
    internal LevelTile clickedTile;
    internal static TileRaycaster instance;

    RaycastHit2D[] m_hits;

    void Awake()
    {
        instance = this;
        m_hits = new RaycastHit2D[1];
    }

    void Update ()
    {
        if (gameLogic.paused)
            return;

        isMouseDown = Input.GetMouseButton(mouseButton);
        isMouseClicked = Input.GetMouseButtonUp(mouseButton);
        
        if(isMouseClicked)
            if (hoveredTile != clickedTile)
                clickedTile = hoveredTile;

        if (!isMouseDown)
        {
            hoveredTile = null;
            heldTile = null;
            return;
        }

        Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int _result = Physics2D.GetRayIntersectionNonAlloc(_ray,m_hits);

        if (_result > 0)
        {
            Transform _objectHit = m_hits[0].transform;
            LevelTile _tile = _objectHit.GetComponent<LevelTile>();

            hoveredTile = _tile;
            heldTile = hoveredTile;
        }
        else
        {
            hoveredTile = null;
            heldTile = null;
        }
    }
}
