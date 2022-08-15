using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Dungeon : MonoBehaviour
{
    public static Grid dungeonMap;
    public DungeonGenerator _generator;
    public List<Path> Paths{get; private set;}
    List<Room> _rooms;

    void Start() {
        this._generator.GenerateDungeon(dungeonMap);

    }

}
