using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

public class SaveGameData
{
    public RawCoinSpawnControllerData CoinSpawnController;
    public RawPusherControllerData PusherController;
    public RawCommandController CommandController;
}

public class SaveGameService
{
    private const string k_nosave = "nosave";
    private const string k_savedatakey = "SaveData";
    
    private readonly CoinSpawnController _coinSpawnController;
    private readonly PusherController _pusherController;
    private readonly CommandController _commandController;
    
    [Inject]
    public SaveGameService(CoinSpawnController coinSpawnController, PusherController pusherController, CommandController commandController)
    {
        _coinSpawnController = coinSpawnController;
        _pusherController = pusherController;
        _commandController = commandController;
    }

    public void SaveGame()
    {
        var saveGame = new SaveGameData();
        saveGame.CoinSpawnController = _coinSpawnController.Save();
        saveGame.PusherController = _pusherController.Save();
        saveGame.CommandController = _commandController.Save();
        SaveJson(JsonConvert.SerializeObject(saveGame, Formatting.Indented));
    }
    
    public void ClearSave()
    {
        PlayerPrefs.DeleteKey(k_savedatakey);
        Debug.Log("Cleared save!");
    }
    
    private void SaveJson(in string json)
    {
        PlayerPrefs.SetString(k_savedatakey, json);
        PlayerPrefs.Save();
        
        string path = "savedataview.json";
        
        StreamWriter writer = new StreamWriter(path, false);
        writer.Write(json);
        writer.Close();
    }

    public void LoadGame()
    {
        if (!TryGetJson(out var json))
        {
            Debug.Log("No savedata found, skipping...");
            return;
        }
        
        var saveGame = JsonConvert.DeserializeObject<SaveGameData>(json);
        
        // TODO: Check for null here
        if (saveGame.CoinSpawnController != null) _coinSpawnController.Load(saveGame.CoinSpawnController);
        if (saveGame.PusherController != null) _pusherController.Load(saveGame.PusherController);
        if (saveGame.CommandController != null) _commandController.Load(saveGame.CommandController);
    }

    private bool TryGetJson(out string json)
    {
        json = PlayerPrefs.GetString(k_savedatakey, k_nosave);
        if (json == k_nosave)
        {
            json = null;
            return false;
        }
        else
        {
            return true;
        }
    }
}
