using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

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

    public BoxCollider CoinSpawnArea
    {
        get
        {
            return _settings.CoinSpawnArea;
        }
    }

    public MeshCollider FunnelCollider
    {
        get
        {
            return _settings.FunnelCollider;
        }
    }
    
    [Serializable]
    public sealed class Settings
    {
        public Transform CoinSpawnTransform;
        public BoxCollider CoinSpawnArea;
        public MeshCollider FunnelCollider;
    }
    
    [Inject]
    public BoardController(Settings settings)
    {
        this._settings = settings;
    }
}
