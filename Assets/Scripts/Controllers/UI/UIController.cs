using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class UIController : MonoBehaviour
{
    [SerializeField] private MoneyUIElementController _raisedAmountController; 
    [SerializeField] private MoneyUIElementController _goalAmountController;
    [SerializeField] private ProgressBarController _progressBarController;
    [SerializeField] private StartEndTimeUIElementController _startEndTimeController;
    [SerializeField] private CauseInfoUIElementController _causeInfoController;
    [SerializeField] private DonationBubbleUIElementController _donationBubbleUiElementController;

    [Space(30)] [SerializeField] private float _holdDonationBubbleSeconds;
    
    private float _lastRaised = 0f;
    private CampaignModel _campaignModel;
    private CoinSpawnController _coinSpawnController;
    
    private Queue<CoinSpawnController.DonationEventData> _presentDonationQueue = new Queue<CoinSpawnController.DonationEventData>();
    private bool _isProcessingDonationQueue;
    
    [Inject]
    void Construct(CampaignModel campaignModel, CoinSpawnController coinSpawnController)
    {
        _campaignModel = campaignModel;
        _campaignModel.OnUpdated += OnCampaignModelUpdated;

        _coinSpawnController = coinSpawnController;
        _coinSpawnController.OnDonationMade += OnDonationMade;
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

    public void OnDonationMade(CoinSpawnController.DonationEventData donationEventData)
    {
        if (donationEventData.ShouldShow == false) return;
        
        _presentDonationQueue.Enqueue(donationEventData);

        if (!_isProcessingDonationQueue)
        {
            StartCoroutine(ProcessDonationQueue());
        }
    }

    IEnumerator ProcessDonationQueue()
    {
        _isProcessingDonationQueue = true;
        
        while (_presentDonationQueue.Any())
        {
            var donation = _presentDonationQueue.Dequeue();
            _donationBubbleUiElementController.SetData(donation.Name, donation.Message, donation.ProfileURL, donation.Amount);
            Debug.Log("setdata");

            var finishedAnimation = false;
            
            _donationBubbleUiElementController.Present(() => finishedAnimation = true);
            

            yield return new WaitUntil(() => finishedAnimation);
            
            Debug.Log("finished presenting");

            yield return new WaitForSeconds(_holdDonationBubbleSeconds);

            finishedAnimation = false;
            _donationBubbleUiElementController.Dismiss(() => finishedAnimation = true);

            yield return new WaitUntil(() => finishedAnimation);
        }

        _isProcessingDonationQueue = false;
    }

    void OnDestroy()
    {
        _campaignModel.OnUpdated -= OnCampaignModelUpdated;
        _coinSpawnController.OnDonationMade -= OnDonationMade;
    }
}
