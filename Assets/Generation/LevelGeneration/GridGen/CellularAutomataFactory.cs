using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GridGenerator/CellularAutomata")]
public class CellularAutomataFactory : GridGeneratorFactory
{
    [SerializeField] string _seed;
    [SerializeField] bool _useRandomSeed = true;
    [SerializeField] [Range(0,100)] int _fillPercentage = 55;
    [SerializeField] int _smoothingIteration = 5, _smoothingStrength = 4;
    [SerializeField] int _width = 64, _height = 64;
    public override IRoomGenerator Create(){
        if(this._useRandomSeed){
            return new CellularAutomata(this._width, this._height, this._fillPercentage, this._smoothingStrength, this._smoothingIteration);
        }else{
            return new CellularAutomata(this._width, this._height, this._seed, this._fillPercentage, this._smoothingStrength, this._smoothingIteration);
        }
    }
}
