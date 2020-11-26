using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Zenject;

public class LocalizationService
{
    [Serializable]
    public sealed class Settings
    {
    }

    private readonly Settings _settings;
    private CultureInfo _culture;

    private CampaignModel _campaignModel;

    public event Action LocaleUpdatedEvent;
    
    [Inject]
    public LocalizationService(Settings settings, CampaignModel campaignModel)
    {
        _settings = settings;
        _campaignModel = campaignModel;
        _campaignModel.OnUpdated += OnCampaignUpdated;
    }

    private void OnCampaignUpdated()
    {
        SetCurrency(_campaignModel.Currency);
    }
    
    public void SetCurrency(string currencyName)
    { 
        var newCulture = CultureInfo.GetCultures(CultureTypes.SpecificCultures).FirstOrDefault(x => new RegionInfo(x.LCID).ISOCurrencySymbol == currencyName);

        if (newCulture != _culture)
        {
            LocaleUpdatedEvent?.Invoke();
        }

        _culture = newCulture;
        
        Debug.Assert(_culture != null, $"Could not get culture for currency {currencyName}");
        
        Debug.Log($"Matched first culture with currency {currencyName}: {_culture}");
    }

    public string LocalizeMoney(in float money)
    {
        return money.ToString("C", _culture);
    }
}
