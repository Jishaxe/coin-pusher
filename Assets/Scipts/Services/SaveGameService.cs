using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

public class SaveGameData
{
    public RawCoinSpawnControllerData CoinSpawnController;
    public RawPusherControllerData PusherController;
}

public class SaveGameService
{
    private const string k_nosave = "nosave";
    private const string k_savedatakey = "SaveData";
    
    private readonly CoinSpawnController _coinSpawnController;
    private readonly PusherController _pusherController;
    
    [Inject]
    public SaveGameService(CoinSpawnController coinSpawnController, PusherController pusherController)
    {
        _coinSpawnController = coinSpawnController;
        _pusherController = pusherController;
    }

    public void SaveGame()
    {
        var saveGame = new SaveGameData();
        saveGame.CoinSpawnController = _coinSpawnController.Save();
        saveGame.PusherController = _pusherController.Save();
        SaveJson(JsonConvert.SerializeObject(saveGame));
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
        
        Debug.Log("Saved data: " + json);
    }

    public void LoadGame()
    {
        if (!TryGetJson(out var json))
        {
            Debug.Log("No savedata found, skipping...");
            return;
        }
        
        var saveGame = JsonConvert.DeserializeObject<SaveGameData>(json);
        _coinSpawnController.Load(saveGame.CoinSpawnController);
        _pusherController.Load(saveGame.PusherController);
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
