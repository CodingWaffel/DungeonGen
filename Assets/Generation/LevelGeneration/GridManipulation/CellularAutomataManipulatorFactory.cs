using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapModifier/Cellular Automata")]
public class CellularAutomataManipulatorFactory : MapModifierFactory
{
    int _width;
    int _height; 
    int _fillPercentage = 45; 
    [SerializeField] int _smoothingStrength = 4; 
    [SerializeField] int _smoothingIteration = 4;
    public override IMapModifier Create() => new CellularAutomata(this._width, this._height, this._fillPercentage, this._smoothingStrength, this._smoothingIteration);

}
