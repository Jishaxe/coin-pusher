using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class QueueDonationCommand : BaseCommand
{
    private CoinSpawnController _coinSpawnController;
    private Data _data;
    
    public class Data: ICommandData
    {
        public string name;
        public string message;
        public string profileURL;
        public float amount;
    }

    [Inject]
    public QueueDonationCommand(CoinSpawnController coinSpawnController)
    {
        _coinSpawnController = coinSpawnController;
    }
    
    public override void Load(string data)
    {
        _type = CommandType.QUEUE_DONATION;
        _data = LoadCommandData<Data>(data);
    }

    public override void Invoke()
    {
        _coinSpawnController.QueueDonation(_data.name, _data.message, _data.profileURL, _data.amount);
    }
}
