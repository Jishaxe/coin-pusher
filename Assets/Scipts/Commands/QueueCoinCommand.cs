using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class QueueCoinCommand : BaseCommand
{
    private CoinSpawnController _coinSpawnController;
    public class Data: ICommandData
    {
        public string name;
    }

    [Inject]
    public QueueCoinCommand(CoinSpawnController coinSpawnController)
    {
        _coinSpawnController = coinSpawnController;
    }
    
    public override void Load(string data)
    {
        _type = CommandType.QUEUE_COIN;
        LoadCommandData<Data>(data);
    }

    public override void Invoke()
    {
        _coinSpawnController.QueueCoin();
    }
}
