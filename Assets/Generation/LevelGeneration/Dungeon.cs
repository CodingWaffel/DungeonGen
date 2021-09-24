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

    [Header("Room Parameter")]
    [SerializeField] Vector2Int _minimumRoomsize;
    [SerializeField] Vector2Int _maximumRoomsize;
    [SerializeField] GameObject _wallTile;
    [SerializeField] GameObject _floorTile;
    [SerializeField] PrimFactory _gridGeneratorFactory;
    [SerializeField] List<MapModifierFactory> _modifierFactory;
    IGridGenerator _gridGrnerator;
    IMapModifier[] _modifier;
    
    List<Room> _rooms;
    void Start() {
        this._gridGrnerator = this._gridGeneratorFactory.Create();
        this._modifier = this._modifierFactory.ConvertAll(f => f.Create()).ToArray();

        int[,] baseGrid = new int[this._width, this._height];
        for (int x = 0; x < baseGrid.GetLength(0); x++)
            for (int y = 0; y < baseGrid.GetLength(1); y++)
                baseGrid[x,y] = -1;
        dungeonMap = new Grid(baseGrid, this._nodeRadius);

        //Get Room coordinates and size
            //Angbad: randomly pace room, if overlapping, try again, max tries possible
        //Generate Rooms
        this._rooms = GenerateRooms();
        

        //Connect Rooms
        //Define Entry-/Exitpoints
        foreach (Node node in dungeonMap.GetGridNodes())
        {
            if(node.value == 1)
                Instantiate(this._wallTile, node.WorldPoint, Quaternion.identity);
            if(node.value == 0)
                Instantiate(this._floorTile, node.WorldPoint, Quaternion.identity);
            
        }
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
            Grid testRoom = this._gridGrnerator.GenerateMap(size.x, size.y, this._nodeRadius);
            for (int i = 0; i < this._modifier.Length; i++)
            {
                testRoom = this._modifier[i].Modify(testRoom);
            }
            int xa = 0;
            int ya = 0;
            for (int x = coordinate.x; x < coordinate.x + size.x; x++)
            {
                ya = 0;
                for (int y = coordinate.y; y < coordinate.y + size.y; y++)
                {
                    dungeonMap.GetGridNodes()[x, y].value = testRoom.GetGridNodes()[xa, ya].value;
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

    void ConnectRooms(){

    }

}
