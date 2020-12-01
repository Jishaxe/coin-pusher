using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = System.Random;

public sealed class BoardItemsModel: ISaveLoadable<RawBoardItemsModelData>
{
    public List<Item> Items => _items;
    public IEnumerable<Coin> Coins => _items.OfType<Coin>();
    
    private List<Item> _items = new List<Item>();
    
    public RawBoardItemsModelData Save()
    {
        var data = new RawBoardItemsModelData();

        foreach (Item item in _items)
        {
            var rawCoinData = item.Save();
            data.items.Add(rawCoinData);
        }

        return data;
    }

    public void Load(RawBoardItemsModelData data)
    {
        throw new NotImplementedException();
    }
}

public sealed class RawBoardItemsModelData
{
    public List<RawItemData> items;
}
