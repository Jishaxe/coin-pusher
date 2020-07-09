using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveLoadable<T>
{
    T Save();
    void Load(T data);
}
