using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class CommandStructureWindow : EditorWindow
{
    private class RawCommandAndTypeCollection
    {
        public ICommandData commandData;
        public CommandType type;
    }
    
    private Vector2 _scroll;

    [MenuItem("Tools/View Command Structure")]
    static void Init()
    {
        var window = EditorWindow.GetWindow(typeof(CommandStructureWindow));
        window.Show();
    }

    RawCommands MockRawCommands()
    {
        var mocks = new RawCommands();
        mocks.commands = new List<RawCommand>();
        
        foreach (var commandAndTypeCollection in CreateMockCommandDatas())
        {
            var rawCommand = new RawCommand();
            rawCommand.data = JsonConvert.SerializeObject(commandAndTypeCollection.commandData);
            rawCommand.type = commandAndTypeCollection.type.ToString();
            mocks.commands.Add(rawCommand);
        }

        return mocks;
    }

    IEnumerable<RawCommandAndTypeCollection> CreateMockCommandDatas()
    {
        var commands = new List<RawCommandAndTypeCollection>();

        commands.Add(new RawCommandAndTypeCollection()
        {
            commandData = new QueueCoinCommand.Data(),
            type = CommandType.QUEUE_COIN
        });
        
        commands.Add(new RawCommandAndTypeCollection()
        {
            commandData = new ClearBoardCommand.Data(),
            type = CommandType.CLEAR_BOARD
        });
        
        return commands;
    }
    
    void OnGUI()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        string json = JsonConvert.SerializeObject(MockRawCommands(), Formatting.Indented);
        EditorGUILayout.TextArea(json, GUILayout.Height(position.height));
        EditorGUILayout.EndScrollView();
    }
}
