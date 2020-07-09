using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

public enum CommandType
{
    NONE,
    QUEUE_DONATION,
    CLEAR_BOARD
}

public class RawCommand
{
    public string type;
    public string id;
    public string time;
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
            case CommandType.QUEUE_DONATION:
                baseCommand = _container.Instantiate<QueueDonationCommand>();
                break;
            case CommandType.CLEAR_BOARD:
                baseCommand = _container.Instantiate<ClearBoardCommand>();
                break;
        }

        Debug.Assert(baseCommand != null, "Couldn't match command " + rawCommand.type);

        DateTime time = CommandUtils.ParseUTCTimeString(rawCommand.time);
        
        baseCommand.Initialize(rawCommand.id, time);
        baseCommand.Load(rawCommand.data);
        
        return baseCommand;
    }
}

public class BaseCommand
{
    public string ID
    {
        get;
        protected set;
    }

    public DateTime Time
    {
        get;
        protected set;
    }
    
    private ICommandData _data;
    protected CommandType _type;

    private Type _dataType;
    
    public sealed class Factory : PlaceholderFactory<RawCommand, BaseCommand>
    {
        
    }
    
    [Inject]
    public BaseCommand()
    {
        
    }

    public void Initialize(string id, DateTime time)
    {
        ID = id;
        Time = time;
    }
    
    public void Acknowledge()
    {
        
    }

    public virtual void Load(string data)
    {
        throw new NotImplementedException();
    }

    public RawCommand Save()
    {
        var raw = new RawCommand();
        raw.id = ID;
        raw.time = CommandUtils.DateTimeToUTCString(Time);
        raw.data = JsonConvert.SerializeObject(_data);
        raw.type = _type.ToString();
        return raw;
    }

    protected T LoadCommandData<T>(string data) where T: ICommandData
    {
        _data = JsonConvert.DeserializeObject<T>(data);
        return (T) this._data;
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

    public static DateTime ParseUTCTimeString(in string utcString)
    {
        return DateTime.Parse(utcString);
    }

    public static string DateTimeToUTCString(DateTime time)
    {
        return time.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
    }
}