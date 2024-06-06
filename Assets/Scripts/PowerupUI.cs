using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupUI : MonoBehaviour
{
    public Game game;
    public PowerupManager powerupManager;
    public TileChainer tileChainer;
    public GameObject defaultSlot;
    public GameObject panel;
    public Color32 selectedColor;
    public Color32 onGridColor;
    public float spacing = 10f;
    public float selectedScaleOffset = 0.2f;
    public bool layoutHorizontal = true;

    Sprite m_defaultSprite;
    Dictionary<int, PowerupSpawner> m_spawners = new Dictionary<int, PowerupSpawner>();
    List<Image> m_slots;
    List<Sprite> m_sprites;
    List<bool> m_checkPowerup;
    List<RectTransform> m_rects;
    Vector3 m_defaultScale;
    Vector3 m_targetScale;

    void Start () 
    {
        int _powerupCount = game.mode.powerups.Count;

        if (_powerupCount > 0)
        {
            int _lastComboOffset = game.mode.powerups[_powerupCount - 1].comboOffset;

            CreateSlots(_lastComboOffset);
        }
	}

    void Update()
    {
        if(m_slots != null && m_slots.Count > 0)
            ShowIncomingPowerups();
    }

    void CreateSlots(int numSlots)
    {
        m_slots = new List<Image>(numSlots);

        RectTransform _defaultRect = defaultSlot.GetComponent<RectTransform>();
        m_defaultScale = _defaultRect.localScale;
        m_targetScale = m_defaultScale + new Vector3(selectedScaleOffset, selectedScaleOffset, 0f);

        Image _defaultImage = defaultSlot.GetComponent<Image>();
        m_defaultSprite = _defaultImage.sprite;

        if (!layoutHorizontal)
        {
            for (int i = 0; i < numSlots; i++)
            {
                GameObject _image = Instantiate(defaultSlot);
                Vector2 _pos = _defaultRect.anchoredPosition;

                if(!layoutHorizontal)
                    _pos.y += i * spacing;
                else
                    _pos.x += i * spacing;

                RectTransform _rect = _image.gameObject.GetComponent<RectTransform>();
                _rect.anchoredPosition = _pos;
                _rect.SetParent(panel.transform, false);
                m_slots.Add(_image.GetComponent<Image>());
            }
        }
        defaultSlot.SetActive(false);
        AssignPowerups();
    }

    void AssignPowerups()
    {
        int _powerupCount = game.mode.powerups.Count;
        for (int i = 0; i < _powerupCount; i++)
        {
            var _powerup = game.mode.powerups[i];
            int _comboOffset = _powerup.comboOffset - 1;
             
            Image _image = m_slots[_comboOffset];
            var _spawner = powerupManager.SpawnerFromType(_powerup.type);
            m_spawners.Add(_comboOffset, _spawner);
            _image.sprite = _spawner.powerUpPrefab.GetComponentInChildren<SpriteRenderer>().sprite;

            if (_powerup.type == PowerupType.ScoreColumn || _powerup.type == PowerupType.ChangeDirection)
            {
                RectTransform _rect = _image.gameObject.GetComponent<RectTransform>();
                _rect.eulerAngles = new Vector3(0f, 0f, 90f);
            }
        }

        m_sprites = new List<Sprite>();
        m_checkPowerup = new List<bool>();
        m_rects = new List<RectTransform>();
        for (int i = 0; i < m_slots.Count; i++)
        {
            m_sprites.Add(m_slots[i].GetComponent<Image>().sprite);
            m_rects.Add(m_slots[i].GetComponent<RectTransform>());
            m_checkPowerup.Add(true);
        }
    }

    void ShowIncomingPowerups()
    {
        int _chainCount = tileChainer.selectedTiles.Count;
        int _powerupCount = m_slots.Count;
        int _selectionCount = Mathf.Min(_chainCount, _powerupCount);

        for (int i = 0; i < m_slots.Count; i++)
        {
            bool _hasPowerupOnGrid = HasPowerupOnGrid(i);
            Image _image = m_slots[i];

            RectTransform _rect = m_rects[i];

            // No chain currently selected
            if (_chainCount < 1)
            {
                // if no chains ( _chainCount < 1) but there has been a chain (icon scale has been scaled up) then reset the menu icons
                if (_rect.localScale.x == m_targetScale.x)
                {
                    StartCoroutine(AdjustScale(_rect, m_defaultScale));
                    _image.sprite = m_defaultSprite;
                    _image.color = onGridColor;
                    m_checkPowerup[i] = false;
                }

                if (m_checkPowerup[i])
                {
                    _image.sprite = _hasPowerupOnGrid ? m_defaultSprite : m_sprites[i];
                    _image.color = onGridColor;
                }

                // A powerup has been unlocked but we need to wait till it's actually on the grid before we can updating it again 
                if (_rect.localScale.x == m_defaultScale.x && _hasPowerupOnGrid && _image.sprite == m_defaultSprite)
                    m_checkPowerup[i] = true;
            }
            else // there is an active chain
            {
                // only select icons under the chain count. Obviously for highlighting icons, this is capped to the number of powerups (see declaring of _selectionCount)
                if (i < _selectionCount)
                {
                    if (_rect.localScale.x == m_defaultScale.x)
                    {
                        StartCoroutine(AdjustScale(_rect, m_targetScale));

                        if (m_spawners.ContainsKey(i) && !m_spawners[i].HasPowerupOnGrid())
                            _image.color = selectedColor;
                    }
                }
            }
        }
    }

    bool HasPowerupOnGrid(int i)
    {
        if (!m_spawners.ContainsKey(i))
            return false;

        return m_spawners[i].HasPowerupOnGrid();
    }

    IEnumerator AdjustScale(RectTransform rect, Vector3 desireScale)
    {
        var _startScale = rect.localScale;
        float t = 0f;
        float _duration = 0.3f;
        while(t < _duration)
        {
            t += Time.deltaTime;
            rect.localScale = Vector3.Lerp(_startScale, desireScale, t / _duration);
            yield return null;
        }
        rect.localScale = desireScale;
    }
}
