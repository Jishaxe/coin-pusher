using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class RawCommands
{
    public List<RawCommand> commands;
}

public class CommandController
{
    private BaseCommand.Factory _commandFactory;
        
    [Inject]
    void Construct(BaseCommand.Factory commandFactory)
    {
        _commandFactory = commandFactory;
    }
    
    public void HandleRawCommands(RawCommands rawCommands)
    {
        var commands = new List<BaseCommand>();

        foreach (var rawCommand in rawCommands.commands)
        {
            commands.Add(_commandFactory.Create(rawCommand));
        }
    }
}
