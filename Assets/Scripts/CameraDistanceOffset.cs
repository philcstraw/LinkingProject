using UnityEngine;

public class CameraDistanceOffset : MonoBehaviour 
{
    public LevelLayout levelLayout;
    public Vector2Int referenceResolution = new Vector2Int(480, 800);
    public float zDistanceOffset = -7.1f;
    public bool debugUpdate = false;

    Vector3 m_OriginalPosition;
    float m_finalOffset;

    void Start () 
    {
        m_OriginalPosition = transform.position;

        CalculateZoffset();
    }

    public void CalculateZoffset()
    {
        Vector3 currentPos = transform.position;

        if (levelLayout.layoutGrid == null)
            return;

        float ratio = levelLayout.layoutGrid.columnLength / 4f;

        if (levelLayout.layoutGrid.rowLength > levelLayout.layoutGrid.columnLength)
            ratio = (float)levelLayout.layoutGrid.rowLength/3.5f;
        
        m_finalOffset = zDistanceOffset * ratio;

        CalculateZFromAspectRation();
    }

    void CalculateZFromAspectRation()
    {
        if (referenceResolution.y == 0 || referenceResolution.x == 0)
            return;

        float refRatio = (float)referenceResolution.x / (float)referenceResolution.y;

        var ratio = (float)Screen.width / (float)Screen.height;

        float targetRatio = 1f;

        if (Screen.width < Screen.height)
            targetRatio = refRatio / ratio;
            
        transform.position = m_OriginalPosition + new Vector3(0f, 0f,  m_finalOffset * targetRatio);
    }
	
	void Update () 
    {    
        if (debugUpdate)
            CalculateZoffset();
	}
}
