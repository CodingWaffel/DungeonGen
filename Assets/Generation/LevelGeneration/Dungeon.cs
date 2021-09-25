using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Dungeon : MonoBehaviour
{
    static Grid dungeonMap;
    public static Grid DungeonMap => dungeonMap;

    [Header("Dungeon Variables")]
    [SerializeField] int _width = 1024;
    [SerializeField] int _height = 1024;
    [SerializeField] float _nodeRadius = .5f;
    [SerializeField] int _attemptsToPlaceRoom = 200;
    [SerializeField] float _pathNoiseWeight = 1f;
    [SerializeField] float _pathNoisedScale = 30f;

    [Header("Room Parameter")]
    [SerializeField] Vector2Int _minimumRoomsize;
    [SerializeField] Vector2Int _maximumRoomsize;
    [SerializeField] RoomSkin[] _roomskin;
    [SerializeField] RoomSkin _pathSkin;
    [SerializeField] PrimFactory _gridGeneratorFactory;
    [SerializeField] List<MapModifierFactory> _modifierFactory;
    [SerializeField] GameObject _entryDoor;

    [Header("Player")]
    [SerializeField] GameObject _player;
    [SerializeField] GameObject _exit;

    IGridGenerator _gridGrnerator;
    IMapModifier[] _modifier;
    
    List<Room> _rooms;

    void Awake(){
        this._gridGrnerator = this._gridGeneratorFactory.Create();
        this._modifier = this._modifierFactory.ConvertAll(f => f.Create()).ToArray();
    }
    void Start() {
        
        //Init new Grid with -1
        int[,] baseGrid = new int[this._width, this._height];
        for (int x = 0; x < baseGrid.GetLength(0); x++)
            for (int y = 0; y < baseGrid.GetLength(1); y++)
                baseGrid[x,y] = -1;
        dungeonMap = new Grid(baseGrid, this._nodeRadius);

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
        this._rooms = GenerateRooms();


        //Connect Rooms
        List<List<Node>> paths = this.ConnectRooms(this._rooms);

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


        // Setup rooms with walls
        foreach (Room room in this._rooms)
        {
            Transform roomParent = (new GameObject("Room")).transform;
            roomParent.SetParent(transform);
            room.Create(roomParent, this._roomskin[Random.Range(0, this._roomskin.Length)]);
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

        // for (int x = 0; x < dungeonMap.GetGridNodes().GetLength(0); x++)
        // {
        //     for (int y = 0; y < dungeonMap.GetGridNodes().GetLength(1); y++)
        //     {
        //         if(dungeonMap.GetGridNodes()[x,y].value == 0){
        //             Instantiate(this._player, dungeonMap.GetGridNodes()[x,y].WorldPoint + Vector3.up, Quaternion.identity);
        //             return;
        //         }
        //     }
        // }

        
        

        
        Instantiate(this._player, playerSpawnPosition.WorldPoint + Vector3.up * .5f, Quaternion.identity);
        Instantiate(this._entryDoor, doorPosition.WorldPoint + Vector3.up * .5f, this._entryDoor.transform.rotation);
        Instantiate(this._exit, dungeonMap.GetNearestNodeOnGrid(distance.Item2.Position).WorldPoint + Vector3.up * .5f, Quaternion.identity);


    }

    List<Room> GenerateRooms(){
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
            Grid tempRoomGrid = this._gridGrnerator.GenerateMap(size.x, size.y, this._nodeRadius);
            for (int i = 0; i < this._modifier.Length; i++)
            {
                tempRoomGrid = this._modifier[i].Modify(tempRoomGrid);
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

    Room GetSingleRoom(){
        return null;
    }

    List<List<Node>> ConnectRooms(List<Room> rooms){
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
            Debug.Log($"pathlength = {path.Count}");
            foreach (Node node in path)
            {
                node.value = 0;
            }
            result.Add(path);
        }

        return result;


        
        //linear connections
          //connection b => horizontal/vertical connection
        //setup path values on grid
    }

    bool IgnoreNodes(Node node) => node.weight <= .8f;

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
