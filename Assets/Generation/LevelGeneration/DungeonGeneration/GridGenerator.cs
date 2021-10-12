using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridGenerator
{

    public Grid Generate(GridProperties gridProperties, RoomProperties roomProperties, PathProperties pathProperties){
        Grid grid = this.InitEmpty(gridProperties.width, gridProperties.height, gridProperties.nodeRadius);
        grid = SetPerlinWeights(grid, gridProperties.perlinPathScale);
        //Rooms
        List<Room> rooms = GenerateRooms(grid, gridProperties.nodeRadius, roomProperties.setupSkins, roomProperties.attemptsToPlace, roomProperties.minimumSize, roomProperties.maximumSize);
        //DetectWallTiles
        foreach (Node node in grid.grid)
        {
            if(IsWall(grid, node)){
                node._tileType = Node.TileType.Wall;
                node._nodeType = Node.NodeType.Room;
            }else if(node.value == 0){
                node._tileType = Node.TileType.Floor;
                node._nodeType = Node.NodeType.Room;
            }else{
                node._tileType = Node.TileType.None;
            }
            node._nodeType = node._tileType != Node.TileType.None ? Node.NodeType.Room : Node.NodeType.None;
        }
        //paths
        List<List<Node>> paths = ConnectRooms(grid, rooms, pathProperties.overheadPaths, pathProperties.broadPaths);




        return grid;
    }
    bool IsWall(Grid grid, Node node) => node.value == 1 && grid.GetNeighbours(node, true).Any(n => n.value == 0);
    //Init empty grid
    Grid InitEmpty(int width, int height, float nodeRadius){
        int[,] grid = new int[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x,y] = -1;   
        return new Grid(grid, nodeRadius);
    }
    Grid SetPerlinWeights(Grid grid, float perlinPathScale){
        //Set Node weights
        float[,] perlin = PerlinFilledMatrix(grid.grid.GetLength(0), grid.grid.GetLength(1), 0, 0, perlinPathScale);
        for (int i = 0; i < perlin.GetLength(0); i++)
        {
            for (int j = 0; j < perlin.GetLength(1); j++)
            {
                grid.grid[i,j].weight = perlin[i,j];
                // dungeonMap.grid[i,j].weight = Random.Range(0f, 1f);
            }
        }
        return grid;
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
    //Create Rooms

    List<Room> GenerateRooms(Grid grid, float nodeRadius, RoomSetupSkin[] setups, int attemptsToPlace, Vector2Int minimumRoomsize, Vector2Int maximumRoomsize){
        List<Room> rooms = new List<Room>();
        int counter = attemptsToPlace;
        while(counter > 0){
            Vector2Int size = new Vector2Int(Random.Range(minimumRoomsize.x, maximumRoomsize.x), Random.Range(minimumRoomsize.y, maximumRoomsize.y));
            Vector2Int coordinate = new Vector2Int(Random.Range(0, grid.grid.GetLength(0)), Random.Range(0, grid.grid.GetLength(1)));


            for (int x = coordinate.x; x < coordinate.x + size.x; x++)
            {
                for (int y = coordinate.y; y < coordinate.y + size.y; y++)
                {
                    if(x < 0 || y < 0 || x >= grid.grid.GetLength(0) || y >= grid.grid.GetLength(1))
                        goto endOfWhile;
                    if(grid.grid[x, y].value == 1)
                        goto endOfWhile;
                }
            }

            RoomSetupSkin skin = setups[Random.Range(0, setups.Length)];
            Grid tempRoomGrid = skin.gridGeneratorFactory.Create().GenerateRoom(size.x, size.y, nodeRadius);
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
                    Node node = grid.grid[x, y];
                    // node.weight = 0;

                    //Set Nodevalues for later
                    node.value = tempRoomGrid.GetGridNodes()[xa, ya].value;
                    

                    ya++;
                }
                xa++;
            }
            rooms.Add(new Room(grid, coordinate.x, coordinate.y, size.x, size.y));
            
            endOfWhile:
            counter--;
        }
    

        return rooms;
    }
    //Connect with paths
    List<List<Node>> ConnectRooms(Grid grid, List<Room> rooms, int overheadPaths, bool broadPaths){
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
            Node startingNode = grid.GetNearestNodeOnGrid(rooms[i].Position);
            Node targetNode = grid.GetNearestNodeOnGrid(rooms[parents[i]].Position);
            List<Node> path = AStarPathfinder.FindPath(grid, startingNode, targetNode, (Node node) => true, false);
            
            foreach (Node node in path)
            {
                node.value = 0;
                if(broadPaths)
                    foreach (Node neighbour in grid.GetNeighbours(node, true))
                    {
                        neighbour.value = 0;
                        foreach (Node neighbour2 in grid.GetNeighbours(neighbour, true))
                        {
                            neighbour2.value = 0;
                        }
                    }
                
            }
            result.Add(path);
        }
        for (int i = 0; i < overheadPaths; i++)
        {
            int randomA = Random.Range(0, rooms.Count);
            int randomB = Random.Range(0, rooms.Count);
            if(randomA == randomB) continue;
            Node roomA = grid.GetNearestNodeOnGrid(rooms[randomA].Position);
            Node roomB = grid.GetNearestNodeOnGrid(rooms[randomB].Position);
            List<Node> newPath = AStarPathfinder.FindPath(grid, roomA, roomB, (node) => true);
            foreach (Node node in newPath)
            {
                node.value = 0;
                if(broadPaths)
                    foreach (Node neighbour in grid.GetNeighbours(node, true))
                    {
                        neighbour.value = 0;
                        foreach (Node neighbour2 in grid.GetNeighbours(neighbour, true))
                        {
                            neighbour2.value = 0;
                        }
                    }
            } 
            result.Add(newPath);
        }

        foreach (List<Node> path in result)
        {
            while(path[2]._nodeType == Node.NodeType.Room)
                path.RemoveAt(0);
            while(path[path.Count - 3]._nodeType == Node.NodeType.Room)
                path.RemoveAt(path.Count-1);
            foreach (Node node in path)
            {
                if(node._nodeType == Node.NodeType.Room){
                    node._tileType = Node.TileType.Floor;
                }else{
                    node._nodeType = Node.NodeType.Path;
                    node._tileType = Node.TileType.Floor;
                    foreach (Node neighbour in grid.GetNeighbours(node, true))
                    {
                        if(neighbour._tileType == Node.TileType.None)
                            neighbour._tileType = Node.TileType.Wall;
                    }
                }
            }
            foreach (Node node in path)
            {

                foreach (Node neighbour in grid.GetNeighbours(node, true))
                {
                    if(neighbour._tileType == Node.TileType.None)
                        neighbour._tileType = Node.TileType.Wall;
                }
                
            }
        }

        return result;

    }

    
}

public struct GridProperties{
    public int width;
    public int height;
    public float nodeRadius;
    public float perlinPathScale;
}
    public struct RoomProperties{
        public RoomSetupSkin[] setupSkins;
        public int attemptsToPlace;
        public Vector2Int minimumSize;
        public Vector2Int maximumSize;
    }
    public struct PathProperties{
        public bool broadPaths;
        public int overheadPaths;
    }