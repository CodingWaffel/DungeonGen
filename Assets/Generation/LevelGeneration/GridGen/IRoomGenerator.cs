using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRoomGenerator
{
    Grid GenerateRoom(int width, int height, float nodeRadius);
}

