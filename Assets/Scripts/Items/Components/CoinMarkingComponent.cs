using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using Zenject;

public class CoinMarkingComponent : MonoBehaviour
{
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private int _normalMaterialSlot;
    [SerializeField] private int _markedMaterialSlot;
    
    private ImageProvisionService _imageProvisionService;
    private CoinMarkingService _coinMarkingService;
    private Material _baseMaterial;
    private Material _markedMaterial;
    
    [Inject]
    public void Construct(ImageProvisionService imageProvisionService, CoinMarkingService coinMarkingService)
    {
        _imageProvisionService = imageProvisionService;
        _coinMarkingService = coinMarkingService;
    }
    
    public void ApplyMarking(string imageURL)
    {
        _imageProvisionService.ResolveImage(imageURL, SetMarkedTexture);
    }

    private void SetMarkedTexture(Texture2D original)
    {
        if (original == null) return;

        _baseMaterial = _renderer.sharedMaterials[_normalMaterialSlot];
        _markedMaterial = _coinMarkingService.GetMarkedMaterial(_baseMaterial, original);
        var mats = _renderer.materials;
        mats[_markedMaterialSlot] = _markedMaterial;
        _renderer.materials = mats;
    }
    
    private void Start()
    {
        
    }
}
