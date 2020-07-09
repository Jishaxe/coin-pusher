using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Zenject;

public class CommandController: ISaveLoadable<RawCommandController>
{
    private List<BaseCommand> _commands = new List<BaseCommand>();
    private BaseCommand.Factory _commandFactory;
    private CommandDeduplicator _commandDeduplicator;
        
    [Inject]
    void Construct(BaseCommand.Factory commandFactory)
    {
        _commandFactory = commandFactory;
        _commandDeduplicator = new CommandDeduplicator(_commands);
    }
    
    public void AddRawCommands(List<RawCommand> rawCommands)
    {
        if (!rawCommands.Any()) return;
        
        var newCommands = new List<BaseCommand>();

        var deduplicatedCommands = _commandDeduplicator.RemoveDuplicates(rawCommands);
        
        foreach (var rawCommand in deduplicatedCommands)
        {
            newCommands.Add(_commandFactory.Create(rawCommand));
        }
        
        _commands.AddRange(newCommands);
        
        SortCommandsByTime();
        ProcessCommands();
    }

    private void SortCommandsByTime()
    {
        // puts oldest commands at the top
        _commands = _commands.OrderBy(command => command.Time).ToList();
    }
    
    private BaseCommand GetCommandByID(string id)
    {
        return _commands.FirstOrDefault(
            command => command.ID == id);
    }

    public RawCommandController Save()
    {
        var rawData = new RawCommandController();

        rawData.commands = _commands.Select(command => command.Save()).ToList();
        rawData.RawCommandDeduplicator = _commandDeduplicator.Save();
        return rawData;
    }

    public void Load(RawCommandController data)
    {
        AddRawCommands(data.commands);
        _commandDeduplicator.Load(data.RawCommandDeduplicator);
        ProcessCommands();
    }

    void ProcessCommands()
    {
        if (_commands.Any())
        {
            foreach (var command in _commands)
            {
                command.Invoke();
            }
            
            _commandDeduplicator.AddToDuplicateHistory(_commands);
            _commands.Clear();
        }
    }
}

public sealed class CommandDeduplicator: ISaveLoadable<RawCommandDeduplicator>
{
    private const int maxDuplicateHistory = 100;
    
    private List<BaseCommand> _commands;
    private List<string> _idHistory;
    
    public CommandDeduplicator(List<BaseCommand> commandList)
    {
        _commands = commandList;
        _idHistory = new List<string>();
    }

    public IEnumerable<RawCommand> RemoveDuplicates(List<RawCommand> rawCommands)
    {
        return rawCommands.FindAll(rawCommand => !IsDuplicateID(rawCommand.id));
    }

    public void AddToDuplicateHistory(List<BaseCommand> commands)
    {
        foreach (BaseCommand command in commands)
        {
            AddIDToDuplicateHistory(command.ID);
        }
    }

    private void AddIDToDuplicateHistory(string id)
    {
        _idHistory.Add(id);
        if (_idHistory.Count > maxDuplicateHistory) _idHistory.RemoveAt(0);
    }

    public bool IsDuplicateID(string id)
    {
        return _commands.Any(command => command.ID == id) || _idHistory.Any(historicID => historicID == id);
    }

    public RawCommandDeduplicator Save()
    {
        var raw = new RawCommandDeduplicator();
        raw.idHistory = _idHistory;
        return raw;
    }

    public void Load(RawCommandDeduplicator data)
    {
        foreach (string id in data.idHistory)
        {
            AddIDToDuplicateHistory(id);
        }
    }
}

public class RawCommandController
{
    public List<RawCommand> commands = new List<RawCommand>();
    public RawCommandDeduplicator RawCommandDeduplicator;
}

public sealed class RawCommandDeduplicator
{
    public List<string> idHistory = new List<string>();
}
