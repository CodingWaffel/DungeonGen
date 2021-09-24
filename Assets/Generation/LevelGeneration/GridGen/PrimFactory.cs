using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GridGenerator/Prim")]
public class PrimFactory : GridGeneratorFactory
{
    [SerializeField][Range(0f, .8f)] float _pointDenseness = .8f;
    [SerializeField] float _pointFrequency = 1f;
    [SerializeField] int _width = 128;
    [SerializeField] int _height = 128;
    [SerializeField] bool _broadPaths = true;
    public override IGridGenerator Create() => new VonPrim(this._width, this._height, 1f - this._pointDenseness, this._pointFrequency, this._broadPaths);

}
