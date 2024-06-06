using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistGameObject : MonoSingleton<PersistGameObject> 
{
    public override void Awake()
    {
        MakePersistant(true);
        base.Awake();
    }
}
