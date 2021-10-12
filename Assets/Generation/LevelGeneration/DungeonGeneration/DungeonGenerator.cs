using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DungeonGenerator : MonoBehaviour
{
    [SerializeField] int _width;
    [SerializeField] int _height;
    [SerializeField] float _nodeRadius;
    public float NodeRadius => this._nodeRadius;
    [SerializeField] float _pathNoisedScale;
    [SerializeField] RoomSetupSkin[] _setupSkins;
    [SerializeField] int _attemptsToPlaceRoom;
    [SerializeField] Vector2Int _minimumRoomsize;
    [SerializeField] Vector2Int _maximumRoomsize;
    [SerializeField] int _overheadPaths;
    [SerializeField] float _noiseWeights;

    // Start is called before the first frame update
    void Start()
    {
        GridGenerator genner = new GridGenerator();
        GridProperties gridProps = new GridProperties();
        gridProps.width = this._width;
        gridProps.height = this._height;
        gridProps.nodeRadius = this._nodeRadius;
        gridProps.perlinPathScale = this._pathNoisedScale;

        RoomProperties roomProps = new RoomProperties();
        roomProps.setupSkins = this._setupSkins;
        roomProps.attemptsToPlace = this._attemptsToPlaceRoom;
        roomProps.minimumSize = this._minimumRoomsize;
        roomProps.maximumSize = this._maximumRoomsize;

        PathProperties pathProps = new PathProperties();
        pathProps.broadPaths = false;
        pathProps.overheadPaths = this._overheadPaths;
        AStarPathfinder.weightMultiplier = this._noiseWeights;
        Dungeon.dungeonMap = genner.Generate(gridProps, roomProps, pathProps);
    }
    public abstract void GenerateDungeon(Grid dungeonMap);
}
