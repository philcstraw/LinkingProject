using System.Collections.Generic;
using UnityEngine;

public class ChangeDirectionPowerup : PowerupBase 
{
    public RectTransform rect;

    CollapseDirection m_direction = CollapseDirection.Up;

    static readonly CollapseDirection[] m_excludeDown = { CollapseDirection.Up, CollapseDirection.Right, CollapseDirection.Left };
    static readonly CollapseDirection[] m_excludeUp = { CollapseDirection.Down, CollapseDirection.Right, CollapseDirection.Left };
    static readonly CollapseDirection[] m_excludeLeft = { CollapseDirection.Up, CollapseDirection.Right, CollapseDirection.Down };
    static readonly CollapseDirection[] m_excludeRight = { CollapseDirection.Up, CollapseDirection.Down, CollapseDirection.Left };

    static readonly Vector3 m_directionLeftToRotation = new Vector3(0f, 0f, 0f);
    static readonly Vector3 m_directionRightToRotation = new Vector3(0f, 0f, 180f);
    static readonly Vector3 m_directionUpToRotation = new Vector3(0f, 0f, -90f);
    static readonly Vector3 m_directionDownToRotation = new Vector3(0f, 0f, 90f);

    void Start () 
    {
        RandomDirection();
	}
	
    void RandomDirection()
    {
        switch(MeshTileManager.instance.collapseDirection)
        {
            case CollapseDirection.Down:
                m_direction = m_excludeDown[Random.Range(0, 3)];
                rect.eulerAngles = m_directionDownToRotation;
                break;
            case CollapseDirection.Up:
                m_direction = m_excludeUp[Random.Range(0, 3)];
                rect.eulerAngles = m_directionUpToRotation;
                break;
            case CollapseDirection.Left:
                m_direction = m_excludeLeft[Random.Range(0, 3)];
                rect.eulerAngles = m_directionLeftToRotation;
                break;
            case CollapseDirection.Right:
                m_direction = m_excludeRight[Random.Range(0, 3)];
                rect.eulerAngles = m_directionRightToRotation;
                break;
        }

        switch(m_direction)
        {
            case CollapseDirection.Down:
                rect.eulerAngles = m_directionDownToRotation;
                break;
            case CollapseDirection.Up:
                rect.eulerAngles = m_directionUpToRotation;
                break;
            case CollapseDirection.Left:
                rect.eulerAngles = m_directionLeftToRotation;
                break;
            case CollapseDirection.Right:
                rect.eulerAngles = m_directionRightToRotation;
                break;
        }
    }

    public override void OnUse(List<MeshTile> chain, int score)
    {
        MeshTileManager.instance.collapseDirection = m_direction;
    }
}
