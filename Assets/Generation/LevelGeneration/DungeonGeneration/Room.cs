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

    public void Create(Transform parent, RoomSkin skin){

        for (int x = 0; x < this.Width; x++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                Node node = this._parentGrid.grid[this._xPos + x, this._yPos + y];
                if(node.value == 1){
                    if(Dungeon.DungeonMap.GetNeighbours(node, true).Any(n => n.value == 0)){
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

}
