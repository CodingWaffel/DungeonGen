using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridGenerator
{
    Grid GenerateMap(int width, int height, float nodeRadius);
}

