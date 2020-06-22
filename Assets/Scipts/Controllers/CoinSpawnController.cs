using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class CoinSpawnController: ITickable, ISaveLoadable<RawCoinSpawnControllerData>
{
    private readonly Settings _settings;
    private readonly Coin.Factory _coinFactory;
    private readonly BoardController _boardController;

    private List<Coin> _coins = new List<Coin>();
    
    [Serializable]
    public sealed class Settings
    {
        public Coin coinPrefab;
        public Vector3 randomCoinDropOffsetFromCenter;
        public float distanceBetweenCoins;
        public float coinRadius;
    }

    [Inject]
    public CoinSpawnController(Settings settings, Coin.Factory coinFactory, BoardController boardController)
    {
        _settings = settings;
        _coinFactory = coinFactory;
        _boardController = boardController;
    }

    public void PopulateBoard()
    {
        var bounds = _boardController.CoinSpawnArea.bounds;

        float increment = _settings.distanceBetweenCoins + (_settings.coinRadius * 2);
        
        for (float x = bounds.min.x; x < bounds.max.x; x += increment)
        {
            for (float z = bounds.min.z; z < bounds.max.z; z += increment)
            {
                var hitColliders = Physics.OverlapCapsule(new Vector3(x, bounds.min.y, z),
                    new Vector3(x, bounds.max.y, z), _settings.coinRadius, Physics.AllLayers, QueryTriggerInteraction.Collide);

                // if this coin will hit the funnel, then ignore
                if (hitColliders.Contains(_boardController.FunnelCollider)) continue;

                var coin = CreateCoin();
                coin.transform.position = new Vector3(x, bounds.center.y, z);
            }
        }
    }

    public void ClearBoard()
    {
        foreach (Coin coin in _coins)
        {
            coin.Destroy();
        }
        
        _coins.Clear();
    }

    private Coin CreateCoin()
    {
        var coin = _coinFactory.Create();
        _coins.Add(coin);
        
        return coin;
    }

    private Vector3 GetCoinDropPosition()
    {
        Vector3 offset = Vector3.Lerp(-_settings.randomCoinDropOffsetFromCenter,
            _settings.randomCoinDropOffsetFromCenter, UnityEngine.Random.value);

        return _boardController.CoinSpawnPosition + offset;
    }

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var coin = CreateCoin();
            coin.transform.position = GetCoinDropPosition();
        }
    }

    public void Load(RawCoinSpawnControllerData data)
    {
        ClearBoard();
        
        foreach (var rawCoinData in data.coins)
        {
            var coin = CreateCoin();
            coin.Load(rawCoinData);
        }
    }

    public RawCoinSpawnControllerData Save()
    {
        var data = new RawCoinSpawnControllerData();

        foreach (Coin coin in _coins)
        {
            var rawCoinData = coin.Save();
            data.coins.Add(rawCoinData);
        }

        return data;
    }
}

public class RawCoinSpawnControllerData
{
    public List<RawCoinData> coins = new List<RawCoinData>();
}
