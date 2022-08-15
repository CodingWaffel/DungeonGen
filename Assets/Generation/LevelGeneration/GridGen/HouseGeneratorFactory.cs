using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GridGenerator/House Generator")]
public class HouseGeneratorFactory : GridGeneratorFactory
{
    [Range(0f,1f)][SerializeField] float _fillPercentage;
    public override IRoomGenerator Create() => new HouseGenerator(this._fillPercentage);
}
