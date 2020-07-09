using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class RawCampaignModel
{
    public string CauseName;
    public string CauseProfileURL;
    public float TotalRaised;
    public float Goal;
    public string StartDate;
    public string EndDate;
}
public class CampaignModel: ISaveLoadable<RawCampaignModel>
{
    private CoinSpawnController _spawnController;
    
    public event Action OnUpdated;
    
    public string CauseName { get; set; }
    public string CauseProfileURL { get; set; }
    public float TotalRaised { get; set; }
    public float Goal { get; set; }

    public float Raised
    {
        get
        {
            return TotalRaised - _spawnController.ValueOnBoard;
        }
    }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [Inject]
    void Construct(CoinSpawnController spawnController)
    {
        _spawnController = spawnController;
    }
    
    public void Update()
    {
        OnUpdated?.Invoke();
    }

    public RawCampaignModel Save()
    {
        throw new NotImplementedException();
    }

    public void Load(RawCampaignModel data)
    {
        CauseName = data.CauseName;
        CauseProfileURL = data.CauseProfileURL;
        TotalRaised = data.TotalRaised;
        Goal = data.Goal;
        StartDate = DateTime.Parse(data.StartDate);
        EndDate = DateTime.Parse(data.EndDate);
    }
}
