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

    private List<Coin> _coinPrefabsSortedByValue = new List<Coin>();
    
    [Serializable]
    public sealed class Settings
    {
        public List<Coin> coinPrefabs;
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
        
        ProcessCoinPrefabs();
    }

    private void ProcessCoinPrefabs()
    {
        _coinPrefabsSortedByValue = _settings.coinPrefabs.OrderByDescending(coin => coin.value).ToList();
        Debug.Assert(!_coinPrefabsSortedByValue.Any((coin) => coin.value == 0), "Coin has zero value");
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

                //var coin = CreateCoin();
               // coin.transform.position = new Vector3(x, bounds.center.y, z);
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

    private Coin CreateCoin(Coin coinDef)
    {
        var coin = _coinFactory.Create(coinDef);
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
            QueueDonation("Test Donation", "Test Message", 5.55f);
        }
    }

    public void QueueDonation(string name, string message, float amount)
    {
        Debug.Log($"Queuing donation, name: {name}, message: {message}, amount: {amount}");
        var coins = BreakDownBalanceIntoCoins(amount);

        foreach (Coin coin in coins)
        {
            var newCoin = _coinFactory.Create(coin);
            newCoin.transform.position = GetCoinDropPosition();
        }
       
    }
    
    private List<Coin> BreakDownBalanceIntoCoins(float amount)
    {
        var results = new List<Coin>();
        
        while (amount > 0)
        {
            var candidates = _coinPrefabsSortedByValue.Where((cn) => cn.value <= amount);
            if (!candidates.Any()) break;

            var coin = candidates.First();
            results.Add(coin);
            amount -= coin.value;
        }

        return results;
    }

    public void Load(RawCoinSpawnControllerData data)
    {
        ClearBoard();
        
        foreach (var rawCoinData in data.coins)
        {
            //var coin = CreateCoin();
            //coin.Load(rawCoinData);
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
