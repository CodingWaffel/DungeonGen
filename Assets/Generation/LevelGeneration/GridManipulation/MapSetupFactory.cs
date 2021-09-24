using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapSetupFactoryAsset : ScriptableObject{
    public abstract IMapSetupFactory GetFactory();
}

public interface IMapSetupFactory
{
    IMapSetup Create();
}



