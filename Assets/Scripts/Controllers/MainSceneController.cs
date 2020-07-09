using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MainSceneController: IInitializable, ITickable
{
    private CoinSpawnController _coinSpawnController;
    private SaveGameService _saveGameService;
    private RemoteController _remoteController;
    
    [Inject]
    public MainSceneController(CoinSpawnController coinSpawnController, SaveGameService saveGameService, RemoteController remoteController)
    {
        _coinSpawnController = coinSpawnController;
        _saveGameService = saveGameService;
        _remoteController = remoteController;
    }

    public void Initialize()
    {
        _saveGameService.LoadGame();
        _remoteController.StartPolling();
        
        //_coinSpawnController.PopulateBoard(9.99f);
        
        Application.quitting += Shutdown;
    }


    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _coinSpawnController.ClearBoard();
            _coinSpawnController.PopulateBoard(53.55f);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            _saveGameService.ClearSave();
        }
    }

    public void Shutdown()
    {
        _saveGameService.SaveGame();
        _remoteController.StopPolling();
    }
}
