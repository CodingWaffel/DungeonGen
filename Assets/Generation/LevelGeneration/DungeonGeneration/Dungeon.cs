using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Dungeon : MonoBehaviour
{
    static Grid dungeonMap;
    public static Grid DungeonMap => dungeonMap;
    [Header("Visuals")]
    [SerializeField] bool _useMesh;
    [SerializeField] Material _dungeonMat;
    [SerializeField] float _wallHeight = 20f;
    [SerializeField] float _obstacleHeight = 5f;
    [SerializeField] int _sacleFactor = 4;

    [Header("Dungeon Variables")]
    [SerializeField] int _width = 1024;
    [SerializeField] int _height = 1024;
    [SerializeField] float _nodeRadius = .5f;
    [SerializeField] int _attemptsToPlaceRoom = 200;

    [Header("Path variables")]
    [SerializeField] RoomSkin _pathSkin;
    [SerializeField] float _pathNoisedScale = 30f;
    [SerializeField] float _noiseWeights = 100f;
    [SerializeField] int _overheadPaths = 5;

    [Header("Room Parameter")]
    [SerializeField] Vector2Int _minimumRoomsize;
    [SerializeField] Vector2Int _maximumRoomsize;
    [SerializeField] RoomSetupSkin[] _setupSkins;
    [SerializeField] RoomSkin[] _roomskin;
    [SerializeField] GameObject _entryDoor;

    [Header("Player")]
    [SerializeField] GameObject _player;
    [SerializeField] GameObject _exit;

    List<Room> _rooms;

    void Start() {
        if(this._useMesh){
            GenerateMeshDungeon();
        }else{
            GenerateBlockDungeon();
        }
        

    }

    void GenerateMeshDungeon(){
        //Init new Grid with -1
        int[,] baseGrid = new int[this._width, this._height];
        for (int x = 0; x < baseGrid.GetLength(0); x++)
            for (int y = 0; y < baseGrid.GetLength(1); y++)
                baseGrid[x,y] = -1;
        dungeonMap = new Grid(baseGrid, this._nodeRadius);
        AStarPathfinder.weightMultiplier = this._noiseWeights;

        //Set Node weights
        float[,] perlin = PerlinFilledMatrix(this._width, this._height, 0, 0, this._pathNoisedScale);
        for (int i = 0; i < perlin.GetLength(0); i++)
        {
            for (int j = 0; j < perlin.GetLength(1); j++)
            {
                dungeonMap.grid[i,j].weight = perlin[i,j];
                // dungeonMap.grid[i,j].weight = Random.Range(0f, 1f);
            }
        }

        //Generate Rooms
        this._rooms = GenerateRooms(this._setupSkins);


        //Connect Rooms
        List<List<Node>> paths = this.ConnectRooms(this._rooms, true);

        

        MapGen.MapGenerator mapGen = new MapGen.MapGenerator();
  
        float[,] smoothedMap = new float[this._width, this._height];
        for (int x = 0; x <= this._width; x++)
        {
            for (int y = 0; y <= this._height; y++)
            {
                float val = 0f;
                
                if(x-1 >= 0 && x+1 < this._width && y-1 >= 0 && y+1 < this._height){

                    for(int i = x-1; i <= x+1; i++){
                        for (int j = y-1; j <= y+1; j++)
                        {
                            val += Mathf.Abs(dungeonMap.grid[i, j].value);
                        }
                    }
                    smoothedMap[x,y] = val/9f;
                }
            }
        }

        int scalefactor = this._sacleFactor;
        float[,] heightMap = new float[this._width * scalefactor, this._height * scalefactor];
        for (int i = 0; i < this._width * scalefactor; i+=scalefactor)
        {
            for (int j = 0; j < this._height * scalefactor; j+=scalefactor)
            {
                float val = (smoothedMap[i/scalefactor,j/scalefactor]) * this._wallHeight;
                for (int x = 0; x < scalefactor; x++)
                {
                    for (int y = 0; y < scalefactor; y++)
                    {
                        heightMap[i+x,j+y] = val;
                    }
                } 
            }
        }
        mapGen.GenerateMap(heightMap, this._dungeonMat);
    }

    void GenerateBlockDungeon(){
        //Init new Grid with -1
        int[,] baseGrid = new int[this._width, this._height];
        for (int x = 0; x < baseGrid.GetLength(0); x++)
            for (int y = 0; y < baseGrid.GetLength(1); y++)
                baseGrid[x,y] = -1;
        dungeonMap = new Grid(baseGrid, this._nodeRadius);
        AStarPathfinder.weightMultiplier = this._noiseWeights;

        //Set Node weights
        float[,] perlin = PerlinFilledMatrix(this._width, this._height, 0, 0, this._pathNoisedScale);
        for (int i = 0; i < perlin.GetLength(0); i++)
        {
            for (int j = 0; j < perlin.GetLength(1); j++)
            {
                dungeonMap.grid[i,j].weight = perlin[i,j];
                // dungeonMap.grid[i,j].weight = Random.Range(0f, 1f);
            }
        }

        //Generate Rooms
        this._rooms = GenerateRooms(this._setupSkins);


        //Connect Rooms
        List<List<Node>> paths = this.ConnectRooms(this._rooms, false);

        int additionalConnections = 5;
        for (int i = 0; i < additionalConnections; i++)
        {
            Node roomA = dungeonMap.GetNearestNodeOnGrid(this._rooms[Random.Range(0, this._rooms.Count)].Position);
            Node roomB = dungeonMap.GetNearestNodeOnGrid(this._rooms[Random.Range(0, this._rooms.Count)].Position);
            List<Node> newPath = AStarPathfinder.FindPath(dungeonMap, roomA, roomB, (node) => true);
            foreach (Node node in newPath)
            {
                node.value = 0;
            } 
            paths.Add(newPath);
        }

        //Define Entry-/Exitpoints TODO
        (Room, Room, float) distance = (this._rooms[0], this._rooms[0], 0f);

        for (int i = 0; i < this._rooms.Count; i++)
        {
            for (int j = 0; j < this._rooms.Count; j++)
            {
                float dist = Vector3.Distance(this._rooms[i].Position, this._rooms[j].Position);
                if(dist > distance.Item3){
                    distance = (this._rooms[i], this._rooms[j], dist);
                }
            }
        }
        //FindDoorway
        Room entryRoom = distance.Item1;
        Vector3 playerPos = new Vector3(entryRoom.XPos, 0, entryRoom.Position.z);
        Node playerSpawnPosition = dungeonMap.GetNearestNodeOnGrid(playerPos);
        while(playerSpawnPosition.value != 0){
            playerPos.x += .5f;
            playerSpawnPosition = dungeonMap.GetNearestNodeOnGrid(playerPos);
        }
        Node doorPosition = dungeonMap.GetNearestNodeOnGrid(playerSpawnPosition.WorldPoint - new Vector3(1f, 0 ,0));
        doorPosition.value = 0;


        //Gradient for skins
        // Vector3 gradient = new Vector3(Random.Range(0f, 1f), 0, Random.Range(0f, 1f)).normalized;
        Vector3 gradient = new Vector3(1, 0, 0);

        Vector3 endOfGradient = new Vector3(dungeonMap.gridWorldSize.x, 0f, 0f);
        

        // Setup rooms with walls
        foreach (Room room in this._rooms)
        {
            Vector3 roomPosition = room.Position;
            float value = roomPosition.x / endOfGradient.x;
            int skin = (int) (this._roomskin.Length * value);


            Transform roomParent = (new GameObject("Room")).transform;
            roomParent.SetParent(transform);
            room.Create(roomParent, this._roomskin[skin]);
        }

        
        //Setup connecting paths
        foreach (List<Node> path in paths)
        {
            foreach (Node node in path)
            {
                if(!node.instantiated && node.value == 0){
                    Instantiate(this._pathSkin.floor, node.WorldPoint, Quaternion.identity);
                    node.instantiated = true;
                }
                foreach (Node wallNode in dungeonMap.GetNeighbours(node, true))
                {
                    if(!wallNode.instantiated && wallNode.value != 0){
                        Instantiate(this._pathSkin.wall, wallNode.WorldPoint + Vector3.up * this._pathSkin.wallYoffset, Quaternion.identity);
                        wallNode.instantiated = true;
                    }
                }
            }
        }

        Instantiate(this._player, playerSpawnPosition.WorldPoint + Vector3.up, Quaternion.identity);
        Instantiate(this._entryDoor, doorPosition.WorldPoint + Vector3.up * .5f, this._entryDoor.transform.rotation);
        Instantiate(this._exit, dungeonMap.GetNearestNodeOnGrid(distance.Item2.Position).WorldPoint + Vector3.up * .5f, Quaternion.identity);
    }

    List<Room> GenerateRooms(RoomSetupSkin[] setups){
        List<Room> rooms = new List<Room>();
        int counter = this._attemptsToPlaceRoom;
        while(counter > 0){
            Vector2Int size = new Vector2Int(Random.Range(this._minimumRoomsize.x, this._maximumRoomsize.x), Random.Range(this._minimumRoomsize.y, this._maximumRoomsize.y));
            Vector2Int coordinate = new Vector2Int(Random.Range(0, dungeonMap.GetGridNodes().GetLength(0)), Random.Range(0, dungeonMap.GetGridNodes().GetLength(1)));


            for (int x = coordinate.x; x < coordinate.x + size.x; x++)
            {
                for (int y = coordinate.y; y < coordinate.y + size.y; y++)
                {
                    if(x < 0 || y < 0 || x >= dungeonMap.GetGridNodes().GetLength(0) || y >= dungeonMap.GetGridNodes().GetLength(1))
                        goto endOfWhile;
                    if(dungeonMap.GetGridNodes()[x, y].value == 1)
                        goto endOfWhile;
                }
            }

            RoomSetupSkin skin = setups[Random.Range(0, setups.Length)];
            Grid tempRoomGrid = skin.gridGeneratorFactory.Create().GenerateMap(size.x, size.y, this._nodeRadius);
            for (int i = 0; i < skin.modifierFactory.Count; i++)
            {
                tempRoomGrid = skin.modifierFactory[i].Create().Modify(tempRoomGrid);
            }
            int xa = 0;
            int ya = 0;
            for (int x = coordinate.x; x < coordinate.x + size.x; x++)
            {
                ya = 0;
                for (int y = coordinate.y; y < coordinate.y + size.y; y++)
                {
                    dungeonMap.GetGridNodes()[x, y].value = tempRoomGrid.GetGridNodes()[xa, ya].value;
                    ya++;
                }
                xa++;
            }
            rooms.Add(new Room(dungeonMap, coordinate.x, coordinate.y, size.x, size.y));
            
            endOfWhile:
            counter--;
        }


        return rooms;
    }

    List<List<Node>> ConnectRooms(List<Room> rooms, bool broadPaths){
        List<List<Node>> result = new List<List<Node>>();

        int roomAmount = rooms.Count;
        int[,] distanceMatrix = new int[roomAmount,roomAmount];
        for (int i = 0; i < roomAmount; i++)
        {
            for (int j = 0; j < roomAmount; j++)
            {
                if(i == j){
                    distanceMatrix[i,j] = 0;
                    continue;
                } 
                //dinstances of positions
                distanceMatrix[i,j] = (int)Vector2.Distance(rooms[i].Position, rooms[j].Position);
            }
        }
        //Prim MST
        int[] parents;
        int[] distances;
        VonPrim.RunPrim(distanceMatrix, roomAmount, out parents, out distances);

        for (int i = 1; i < roomAmount; i++)
        {
            Node startingNode = dungeonMap.GetNearestNodeOnGrid(rooms[i].Position);
            Node targetNode = dungeonMap.GetNearestNodeOnGrid(rooms[parents[i]].Position);
            List<Node> path = AStarPathfinder.FindPath(dungeonMap, startingNode, targetNode, (Node node) => true, false);
            
            foreach (Node node in path)
            {
                node.value = 0;
                if(broadPaths)
                    foreach (Node neighbour in dungeonMap.GetNeighbours(node, true))
                    {
                        neighbour.value = 0;
                        foreach (Node neighbour2 in dungeonMap.GetNeighbours(neighbour, true))
                        {
                            neighbour2.value = 0;
                        }
                    }
                
            }
            result.Add(path);
        }
        for (int i = 0; i < this._overheadPaths; i++)
        {
            Node roomA = dungeonMap.GetNearestNodeOnGrid(this._rooms[Random.Range(0, this._rooms.Count)].Position);
            Node roomB = dungeonMap.GetNearestNodeOnGrid(this._rooms[Random.Range(0, this._rooms.Count)].Position);
            List<Node> newPath = AStarPathfinder.FindPath(dungeonMap, roomA, roomB, (node) => true);
            foreach (Node node in newPath)
            {
                node.value = 0;
                if(broadPaths)
                    foreach (Node neighbour in dungeonMap.GetNeighbours(node, true))
                    {
                        neighbour.value = 0;
                        foreach (Node neighbour2 in dungeonMap.GetNeighbours(neighbour, true))
                        {
                            neighbour2.value = 0;
                        }
                    }
            } 
            result.Add(newPath);
        }

        return result;

    }

    float[,] PerlinFilledMatrix(int widthIn, int heightIn, float xOrg, float yOrg, float scale){
        float[,] result = new float[widthIn,heightIn];
        float width = (float)widthIn;
        float height = (float)heightIn;
        float y = 0.0F;

        while (y < height)
        {
            float x = 0.0F;
            while (x < width)
            {
                float xCoord = xOrg + x / width * scale;
                float yCoord = yOrg + y / height * scale;
                result[(int)x, (int)y] = 1 - Mathf.PerlinNoise(xCoord, yCoord);
                xCoord = xOrg + x / width * scale/2f;
                yCoord = yOrg + y / height * scale/2f;
                result[(int)x, (int)y] *= 1 - Mathf.PerlinNoise(xCoord, yCoord);
                x++;
            }
            y++;
        }
        return result;
    }

}
