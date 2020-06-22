using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueCoinCommand : BaseCommand
{
    public class Data: ICommandData
    {
        public string name;
    }
    public override void Load(string data)
    {
        LoadCommandData<Data>(data);
    }

    public override void Invoke()
    {
        
    }
}
