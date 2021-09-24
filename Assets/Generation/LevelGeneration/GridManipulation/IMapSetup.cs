using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapSetup
{
    List<Node> Setup(Grid grid, List<Node> spawnpoints);
}
