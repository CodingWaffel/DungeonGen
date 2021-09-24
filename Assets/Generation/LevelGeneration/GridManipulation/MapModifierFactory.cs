using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapModifierFactory : ScriptableObject
{
    public abstract IMapModifier Create();
}
