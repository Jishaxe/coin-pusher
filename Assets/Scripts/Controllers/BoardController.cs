using System;
using System.CodeDom;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class BoardController: ISaveLoadable<RawBoardControllerData>
{
    private Settings _settings;
    private BoardItemsModel _itemsModel;
    private ItemSpawnerProvider _itemSpawnerProvider;

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

    public Item SpawnItemFromData(RawItemData itemData)
    {
        var spawnedItem = _itemSpawnerProvider.SpawnItem(itemData);
        _itemsModel.Items.Add(spawnedItem);

        return spawnedItem;
    }

    public void RemoveItem(Item item, bool destroy = true)
    {
        _itemsModel.Items.Remove(item);

        if (destroy)
        {
            GameObject.Destroy(item.gameObject);
        }
    }

    private void ClearBoard()
    {
        foreach (var item in _itemsModel.Items)
        {
            GameObject.Destroy(item.gameObject);
        }

        _itemsModel.Items.Clear();
    }
    
    [Inject]
    public BoardController(Settings settings, BoardItemsModel itemsModel, ItemSpawnerProvider itemSpawnerProvider)
    {
        this._settings = settings;
        this._itemsModel = itemsModel;
        this._itemSpawnerProvider = itemSpawnerProvider;
    }

    public RawBoardControllerData Save()
    {
        RawBoardControllerData saveData = new RawBoardControllerData();
        saveData.Items = new List<RawItemData>();
        
        foreach (var item in _itemsModel.Items)
        {
            var savedData = item.Save();
            saveData.Items.Add(savedData);
        }

        return saveData;
    }

    public void Load(RawBoardControllerData data)
    {
        ClearBoard();

        foreach (var rawItemData in data.Items)
        {
            var newItem = SpawnItemFromData(rawItemData);
            newItem.Initialize();
        }
    }
}

public sealed class RawBoardControllerData
{
    public List<RawItemData> Items;
}