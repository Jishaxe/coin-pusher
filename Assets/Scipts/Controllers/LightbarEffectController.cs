using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LightbarEffectController: ITickable
{
    private readonly Settings _settings;
    private LightbarEffectDefinition _currentEffect;
    
    [Serializable]
    public class Settings
    {
        public Material lightbarMaterial;
        public LightbarEffectDefinition lightbarEffect;
    }
    
    [Inject]
    public LightbarEffectController(Settings settings)
    {
        _settings = settings;
        PlayEffect(settings.lightbarEffect);
    }

    public void PlayEffect(LightbarEffectDefinition definition)
    {
        _currentEffect = definition;
        _settings.lightbarMaterial.mainTextureScale = _currentEffect.tiling;
    }

    public void Tick()
    {
        _settings.lightbarMaterial.mainTextureOffset += _currentEffect.offsetSpeed * Time.deltaTime;
    }
}
