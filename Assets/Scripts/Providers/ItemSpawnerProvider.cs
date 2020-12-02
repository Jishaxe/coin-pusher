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
    private ItemGoalSpawnController.Settings _itemGoalSpawnerSettings;
    
    private Item.Factory _itemFactory;
    
    private List<Coin> _coinPrefabsSortedByValue = new List<Coin>();
    private Dictionary<ItemGoalType, ItemGoal> _itemGoalsByType = new Dictionary<ItemGoalType, ItemGoal>();
    
    public List<Coin> CoinPrefabsSortedByValue => _coinPrefabsSortedByValue;
    
    [Inject]
    private void Construct(Item.Factory itemFactory, CoinSpawnController.Settings coinSpawnerSettings, ItemGoalSpawnController.Settings itemGoalSpawnerSettings)
    {
        _itemFactory = itemFactory;
        _coinSpawnerSettings = coinSpawnerSettings;
        _itemGoalSpawnerSettings = itemGoalSpawnerSettings;
        
        _itemPrefabCreatorMappings = new Dictionary<Type, Func<RawItemData, Item>>()
        {
            [typeof(RawCoinData)] = CreatePrefab_FromCoin,
            [typeof(RawItemGoalData)] = CreatePrefab_FromItemGoal
        };
        
        ProcessCoinPrefabs();
        ProcessItemGoalPrefabs();
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
        _coinPrefabsSortedByValue = _coinSpawnerSettings.coinPrefabs.OrderByDescending(coin => coin.Value).ToList();
        Debug.Assert(!_coinPrefabsSortedByValue.Any((coin) => coin.Value == 0), "Coin has zero value");
    }
    
    private Coin GetCoinPrefabFromValue(float value)
    {
        return _coinPrefabsSortedByValue.FirstOrDefault((coin) => Mathf.Approximately(value, coin.Value));
    }
    
    private Item CreatePrefab_FromCoin(RawItemData item)
    {
        var coinData = (RawCoinData) item;
        var prefab = GetCoinPrefabFromValue(coinData.value);

        var createdCoin = _itemFactory.Create(prefab);
        
        createdCoin.Load(coinData);
        return createdCoin;
    }
    #endregion

    #region ItemGoal spawning

    private void ProcessItemGoalPrefabs()
    {
        _itemGoalsByType = _itemGoalSpawnerSettings.itemGoalPrefabs.ToDictionary(i => i.ItemGoalType, i => i);
    }
    
    private Item CreatePrefab_FromItemGoal(RawItemData item)
    {
        var itemData = (RawItemGoalData) item;
        var prefab = _itemGoalsByType[itemData.itemGoalType];

        var createdItemGoal = _itemFactory.Create(prefab);
        
        createdItemGoal.Load(itemData);
        return createdItemGoal;
    }

    #endregion
}
