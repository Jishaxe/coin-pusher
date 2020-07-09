using System;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class BoardController
{
    private Settings _settings;

    public Vector3 CoinSpawnPosition
    {
        get
        {
            return _settings.CoinSpawnTransform.position;
        }
    }

    [Serializable]
    public sealed class Settings
    {
        public Transform CoinSpawnTransform;
        public BoxCollider CoinPopulateArea;
    }

    public Vector3 GetRandomPopulationPosition()
    {
        var bounds = _settings.CoinPopulateArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(x, y, z);
    }
    
    [Inject]
    public BoardController(Settings settings)
    {
        this._settings = settings;
    }
}
