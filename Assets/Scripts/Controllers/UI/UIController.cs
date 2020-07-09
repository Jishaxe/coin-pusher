using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UIController : MonoBehaviour
{
    [SerializeField] private MoneyUIElementController _raisedAmountController; 
    [SerializeField] private MoneyUIElementController _goalAmountController;
    [SerializeField] private ProgressBarController _progressBarController;
    [SerializeField] private StartEndTimeUIElementController _startEndTimeController;
    [SerializeField] private CauseInfoUIElementController _causeInfoController;

    private float _lastRaised = 0f;
    private CampaignModel _campaignModel;
    
    [Inject]
    void Construct(CampaignModel campaignModel)
    {
        _campaignModel = campaignModel;
        _campaignModel.OnUpdated += OnCampaignModelUpdated;
    }

    void OnCampaignModelUpdated()
    {
        _causeInfoController.SetCauseInfo(_campaignModel.CauseProfileURL, _campaignModel.CauseName);
        _startEndTimeController.SetDates(_campaignModel.StartDate, _campaignModel.EndDate);
        _goalAmountController.SetMoney(_campaignModel.Goal, true);

        float raised = _campaignModel.Raised;

        if (raised > _lastRaised)
        {
            _raisedAmountController.SetMoney(raised);
            _progressBarController.SetValue(raised / _campaignModel.Goal);
        }
        else
        {
            // if we've reduced the money count then skip animations
            _raisedAmountController.SetMoney(raised, true);
            _progressBarController.SetValue(raised / _campaignModel.Goal, true);
        }

        _lastRaised = raised;
    }

    void OnDestroy()
    {
        _campaignModel.OnUpdated -= OnCampaignModelUpdated;
    }
}
