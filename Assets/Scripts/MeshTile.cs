using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { Red = 0, Green = 1, Blue = 2, Yellow = 3, Purple = 4 }

// script attached to 3D mesh tiles. Contains the different tile types based on color
public class MeshTile : MonoBehaviour 
{
    public Renderer _renderer;
    public AnimationCurve moveCurve;
    // should probably move to another class?
    public float moveSpeed = 0.5f;
    public TileGlow tileGlow;
    [ReadOnly] public TileType type;
    [ReadOnly] public PowerupBase currentPowerUp;

    internal PersistentTilePowerup persistantPowerup;
    internal Vector3 startSize;
    internal LevelTile tile;
    internal bool selected;
    internal bool delete;
    internal bool persistent;
    internal bool addLife;
    internal bool moving {  get { return m_moving; } }
    bool m_moving;
    bool m_scaling;

    //store the last scaled coroutine so we can cancel it
    IEnumerator m_scaleRoutine;
    Queue<Vector4> m_targetPositions;

    void Start()
    {
        m_targetPositions = new Queue<Vector4>();

        StartCoroutine(ProcessQue());
    }

    internal void NextRandomType()
    {
        List<TileType> _types = new List<TileType>() { TileType.Red, TileType.Green, TileType.Blue, TileType.Yellow, TileType.Purple };
        _types.RemoveAt((int)type);

        TileType _next_random = _types[UnityEngine.Random.Range(0,_types.Count - 1)];
        type = _next_random;

        UpdateMaterialFromType();
    }

    internal void UpdateMaterialFromType()
    {
        Material _material = MeshTileManager.instance.materials[(int)type];

        _renderer.material = _material;

        tileGlow.SetMaterial(_material);
    }

    internal void MoveTo(Vector3 targetPos)
    {
          m_targetPositions.Enqueue(new Vector4(targetPos.x, targetPos.y, targetPos.z, moveSpeed));
    }

    internal void PopPosition(Vector4 start, Vector4 targetPos)
    {
        m_targetPositions.Enqueue(targetPos);

        StartCoroutine(PopPositionRoutine(start));
    }

    internal void CancelScaling()
    {
        if (this == null)
            return;

        if (m_scaleRoutine != null)
            StopCoroutine(m_scaleRoutine);

        m_scaling = false;
    }

    internal void ScaleTo(Vector3 targetScale, float speed, AnimationCurve scaleCurve)
    {
        if (this == null)
            return;

        StartCoroutine(m_scaleRoutine = LerpScaleTo(targetScale, speed, scaleCurve));
    }

    internal void ScaleToDelayed(Vector3 targetScale, float speed, AnimationCurve scaleCurve,float delay)
    {
        if (this == null)
            return;

        StartCoroutine(ScaleToRoutine(targetScale, speed, scaleCurve, delay));
    }

    internal void AddPowerup(PowerupBase powerUp)
    {
        tile.available = false;
        powerUp.meshTile = this;
        currentPowerUp = powerUp;
        powerUp.transform.SetParent(transform, false);
        powerUp.transform.localPosition = new Vector3(0, 0, powerUp.zLocalOffset);
        powerUp.enabled = true;
    }

    internal void Destory()
    {
        tile.available = true;
        Destroy(gameObject);
    }

    IEnumerator PopPositionRoutine(Vector4 start)
    {
        while (moving)
            yield return null;

        m_targetPositions.Enqueue(start);
    }

    IEnumerator LerpPositionTo(Vector3 targetPos, float speed)
    {
        float _animTime = 0.0f;
        float _curveValue = 0f;
        Vector3 _lastPos = transform.position;
        m_moving = true;

        while (_animTime < 1f)
        {
            _animTime += Time.deltaTime * speed;
            _curveValue = moveCurve.Evaluate(_animTime);
            transform.position = Vector3.Lerp(_lastPos, targetPos, _curveValue);
            yield return null;
        }
        m_moving = false;
        transform.position = targetPos;
    }

    IEnumerator ScaleToRoutine(Vector3 targetScale, float speed, AnimationCurve scaleCurve, float delay)
    {
        yield return new WaitForSeconds(delay);

        ScaleTo(targetScale, speed, scaleCurve);
    }

    IEnumerator LerpScaleTo(Vector3 targetScale, float speed, AnimationCurve scaleCurve)
    {
        float _animTime = 0.0f;
        float _curveValue = 0f;
        Vector3 _lastScale = transform.localScale;
        m_scaling = true;

        while (_animTime < 1f)
        {
            _animTime += Time.deltaTime * speed;
            _curveValue = scaleCurve.Evaluate(_animTime);
            transform.localScale = Vector3.Lerp(_lastScale, targetScale, _curveValue);
            yield return null;
        }
        m_scaling = false;
        transform.localScale = targetScale;
    }

    IEnumerator ProcessQue()
    {
        while (true)
        {
            while (m_targetPositions.Count > 0)
            {
                Vector4 _targetPosition = m_targetPositions.Peek();

                yield return StartCoroutine(LerpPositionTo(new Vector3(_targetPosition.x, _targetPosition.y, _targetPosition.z), _targetPosition.w));

                m_targetPositions.Dequeue();
            }
            yield return null;
        }
    }
}