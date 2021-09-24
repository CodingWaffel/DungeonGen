using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapModifier/Surround")]
public class MapSurroundFactory : MapModifierFactory
{
    public override IMapModifier Create() => new MapSurround();
}
