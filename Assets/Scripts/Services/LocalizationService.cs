using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Zenject;

public class LocalizationService
{
    [Serializable]
    public sealed class Settings
    {
        public string Locale;
    }

    private readonly Settings _settings;
    private CultureInfo _culture;
    
    [Inject]
    public LocalizationService(Settings settings)
    {
        _settings = settings;
        _culture = CultureInfo.CreateSpecificCulture(_settings.Locale);
    }

    public string LocalizeMoney(in float money)
    {
        return money.ToString("C", _culture);
    }
}
