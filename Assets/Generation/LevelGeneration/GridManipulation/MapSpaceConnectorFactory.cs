using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapModifier/Space Connector")]
public class MapSpaceConnectorFactory : MapModifierFactory
{
    [SerializeField] bool _broadPaths = true;
    public override IMapModifier Create() => new MapSpaceConnector(this._broadPaths);
}
