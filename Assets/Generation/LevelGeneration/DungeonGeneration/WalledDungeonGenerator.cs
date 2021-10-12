using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WalledDungeonGenerator : DungeonGenerator
{
    [SerializeField] GameObject _wall;
    [SerializeField] GameObject _floor;
    public override void GenerateDungeon(Grid dungeonMap)
    {
        foreach (Wall wall in GetWalls(dungeonMap, dungeonMap.grid.GetLength(0), dungeonMap.grid.GetLength(1)))
        {
            GameObject newWall = GameObject.Instantiate(this._wall, wall.Position(), Quaternion.identity, this.transform);
            newWall.transform.localScale = new Vector3(1, 1, wall.Size());
            // newWall.transform.LookAt( newWall.transform.position + new Vector3(wall.Direction().x, 0, wall.Direction().y));
            newWall.transform.LookAt(newWall.transform.position + wall.Direction());
        }
        Instantiate(this._floor, transform.position, Quaternion.identity).transform.localScale = new Vector3(dungeonMap.grid.GetLength(0), 1, dungeonMap.grid.GetLength(1));
    }
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

    List<Wall> GetWalls(Grid grid, int width, int height){
        List<Wall> result = new List<Wall>();
        int[,] temp = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                temp[x,y] = grid.grid[x,y].value == 0 ? 0 : IsWall(grid.grid[x,y]) ? 1 : -1;
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid.grid[x,y].value = temp[x,y];
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
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(!(grid.grid[x,y].value == 1)) continue;

                Wall wall = new Wall();
                wall.start = grid.grid[x,y].WorldPoint;
                grid.grid[x,y].value = -2; //first start

                Vector2Int wallDirection = GetWallDirection(grid, grid.grid[x,y]);
                Wall nextWall = GetWall(grid, grid.grid[x,y], wallDirection);

                while(nextWall.start != nextWall.end){
                    result.Add(nextWall);
                    nextWall = GetWall(grid, grid.GetNearestNodeOnGrid(nextWall.end), GetWallDirection(grid, grid.GetNearestNodeOnGrid(nextWall.end)));
                }

            }
        }

        return result;
    }
    bool IsWall(Node node) => node._tileType == Node.TileType.Wall;

    Wall GetWall(Grid grid, Node start, Vector2Int direction){
        if(direction.x == 0 && direction.y == 0) return new Wall(start.WorldPoint, start.WorldPoint);
        Node nextNode = grid.grid[start.gridX + direction.x, start.gridY + direction.y];
        int counter = 1;

        while(nextNode.value == 1){
            nextNode.value = -1;
            counter++;
            if(grid.IsOnGrid(start.gridX + direction.x * counter, start.gridY + direction.y * counter))
                nextNode = grid.grid[start.gridX + direction.x * counter, start.gridY + direction.y * counter];                      
        }
        counter--;
        nextNode = grid.grid[start.gridX + direction.x * counter, start.gridY + direction.y * counter];
        return new Wall(start.WorldPoint, nextNode.WorldPoint);

    }

    Vector2Int GetWallDirection(Grid grid, Node node){
        List<Node> neighbours = grid.GetNeighbours(node, false);
        foreach (Node n in neighbours)
        {
            if(n.value == 1)
                return new Vector2Int(n.gridX - node.gridX, n.gridY - node.gridY);;
        }
        
        return new Vector2Int(0,0);
    }

    
}
