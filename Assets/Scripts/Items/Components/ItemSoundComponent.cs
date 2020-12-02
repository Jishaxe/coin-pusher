using UnityEngine;

public class ItemSoundComponent : MonoBehaviour
{
    [SerializeField] private SoundClipContainer _itemCollectSounds;
    [SerializeField] private SoundClipContainer _itemDropSounds;
    [SerializeField] private float _itemDropSoundThreshold;
    
    void Awake()
    {
        Item.ItemCollectedEvent += OnCollected;
    }

    private void OnDestroy()
    {
        Item.ItemCollectedEvent -= OnCollected;
    }

    private void OnCollected(Item item)
    {
        PlayItemCollectSound();
    }
    
    public void OnCollisionEnter(Collision other)
    {
        var force = other.relativeVelocity.magnitude;
        if (force > _itemDropSoundThreshold)
        {
            PlayItemDropSound();
        }
    }
    
    private void PlayItemDropSound()
    {
        var sound = _itemDropSounds.GetRandomClip();
        _itemDropSounds.AudioSource.PlayOneShot(sound);
    }

    private void PlayItemCollectSound()
    {
        var sound = _itemCollectSounds.GetRandomClip();
        _itemCollectSounds.AudioSource.PlayOneShot(sound);
    }
}
