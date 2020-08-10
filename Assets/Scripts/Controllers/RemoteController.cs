using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class RawCampaignAndCommandData
{
    public RawCampaignModel Campaign;
    public List<RawCommand> Commands;
}

public class RemoteController: MonoBehaviour
{
    public class WebRequestResult
    {
        public string text;
        public string error;
    }
    
    public bool IsPolling
    {
        get
        {
            return _pollingLoopCoroutine != null;
        }
    }

    private static string k_commandsEnd = "/commands";
    private Coroutine _pollingLoopCoroutine;
    private Coroutine _pollCoroutine;
    
    private Settings _settings;
    private SecretService _secretService;
    private CampaignModel _campaignModel;
    private CoinSpawnController _coinSpawnController;
    private CommandController _commandController;
    private bool _firstPoll;
    
    [Serializable]
    public sealed class Settings
    {
        public bool isTesting;
        public string baseURL;
        public string localBaseURL;
        public float pollTime;
    }

    [Inject]
    void Construct(Settings settings, CommandController commandController, SecretService secretService, CampaignModel campaignModel, CoinSpawnController coinSpawnController)
    {
        _settings = settings;
        _commandController = commandController;
        _secretService = secretService;
        _campaignModel = campaignModel;
        _coinSpawnController = coinSpawnController;
    }

    public void StartPolling()
    {
        if (_pollingLoopCoroutine != null)
        {
            throw new ArgumentException("Polling coroutine is already running!");
        }

        _firstPoll = true;
        _pollingLoopCoroutine = StartCoroutine(PollingCoroutine());
    }

    IEnumerator PollingCoroutine()
    {
        while (true)
        {
            if (_pollCoroutine == null)
            {
                _pollCoroutine = StartCoroutine(Poll());
            }
            else
            {
                Debug.Log("Poll coroutine still running, skipping...");
            }
            yield return new WaitForSeconds(_settings.pollTime / Time.timeScale);
        }
    }

    IEnumerator GetTextFromEndpoint(string endpoint, WebRequestResult result)
    {
        UnityWebRequest www = UnityWebRequest.Get(endpoint);
        www.SetRequestHeader("Authorization", _secretService.GetAccessToken());
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            result.error = www.error;
            yield break;
        }

        result.text = www.downloadHandler.text;
    }
    
    IEnumerator Poll()
    {
        var result = new WebRequestResult();

        string baseURL = _settings.isTesting ? _settings.localBaseURL : _settings.baseURL;
        yield return StartCoroutine(GetTextFromEndpoint($"{baseURL}{k_commandsEnd}", result));

        if (result.error != null)
        {
            Debug.Log("Error while polling: " + result.error);
            _pollCoroutine = null;
            yield break;
        }

        ProcessRawData(result.text);
        
        _pollCoroutine = null;
    }

    private void ProcessRawData(string json)
    {
        var rawData = ParseCampaignAndCommandsData(json);

        _commandController.AddRawCommands(rawData.Commands);
        
        _campaignModel.Load(rawData.Campaign);

        if (_firstPoll)
        {
            //_coinSpawnController.PopulateBoard(Mathf.Min(_campaignModel.TotalRaised, 9.99f));
            _firstPoll = false;
        }
        
        _campaignModel.Update();
    }

    public RawCampaignAndCommandData ParseCampaignAndCommandsData(string json)
    {
        var data = JsonConvert.DeserializeObject<RawCampaignAndCommandData>(json);
        return data;
    }

    public void StopPolling()
    {
        if (!IsPolling) return;
        
        StopCoroutine(_pollingLoopCoroutine);
    }
}
