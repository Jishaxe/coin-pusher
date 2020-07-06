using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CauseInfoUIElementController : MonoBehaviour
{
    [SerializeField] private Text _causeNameText;
    [SerializeField] private Image _causeImage;

    public void SetCauseInfo(string imageURL, string name)
    {
        _causeNameText.text = name;
    }
}
