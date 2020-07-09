using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CauseInfoUIElementController : MonoBehaviour
{
    [SerializeField] private Text _causeNameText;
    [SerializeField] private RawImage _causeImage;

    private ImageProvisionService _imageProvisionService;
    
    [Inject]
    void Construct(ImageProvisionService imageProvisionService)
    {
        _imageProvisionService = imageProvisionService;
    }
    
    public void SetCauseInfo(string imageURL, string name)
    {
        _causeNameText.text = name;
        _imageProvisionService.ResolveImage(imageURL, SetImage);
    }

    private void SetImage(Texture2D image)
    {
        if (image != null) _causeImage.texture = image;
        else _causeImage.color = Color.magenta;
    }
}
