using System.Collections.Generic;
using UnityEngine;

public class ScoreTileFXControl : MonoBehaviour
{
    public GameObject fxContainer;
    public ParticleSystem particleSystemPrefab;
    public List<Material> materialTypes = new List<Material>();
    public List<ParticleSystem> particlePlayerTypes;
    public float scoreFxZoffset = -0.5f;

    internal static ScoreTileFXControl instance;

    void Awake()
    {
        instance = this;        
    }

    void Start () 
    {
        // create a list of particle systems for each type and assign the corresponding material. num materials = valid tile type count
        // the order of materials in the list should match the corresponding tile type
        particlePlayerTypes = new List<ParticleSystem>(materialTypes.Count);
        for (int i = 0; i < materialTypes.Count; i++)
        {
            ParticleSystem _particleSystem = Instantiate(particleSystemPrefab);
            _particleSystem.GetComponentInChildren<ParticleSystemRenderer>().material = materialTypes[i];
            particlePlayerTypes.Add(_particleSystem);

            if(fxContainer != null)
                _particleSystem.transform.SetParent(fxContainer.transform);
        }
    }

    internal void Play(Vector3 position, TileType type)
    {
        ParticleSystem _particlePlayer = Instantiate(particlePlayerTypes[((int)type)]);
        _particlePlayer.transform.position = position + new Vector3(0f, 0f, scoreFxZoffset);
        _particlePlayer.Play();
        Destroy(_particlePlayer.gameObject, 3f);
    }
}
