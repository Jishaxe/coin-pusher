using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class ItemFactory : IFactory<Item, Item>
{
    private DiContainer _container;
    
    public ItemFactory(DiContainer container)
    {
        _container = container;
    }
    
    public Item Create(Item prefab)
    {
        return _container.InstantiatePrefab(prefab.gameObject).GetComponent<Item>();
    }
}


public abstract class Item : MonoBehaviour, ISaveLoadable<RawItemData>
{
    [SerializeField] private ParticleSystem _collectionPFX;
    
    public static event Action<Item> ItemCollectedEvent;
    public Rigidbody Rigidbody => _rigidbody;
    public Collider Collider => _collider;
    
    private Rigidbody _rigidbody;
    private Collider _collider;
    
    private bool _isCollected = false;
    
    public class Factory : PlaceholderFactory<Item, Item>
    {
        
    }
    
    public virtual void Initialize()
    {

    }

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ItemCatcher"))
        {
            if (_isCollected) return;
            StartCoroutine(OnCollected());
        }
    }
    
    private IEnumerator OnCollected()
    {
        _isCollected = true;
        
        ItemCollectedEvent?.Invoke(this);

        Rigidbody.isKinematic = true;
        Collider.enabled = false;

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

    protected void SaveBase(RawItemData data)
    {
        data.position = transform.position;
        data.rotation = transform.rotation.eulerAngles;
        data.velocity = Rigidbody.velocity;
        data.angularVelocity = Rigidbody.angularVelocity;
    }
    
    protected void LoadBase(RawItemData data)
    {
        transform.position = data.position;
        transform.rotation = Quaternion.Euler(data.rotation);
        _rigidbody.velocity = data.velocity;
        _rigidbody.angularVelocity = data.angularVelocity;
        _rigidbody.WakeUp();
    }
    
    public virtual RawItemData Save()
    {
        throw new NotImplementedException();
    }

    public virtual void Load(RawItemData data)
    {
        throw new NotImplementedException();
    }
}

public class RawItemData
{
    public RawItemData()
    {
        ItemType = GetType().ToString();
    }
    
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;
    
    public string ItemType;
}
