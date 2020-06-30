using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class CoinSpawnController: MonoBehaviour, ISaveLoadable<RawCoinSpawnControllerData>
{
    public delegate Vector3 CoinSpawnPositionProvider();
    private const int k_populateBoardWaves = 20;
    private const float k_populateBoardDelay = 0;
    
    private Settings _settings;
    private Coin.Factory _coinFactory;
    private BoardController _boardController;
    private PusherController _pusherController;

    private List<Coin> _coins = new List<Coin>();
    private List<Coin> _coinPrefabsSortedByValue = new List<Coin>();

    private bool _isPopulating = false;
    
    public float ValueOnBoard
    {
        get
        {
            return _coins.Sum(coin => coin.value);
        }
    }
    
    [Serializable]
    public sealed class Settings
    {
        public List<Coin> coinPrefabs;
        public Vector3 randomCoinDropOffsetFromCenter;
        public float distanceBetweenCoins;
        public float coinRadius;
        public bool populateOnStart;
    }

    [Inject]
    public void Construct(Settings settings, Coin.Factory coinFactory, BoardController boardController, PusherController pusherController)
    {
        _settings = settings;
        _coinFactory = coinFactory;
        _boardController = boardController;
        _pusherController = pusherController;
        ProcessCoinPrefabs();

        Coin.OnCoinCollected += OnCoinCollected;
    }

    private void OnCoinCollected(Coin coin)
    {
        _coins.Remove(coin);
    }
    
    private void ProcessCoinPrefabs()
    {
        _coinPrefabsSortedByValue = _settings.coinPrefabs.OrderByDescending(coin => coin.value).ToList();
        Debug.Assert(!_coinPrefabsSortedByValue.Any((coin) => coin.value == 0), "Coin has zero value");
    }

    public void PopulateBoard(float value)
    {
        if (_isPopulating) return;
        _isPopulating = true;

        StartCoroutine(PopulateBoardCoroutine(value));
    }

    private IEnumerator PopulateBoardCoroutine(float value)
    {
        float valuePerWave = value / k_populateBoardWaves;
        for (int i = 0; i < k_populateBoardWaves; i++)
        {
            QueueDonation("Backlog", "", valuePerWave, _boardController.GetRandomPopulationPosition);
            yield return new WaitForSeconds(k_populateBoardDelay);
        }
        
        _isPopulating = false;
        Debug.Log("Populuated, value on board: " + ValueOnBoard);
    }

    public void ClearBoard()
    {
        foreach (Coin coin in _coins)
        {
            coin.Destroy();
        }
        
        _coins.Clear();
    }

    private Vector3 GetCoinDropPosition()
    {
        Vector3 offset = Vector3.Lerp(-_settings.randomCoinDropOffsetFromCenter,
            _settings.randomCoinDropOffsetFromCenter, UnityEngine.Random.value);

        return _boardController.CoinSpawnPosition + offset;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            QueueDonation("Test Donation", "Test Message", 3.99f);
        }
    }

    public void QueueDonation(string name, string message, float amount, CoinSpawnPositionProvider positionProvider)
    {
        Debug.Log($"Queuing donation, name: {name}, message: {message}, amount: {amount}");
        var coins = BreakDownBalanceIntoCoins(amount);

        foreach (Coin coin in coins)
        {
            var newCoin = _coinFactory.Create(coin);
            newCoin.transform.position = positionProvider();
            _coins.Add(newCoin);
        }
    }

    public void QueueDonation(string name, string message, float amount)
    {
        QueueDonation(name, message, amount, GetCoinDropPosition);
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
