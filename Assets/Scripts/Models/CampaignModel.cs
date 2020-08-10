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
    public string Currency;
}
public class CampaignModel: ISaveLoadable<RawCampaignModel>
{
    private CoinSpawnController _spawnController;
    
    public event Action OnUpdated;
    
    public string CauseName { get; set; }
    public string CauseProfileURL { get; set; }
    public float TotalRaised { get; set; }
    public float Goal { get; set; }

    public float Raised => TotalRaised;

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Currency { get; set; }

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
        Currency = data.Currency;
        
        if (String.IsNullOrEmpty(data.EndDate))
        {
            EndDate = null;
        }
        else
        {
            EndDate = DateTime.Parse(data.EndDate);
        }
    }
}
