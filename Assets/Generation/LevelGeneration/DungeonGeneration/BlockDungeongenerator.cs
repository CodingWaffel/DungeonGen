using System.Linq;
using UnityEngine;


public class BlockDungeongenerator : DungeonGenerator
{
    [SerializeField] RoomSkin[] _roomskin;
    [SerializeField][Range(0f, 1f)] float _skinThreshold;
    [SerializeField] bool _setupFloortiles = true;
    [SerializeField] bool _fillVoidWithWalls = false;

    public override void GenerateDungeon(Grid dungeonMap)
    {

        foreach (Node node in dungeonMap.grid)
        {

            if (node._tileType == Node.TileType.Wall)
                if (Dungeon.dungeonMap.GetNeighbours(node, true).Any(n => n._tileType == Node.TileType.Floor))
                {
                    if (node.gridX - 1 >= 0 && node.gridY - 1 >= 0 && node.gridX + 1 < dungeonMap.grid.GetLength(0) && node.gridY + 1 < dungeonMap.grid.GetLength(1))
                    {
                        if (dungeonMap.grid[node.gridX, node.gridY - 1]._tileType == Node.TileType.Wall && dungeonMap.grid[node.gridX + 1, node.gridY]._tileType == Node.TileType.Wall &&
                            !(dungeonMap.grid[node.gridX, node.gridY + 1]._tileType == Node.TileType.Wall || dungeonMap.grid[node.gridX - 1, node.gridY]._tileType == Node.TileType.Wall))
                        {

                            Instantiate(_roomskin[0].wallCorner, node.WorldPoint, Quaternion.Euler(0, -90, 0), transform);
                            if (_setupFloortiles)
                                Instantiate(_roomskin[0].floorCorner, node.WorldPoint + Vector3.up * _roomskin[0].floorYoffset, Quaternion.Euler(0, -90, 0), transform);
                        }
                        else
                        if (dungeonMap.grid[node.gridX - 1, node.gridY]._tileType == Node.TileType.Wall && dungeonMap.grid[node.gridX, node.gridY - 1]._tileType == Node.TileType.Wall &&
                            !(dungeonMap.grid[node.gridX + 1, node.gridY]._tileType == Node.TileType.Wall || dungeonMap.grid[node.gridX, node.gridY + 1]._tileType == Node.TileType.Wall))
                        {

                            Instantiate(_roomskin[0].wallCorner, node.WorldPoint, Quaternion.Euler(0, 0, 0), transform);
                            if (_setupFloortiles)
                                Instantiate(_roomskin[0].floorCorner, node.WorldPoint + Vector3.up * _roomskin[0].floorYoffset, Quaternion.Euler(0, 0, 0), transform);
                        }
                        else
                        if (dungeonMap.grid[node.gridX, node.gridY + 1]._tileType == Node.TileType.Wall && dungeonMap.grid[node.gridX + 1, node.gridY]._tileType == Node.TileType.Wall &&
                            !(dungeonMap.grid[node.gridX, node.gridY - 1]._tileType == Node.TileType.Wall || dungeonMap.grid[node.gridX - 1, node.gridY]._tileType == Node.TileType.Wall))
                        {

                            Instantiate(_roomskin[0].wallCorner, node.WorldPoint, Quaternion.Euler(0, 180, 0), transform);
                            if (_setupFloortiles)
                                Instantiate(_roomskin[0].floorCorner, node.WorldPoint + Vector3.up * _roomskin[0].floorYoffset, Quaternion.Euler(0, 180, 0), transform);
                        }
                        else
                        if (dungeonMap.grid[node.gridX, node.gridY + 1]._tileType == Node.TileType.Wall && dungeonMap.grid[node.gridX - 1, node.gridY]._tileType == Node.TileType.Wall &&
                            !(dungeonMap.grid[node.gridX, node.gridY - 1]._tileType == Node.TileType.Wall || dungeonMap.grid[node.gridX + 1, node.gridY]._tileType == Node.TileType.Wall))
                        {

                            Instantiate(_roomskin[0].wallCorner, node.WorldPoint, Quaternion.Euler(0, 90, 0), transform);
                            if (_setupFloortiles)
                                Instantiate(_roomskin[0].floorCorner, node.WorldPoint + Vector3.up * _roomskin[0].floorYoffset, Quaternion.Euler(0, 90, 0), transform);
                        }
                        else
                        {
                            Instantiate(_roomskin[0].wall, node.WorldPoint, _roomskin[0].wall.transform.rotation, transform);
                            if (_setupFloortiles)
                                Instantiate(_roomskin[0].floor, node.WorldPoint + Vector3.up * _roomskin[0].floorYoffset, _roomskin[0].floor.transform.rotation, transform);
                        }
                    }
                    else
                    {
                        Instantiate(_roomskin[0].wall, node.WorldPoint, _roomskin[0].wall.transform.rotation, transform);
                    }

                    // GameObject.Instantiate(this._roomskin[0].floor, node.WorldPoint + Vector3.up * this._roomskin[0].floorYoffset, this._roomskin[0].floor.transform.rotation, transform);
                    node.instantiated = true;
                }

            if (_setupFloortiles && node._tileType != Node.TileType.None /*&& node._tileType != Node.TileType.Wall*/)
                Instantiate(_roomskin[0].floor, node.WorldPoint + Vector3.up * _roomskin[0].floorYoffset, Quaternion.identity, transform);

            if (_fillVoidWithWalls && node._tileType == Node.TileType.None)
            {
                GameObject wall = Instantiate(_roomskin[0].wall, node.WorldPoint + Vector3.up * _roomskin[0].floorYoffset, Quaternion.identity, transform);
                Collider col = wall.GetComponent<Collider>();
                if (col)
                    Destroy(col);
            }

        }
    }
}

namespace asdf
{

}