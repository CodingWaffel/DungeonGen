using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Room
{
    Grid _parentGrid;
    Dungeon _parentDungeon;
    int _xPos, _yPos, _width, _height;
    Node[,] _subGrid;

    public int XPos => this._xPos;
    public int YPos => this._yPos;
    public int Width => this._width;
    public int Height => this._height;
    public Vector3 Position => new Vector3(XPos + Width/2, 0 , YPos + Height/2);
    List<Node> _spawnPoints;
    public List<Node> GetSpawnpoints() => this._spawnPoints.Where(n => !this._parentDungeon.Paths.Any(path => path.PathNodes.Contains(n))).ToList();
    public List<Node> GetWallAdjacentSpawnPoints(){
        return GetSpawnpoints().Where(n => this._parentGrid.GetNeighbours(n).Any(neighbour => neighbour.value > .2f)).ToList();
    }
    public Room(Grid grid, Dungeon dungeon, int xPos, int yPos, int width, int height)
    {
        this._parentGrid = grid;
        this._parentDungeon = dungeon;
        this._xPos = xPos;
        this._yPos = yPos;
        this._width = width;
        this._height = height;
        this._spawnPoints = new List<Node>();

        this._subGrid = new Node[width, height];
        int xa = 0;
        int ya = 0;
        for (int x = xPos; x < xPos + width; x++)
        {
            ya = 0;
            for (int y = yPos; y < yPos + height; y++)
            {
                this._subGrid[xa, ya] = this._parentGrid.GetGridNodes()[x,y];

                ya++;
            }
            xa++;
        }
    }
    public Room(Grid grid, int xPos, int yPos, int width, int height)
    {
        this._parentGrid = grid;
        this._xPos = xPos;
        this._yPos = yPos;
        this._width = width;
        this._height = height;
        this._spawnPoints = new List<Node>();

        this._subGrid = new Node[width, height];
        int xa = 0;
        int ya = 0;
        for (int x = xPos; x < xPos + width; x++)
        {
            ya = 0;
            for (int y = yPos; y < yPos + height; y++)
            {
                this._subGrid[xa, ya] = this._parentGrid.GetGridNodes()[x,y];

                ya++;
            }
            xa++;
        }
    }

    public void Create(Transform parent, RoomSkin skin){
        
        for (int x = 0; x < this.Width; x++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                Node node = this._parentGrid.grid[this._xPos + x, this._yPos + y];
                if(IsWall(node)){
                    if(Dungeon.dungeonMap.GetNeighbours(node, true).Any(n => n.value == 0)){
                        if(node.gridX -1 >= 0 && node.gridY - 1 >= 0 && node.gridX + 1 < this._parentGrid.grid.GetLength(0) && node.gridY + 1 < this._parentGrid.grid.GetLength(1)){
                            if((this._parentGrid.grid[node.gridX, node.gridY - 1].value == 1 && this._parentGrid.grid[node.gridX + 1, node.gridY].value == 1) &&
                                !(this._parentGrid.grid[node.gridX, node.gridY + 1].value == 1 || this._parentGrid.grid[node.gridX - 1, node.gridY].value == 1)){
                                GameObject.Instantiate(skin.wallCorner, node.WorldPoint, Quaternion.Euler(0, -90, 0), parent);
                            }else 
                            if((this._parentGrid.grid[node.gridX - 1, node.gridY].value == 1 && this._parentGrid.grid[node.gridX, node.gridY - 1].value == 1) &&
                                !(this._parentGrid.grid[node.gridX + 1, node.gridY].value == 1 || this._parentGrid.grid[node.gridX, node.gridY + 1].value == 1)){
                                GameObject.Instantiate(skin.wallCorner, node.WorldPoint, Quaternion.Euler(0, 0, 0), parent);
                            }else 
                            if((this._parentGrid.grid[node.gridX, node.gridY + 1].value == 1 && this._parentGrid.grid[node.gridX + 1, node.gridY].value == 1) &&
                                !(this._parentGrid.grid[node.gridX, node.gridY - 1].value == 1 || this._parentGrid.grid[node.gridX - 1, node.gridY].value == 1)){
                                GameObject.Instantiate(skin.wallCorner, node.WorldPoint, Quaternion.Euler(0, 180, 0), parent);
                            }else 
                            if((this._parentGrid.grid[node.gridX, node.gridY + 1].value == 1 && this._parentGrid.grid[node.gridX - 1, node.gridY].value == 1) &&
                                !(this._parentGrid.grid[node.gridX, node.gridY - 1].value == 1 || this._parentGrid.grid[node.gridX + 1, node.gridY].value == 1)){
                                GameObject.Instantiate(skin.wallCorner, node.WorldPoint, Quaternion.Euler(0, 90, 0), parent);
                            }else{
                                GameObject.Instantiate(skin.wall, node.WorldPoint, skin.wall.transform.rotation, parent);
                            }
                        }else{
                            GameObject.Instantiate(skin.wall, node.WorldPoint, skin.wall.transform.rotation, parent);
                        }
                        
                        GameObject.Instantiate(skin.floor, node.WorldPoint + Vector3.up * skin.floorYoffset, skin.floor.transform.rotation, parent);
                        node.instantiated = true;
                    }
                    
                }
                if(node.value == 0){
                    GameObject.Instantiate(skin.floor, node.WorldPoint + Vector3.up * skin.floorYoffset, skin.floor.transform.rotation, parent);
                    node.instantiated = true;
                    this._spawnPoints.Add(node);
                }
            }
        }
    }
    public void Create(Transform parent, GameObject wallObject){
        foreach (Wall wall in GetWalls())
        {
            GameObject newWall = GameObject.Instantiate(wallObject, wall.Position(), Quaternion.identity, parent);
            newWall.transform.localScale = new Vector3(1, 1, wall.Size());
            // newWall.transform.LookAt( newWall.transform.position + new Vector3(wall.Direction().x, 0, wall.Direction().y));
            newWall.transform.LookAt(newWall.transform.position + wall.Direction());
        }
        
    }
    bool IsWall(Node node) => node.value == 1 && Dungeon.dungeonMap.GetNeighbours(node, true).Any(n => n.value == 0);

    class Wall{
        public Vector3 start, end;
        public Wall(){}
        public Wall(Vector3 start, Vector3 end){
            this.start = start;
            this.end = end;
        }
        public Vector3 Position(){
            float size = Size();
            return this.start + Direction() * (size/2f);
        }
        public float Size() => Vector3.Distance(this.start, this.end);
        public Vector3 Direction() => (this.end - this.start).normalized;
    }

    List<Wall> GetWalls(){
        List<Wall> result = new List<Wall>();
        int[,] temp = new int[this._width, this._height];
        for (int x = 0; x < this._width; x++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                temp[x,y] = this._subGrid[x,y].value == 0 ? 0 : IsWall(this._subGrid[x,y]) ? 1 : -1;
            }
        }
        for (int x = 0; x < this._width; x++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                this._subGrid[x,y].value = temp[x,y];
            }
        }
        /*
            iteriere über alle nodes
                wenn wallnode
                    start = verbindungspunk
                    getdirection
                    end = verbinsungspunk

                    neuer start = end
                    repeat bis neuer end == first start
        */
        for (int x = this.XPos; x < this._width + this.XPos; x++)
        {
            for (int y = this.YPos; y < this._height + this.YPos; y++)
            {
                if(!(this._parentGrid.grid[x,y].value == 1)) continue;

                Wall wall = new Wall();
                wall.start = this._parentGrid.grid[x,y].WorldPoint;
                this._parentGrid.grid[x,y].value = -2; //first start

                Vector2Int wallDirection = GetWallDirection(this._parentGrid.grid[x,y]);
                Wall nextWall = GetWall(this._parentGrid.grid[x,y], wallDirection);

                while(nextWall.start != nextWall.end){
                    result.Add(nextWall);
                    nextWall = GetWall(this._parentGrid.GetNearestNodeOnGrid(nextWall.end), GetWallDirection(this._parentGrid.GetNearestNodeOnGrid(nextWall.end)));
                }

            }
        }

        return result;
    }

    Wall GetWall(Node start, Vector2Int direction){
        if(direction.x == 0 && direction.y == 0) return new Wall(start.WorldPoint, start.WorldPoint);
        Node nextNode = this._parentGrid.grid[start.gridX + direction.x, start.gridY + direction.y];
        int counter = 1;

        while(nextNode.value == 1){
            nextNode.value = -1;
            counter++;
            if(this._parentGrid.IsOnGrid(start.gridX + direction.x * counter, start.gridY + direction.y * counter))
                nextNode = this._parentGrid.grid[start.gridX + direction.x * counter, start.gridY + direction.y * counter];                      
        }
        counter--;
        nextNode = this._parentGrid.grid[start.gridX + direction.x * counter, start.gridY + direction.y * counter];
        return new Wall(start.WorldPoint, nextNode.WorldPoint);

    }

    Vector2Int GetWallDirection(Node node){
        List<Node> neighbours = this._parentGrid.GetNeighbours(node, false);
        foreach (Node n in neighbours)
        {
            if(n.value == 1)
                return new Vector2Int(n.gridX - node.gridX, n.gridY - node.gridY);;
        }
        
        return new Vector2Int(0,0);
    }

}
