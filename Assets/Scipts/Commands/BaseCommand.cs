using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

public enum CommandType
{
    QUEUE_COIN,
    CLEAR_BOARD
}

public class RawCommand
{
    public string type;
    public string id;
    public double time;
    public string data;
}

public interface ICommandData
{
    
}

public class CommandFactory : IFactory<RawCommand, BaseCommand>
{
    private readonly DiContainer _container;

    [Inject]
    public CommandFactory(DiContainer container)
    {
        _container = container;
    }
    
    public BaseCommand Create(RawCommand rawCommand)
    {
        BaseCommand baseCommand = null;

        var type = CommandUtils.GetCommandTypeFromString(rawCommand.type);
        
        switch (type)
        {
            case CommandType.QUEUE_COIN:
                baseCommand = _container.Instantiate<QueueCoinCommand>();
                break;
            case CommandType.CLEAR_BOARD:
                baseCommand = _container.Instantiate<ClearBoardCommand>();
                break;
        }

        Debug.Assert(baseCommand != null, "Couldn't match command " + rawCommand.type);
        baseCommand.Initialize(rawCommand.id, rawCommand.time);
        baseCommand.Load(rawCommand.data);
        return baseCommand;
    }
}

public class BaseCommand
{
    protected string _id;
    protected double _time;
    protected ICommandData _data;
    
    public sealed class Factory : PlaceholderFactory<RawCommand, BaseCommand>
    {
        
    }
    
    [Inject]
    public BaseCommand()
    {
        
    }

    public void Initialize(string id, double time)
    {
        _id = id;
        _time = time;
    }
    
    public void Acknowledge()
    {
        
    }

    public virtual void Load(string data)
    {
        throw new NotImplementedException();
    }

    protected void LoadCommandData<T>(string data) where T: ICommandData
    {
        _data = JsonConvert.DeserializeObject<T>(data);
    }
    
    public virtual void Invoke()
    {
        throw new NotImplementedException();
    }
}

public static class CommandUtils
{
    public static CommandType GetCommandTypeFromString(in string text)
    {
        CommandType type;
        if (!CommandType.TryParse(text, out type))
        {
            throw new ArgumentException("Invalid CommandType: " + text);
        }
        
        return type;
    }
}