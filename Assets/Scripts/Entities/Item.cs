using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour, ISaveLoadable<RawItemData>
{
    public virtual void Initialize()
    {
        
    }
    
    public virtual RawItemData Save()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Load(RawItemData data)
    {
        throw new System.NotImplementedException();
    }
}

public class RawItemData
{
    public RawItemData()
    {
        ItemType = GetType().ToString();
    }
    
    public string ItemType;
}
