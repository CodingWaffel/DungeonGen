using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridGeneratorFactory : ScriptableObject
{
    public abstract IGridGenerator Create();
}
