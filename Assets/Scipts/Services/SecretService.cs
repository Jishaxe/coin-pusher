using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

public class RawSecretsData
{
    public string accessToken;
}

public class SecretService
{
    private RawSecretsData _secrets;
    public const string k_secretsPath = "secrets.json";
    
    [Inject]
    public SecretService()
    {
        LoadSecrets();
    }

    public string GetAccessToken()
    {
        return _secrets.accessToken;
    }
    
    private void LoadSecrets()
    {
        var streamer = new StreamReader(k_secretsPath);
        string json = streamer.ReadToEnd();
        streamer.Close();

        this._secrets = JsonConvert.DeserializeObject<RawSecretsData>(json);
    }
}
