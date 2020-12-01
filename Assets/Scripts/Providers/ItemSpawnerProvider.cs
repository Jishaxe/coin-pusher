using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using Zenject;

public sealed class ItemSpawnerProvider
{
    private Dictionary<Type, Func<RawItemData, Item>> _itemPrefabCreatorMappings;

    private CoinSpawnController.Settings _coinSpawnerSettings;
    private Coin.Factory _coinFactory;
    private List<Coin> _coinPrefabsSortedByValue = new List<Coin>();

    public List<Coin> CoinPrefabsSortedByValue => _coinPrefabsSortedByValue;
    
    [Inject]
    private void Construct(Coin.Factory coinFactory, CoinSpawnController.Settings coinSpawnerSettings)
    {
        _coinFactory = coinFactory;
        _coinSpawnerSettings = coinSpawnerSettings;
        
        _itemPrefabCreatorMappings = new Dictionary<Type, Func<RawItemData, Item>>()
        {
            [typeof(RawCoinData)] = CreatePrefab_FromCoin
        };
        
        ProcessCoinPrefabs();
    }
    
    
    public Item SpawnItem(RawItemData itemData)
    {
        if (_itemPrefabCreatorMappings.TryGetValue(itemData.GetType(), out var SpawnerMethod))
        {
            return SpawnerMethod(itemData);
        }
        else
        {
            Log.Error($"Couldn't find spawner method for {itemData.GetType()}");
            return null;
        }
    }
    
    
    #region Coin spawning
    private void ProcessCoinPrefabs()
    {
        _coinPrefabsSortedByValue = _coinSpawnerSettings.coinPrefabs.OrderByDescending(coin => coin.value).ToList();
        Debug.Assert(!_coinPrefabsSortedByValue.Any((coin) => coin.value == 0), "Coin has zero value");
    }
    
    private Coin GetCoinPrefabFromValue(float value)
    {
        return _coinPrefabsSortedByValue.FirstOrDefault((coin) => Mathf.Approximately(value, coin.value));
    }
    
    private Item CreatePrefab_FromCoin(RawItemData item)
    {
        var coinData = (RawCoinData) item;
        var prefab = GetCoinPrefabFromValue(coinData.value);

        var createdCoin = _coinFactory.Create(prefab);
        
        createdCoin.Load(coinData);
        return createdCoin;
    }
    #endregion
}
