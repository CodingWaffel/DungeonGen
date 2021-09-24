using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapModifier
{
    Grid Modify(Grid grid);
}
