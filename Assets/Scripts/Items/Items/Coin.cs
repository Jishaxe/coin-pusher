using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class Coin : Item
{
    private CoinMarkingComponent _coinMarkingComponent;
    [FormerlySerializedAs("value")] public float Value;
    private string _markingURL;

    public override void Initialize()
    {
        base.Initialize();
        _coinMarkingComponent = GetComponent<CoinMarkingComponent>();
        ApplyMarking();
    }

    public override RawItemData Save()
    {
        var rawCoinData = new RawCoinData();
        rawCoinData.markingURL = _markingURL;
        rawCoinData.value = Value;

        SaveBase(rawCoinData);
        
        return rawCoinData;
    }

    public override void Load(RawItemData rawData)
    {
        LoadBase(rawData);
        var data = (RawCoinData) rawData;
        _markingURL = data.markingURL;
    }

    public void ApplyMarking(string markingURL)
    {
        _markingURL = markingURL;
        _coinMarkingComponent.ApplyMarking(markingURL);
    }

    public void ApplyMarking()
    {
        _coinMarkingComponent.ApplyMarking(_markingURL);
    }
}

public class RawCoinData: RawItemData
{
    public string markingURL;
    public float value;
}

