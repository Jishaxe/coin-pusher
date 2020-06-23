using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

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
    private CommandController _commandController;
    
    [Serializable]
    public sealed class Settings
    {
        public string baseURL;
        public float pollTime;
    }

    [Inject]
    void Construct(Settings settings, CommandController commandController)
    {
        _settings = settings;
        _commandController = commandController;
    }

    public void StartPolling()
    {
        if (_pollingLoopCoroutine != null)
        {
            throw new ArgumentException("Polling coroutine is already running!");
        }

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
            yield return new WaitForSeconds(_settings.pollTime);
        }
    }

    IEnumerator GetTextFromEndpoint(string endpoint, WebRequestResult result)
    {
        UnityWebRequest www = UnityWebRequest.Get(endpoint);
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
        yield return StartCoroutine(GetTextFromEndpoint($"{_settings.baseURL}{k_commandsEnd}", result));

        if (result.error != null)
        {
            Debug.Log("Error while polling: " + result.error);
            _pollCoroutine = null;
            yield break;
        }

        _commandController.AddRawCommands(ParseCommands(result.text));
        
        _pollCoroutine = null;
    }

    private RawCommands ParseCommands(string json)
    {
        var rawCommands = JsonConvert.DeserializeObject<RawCommands>(json);
        return rawCommands;
    }

    public void StopPolling()
    {
        if (!IsPolling) return;
        
        StopCoroutine(_pollingLoopCoroutine);
    }
}
