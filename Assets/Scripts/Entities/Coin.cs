using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using Zenject;

public class CoinFactory : IFactory<Coin, Coin>
{
    private DiContainer _container;
    
    public CoinFactory(DiContainer container)
    {
        _container = container;
    }
    
    public Coin Create(Coin coinDef)
    {
        return _container.InstantiatePrefab(coinDef.gameObject).GetComponent<Coin>();
    }
}

public class Coin : MonoBehaviour, ISaveLoadable<RawCoinData>
{
    public static event Action<Coin> OnCoinCollected;
    
    private Rigidbody _rigidbody;
    private Collider _collider;

    [SerializeField] private ParticleSystem _collectionPFX;

    [Header("Sound")] [Space(30)] 
    [SerializeField] private SoundClipContainer _coinDropSounds;
    [SerializeField] private AudioSource _coinDropAudioSource;
    [SerializeField] private float _coinDropSoundThreshold;
    [SerializeField] private SoundClipContainer _coinCollectSounds;
    [SerializeField] private AudioSource _coinCollectAudioSource;
    [Space(30)]
    
    public float value;
    private bool _isCollected = false;

    public class Factory : PlaceholderFactory<Coin, Coin>
    {
        
    }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CoinCatcher"))
        {
            if (_isCollected) return;
            StartCoroutine(OnCollected());
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        var force = other.relativeVelocity.magnitude;
        if (force > _coinDropSoundThreshold)
        {
            PlayCoinDropSound();
        }
    }

    private IEnumerator OnCollected()
    {
        _isCollected = true;
        
        OnCoinCollected?.Invoke(this);

        PlayCoinCollectSound();
        
        _rigidbody.isKinematic = true;
        _collider.enabled = false;

        yield return StartCoroutine(PlayCollectionPFX());
        
        Destroy(gameObject);
    }

    IEnumerator PlayCollectionPFX()
    {
        _collectionPFX.gameObject.SetActive(true);
        var trns = _collectionPFX.transform;
        
        trns.SetParent(null);
        trns.up = Vector3.up;
        _collectionPFX.Play();
        
        yield return new WaitUntil(() => _collectionPFX.isStopped);
    }

    public RawCoinData Save()
    {
        var rawCoinData = new RawCoinData();
        rawCoinData.position = transform.position;
        rawCoinData.rotation = transform.rotation.eulerAngles;

        rawCoinData.velocity = _rigidbody.velocity;
        rawCoinData.angularVelocity = _rigidbody.angularVelocity;

        return rawCoinData;
    }

    public void Load(RawCoinData data)
    {
        transform.position = data.position;
        transform.rotation = Quaternion.Euler(data.rotation);
        _rigidbody.velocity = data.velocity;
        _rigidbody.angularVelocity = data.angularVelocity;
        _rigidbody.WakeUp();
    }

    private void PlayCoinDropSound()
    {
        var sound = _coinDropSounds.GetRandomClip();
        _coinDropAudioSource.PlayOneShot(sound);
    }

    private void PlayCoinCollectSound()
    {
        var sound = _coinCollectSounds.GetRandomClip();
        _coinCollectAudioSource.PlayOneShot(sound);
    }
}

public class RawCoinData
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;
}

