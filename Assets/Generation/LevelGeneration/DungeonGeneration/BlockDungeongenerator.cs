using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockDungeongenerator : DungeonGenerator
{
    [SerializeField] RoomSkin[] _roomskin;

    public override void GenerateDungeon(Grid dungeonMap)
    {
        
        foreach (Node node in dungeonMap.grid)
                {
                    if(node._tileType == Node.TileType.Wall)
                        if(Dungeon.dungeonMap.GetNeighbours(node, true).Any(n => n._tileType == Node.TileType.Floor)){
                            if(node.gridX -1 >= 0 && node.gridY - 1 >= 0 && node.gridX + 1 < dungeonMap.grid.GetLength(0) && node.gridY + 1 < dungeonMap.grid.GetLength(1)){
                                if((dungeonMap.grid[node.gridX, node.gridY - 1]._tileType == Node.TileType.Wall && dungeonMap.grid[node.gridX + 1, node.gridY]._tileType == Node.TileType.Wall) &&
                                    !(dungeonMap.grid[node.gridX, node.gridY + 1]._tileType == Node.TileType.Wall || dungeonMap.grid[node.gridX - 1, node.gridY]._tileType == Node.TileType.Wall)){
                                    GameObject.Instantiate(this._roomskin[0].wallCorner, node.WorldPoint, Quaternion.Euler(0, -90, 0), transform);
                                }else 
                                if((dungeonMap.grid[node.gridX - 1, node.gridY]._tileType == Node.TileType.Wall && dungeonMap.grid[node.gridX, node.gridY - 1]._tileType == Node.TileType.Wall) &&
                                    !(dungeonMap.grid[node.gridX + 1, node.gridY]._tileType == Node.TileType.Wall || dungeonMap.grid[node.gridX, node.gridY + 1]._tileType == Node.TileType.Wall)){
                                    GameObject.Instantiate(this._roomskin[0].wallCorner, node.WorldPoint, Quaternion.Euler(0, 0, 0), transform);
                                }else 
                                if((dungeonMap.grid[node.gridX, node.gridY + 1]._tileType == Node.TileType.Wall && dungeonMap.grid[node.gridX + 1, node.gridY]._tileType == Node.TileType.Wall) &&
                                    !(dungeonMap.grid[node.gridX, node.gridY - 1]._tileType == Node.TileType.Wall || dungeonMap.grid[node.gridX - 1, node.gridY]._tileType == Node.TileType.Wall)){
                                    GameObject.Instantiate(this._roomskin[0].wallCorner, node.WorldPoint, Quaternion.Euler(0, 180, 0), transform);
                                }else 
                                if((dungeonMap.grid[node.gridX, node.gridY + 1]._tileType == Node.TileType.Wall && dungeonMap.grid[node.gridX - 1, node.gridY]._tileType == Node.TileType.Wall) &&
                                    !(dungeonMap.grid[node.gridX, node.gridY - 1]._tileType == Node.TileType.Wall || dungeonMap.grid[node.gridX + 1, node.gridY]._tileType == Node.TileType.Wall)){
                                    GameObject.Instantiate(this._roomskin[0].wallCorner, node.WorldPoint, Quaternion.Euler(0, 90, 0), transform);
                                }else{
                                    GameObject.Instantiate(this._roomskin[0].wall, node.WorldPoint, this._roomskin[0].wall.transform.rotation, transform);
                                }
                            }else{
                                GameObject.Instantiate(this._roomskin[0].wall, node.WorldPoint, this._roomskin[0].wall.transform.rotation, transform);
                        }
                        
                        GameObject.Instantiate(this._roomskin[0].floor, node.WorldPoint + Vector3.up * this._roomskin[0].floorYoffset, this._roomskin[0].floor.transform.rotation, transform);
                        node.instantiated = true;
                    }
                    if(node._tileType != Node.TileType.None)
                        Instantiate(this._roomskin[0].floor, node.WorldPoint + Vector3.up * this._roomskin[0].floorYoffset, Quaternion.identity, transform);
                }
    }
}
