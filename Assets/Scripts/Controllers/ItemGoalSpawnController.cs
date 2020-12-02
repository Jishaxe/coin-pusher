using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = System.Random;

public class ItemGoalSpawnController: MonoBehaviour, ISaveLoadable<RawItemGoalSpawnControllerData>
{

    private Settings _settings;
    private BoardController _boardController;
    private BoardItemsModel _boardItems;

    [Serializable]
    public sealed class Settings
    {
        public List<ItemGoal> itemGoalPrefabs;
        public Vector3 randomCoinDropOffsetFromCenter;
    }

    [Inject]
    public void Construct(Settings settings, BoardController boardController, BoardItemsModel boardItemsModel)
    {
        _settings = settings;
        _boardController = boardController;
        _boardItems = boardItemsModel;
    }

    private Vector3 GetCoinDropPosition()
    {
        Vector3 offset = Vector3.Lerp(-_settings.randomCoinDropOffsetFromCenter,
            _settings.randomCoinDropOffsetFromCenter, UnityEngine.Random.value);

        return _boardController.ItemSpawnPosition + offset;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            var newItem = _boardController.SpawnItemFromData(new RawItemGoalData
            {
                itemGoalType = ItemGoalType.BIG_COIN
            });

            newItem.transform.position = GetCoinDropPosition();
        }
    }

    public RawItemGoalSpawnControllerData Save()
    {
        return new RawItemGoalSpawnControllerData();
    }

    public void Load(RawItemGoalSpawnControllerData data)
    {
        // nothing yet
    }
}

public class RawItemGoalSpawnControllerData
{
    
}
