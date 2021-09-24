using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class RoomGenerator : MonoBehaviour
{
    Grid _map;
    [SerializeField] GameObject[] _wallTiles;
    [SerializeField] GameObject _floorTile;
    [SerializeField] float _tileYoffset = 0f;
    [SerializeField] [Range(0f, 1f)] float _nodeSize = 1f;
    [SerializeField] float _tileSize = 2.5f;
    
    [SerializeField] GridGeneratorFactory _gridGenerator;
    [SerializeField] MapModifierFactory[] _gridModifier; 
    [SerializeField] List<MapSetupFactoryAsset> _mapSetupFactories;
    [SerializeField] PlayerController _player;
    List<IMapSetupFactory> _mapSetup; 
    public List<Node> _spawnPoints;

    public Room GenerateNewRoom(Grid grid, int xPos, int yPos, int width, int height){
        return null;
    }

    void Awake(){
        this._mapSetup = this._mapSetupFactories.ConvertAll(fac => fac.GetFactory());
        _map = this.CreateGrid(64, 64, this._gridGenerator, this._gridModifier);
        FillMap(_map);
        for (int x = 0; x < _map.GetGridNodes().GetLength(0); x++)
        {
            for (int y = 0; y < _map.GetGridNodes().GetLength(1); y++)
            {
                if(_map.GetGridNodes()[x,y].value == 0){
                    Instantiate(this._player, _map.GetGridNodes()[x,y].WorldPoint + Vector3.up, Quaternion.identity);
                    return;
                }
            }
        }
 
    }

    Grid CreateGrid(int width, int height, GridGeneratorFactory gridGenerator, MapModifierFactory[] mapModifier){
        Grid grid = gridGenerator.Create().GenerateMap(width, height, this._nodeSize/2f);
        foreach (MapModifierFactory modifier in mapModifier)
            grid = modifier.Create().Modify(grid);
        return grid;
    }

    List<Node> GetSpawnPoints(){
        List<MapShape> shapes = ShapeDetector.DetectShapes(_map);
        return shapes.First(shape => !shape.isWall).Pixels;
    }

    List<Node> SetupMap(List<Node> spawnPoints){
        this.FillMap(_map);
        foreach (IMapSetupFactory mapSetup in this._mapSetup)
            spawnPoints = mapSetup.Create().Setup(_map, spawnPoints);
        return spawnPoints;
    }
    // List<Node> SpawnPlayer(List<Node> spawnPoints){
    //     Node playerSpawn = spawnPoints[Random.Range(0, spawnPoints.Count)];
    //     spawnPoints.Remove(playerSpawn);
    //     Instantiate(this._player, playerSpawn.WorldPoint, Quaternion.identity);
    //     return spawnPoints;
    // }

    void FillMap(Grid map){
        int width = map.GetGridNodes().GetLength(0);
        int height = map.GetGridNodes().GetLength(1);
        Node maxNode = map.GetGridNodes()[width - 1, height - 1];
        Node minNode = map.GetGridNodes()[0,0];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = map.GetGridNodes()[x,y].WorldPoint;
                position.x *= this._tileSize;
                position.z *= this._tileSize;
                if(!(position.x > maxNode.WorldPoint.x || position.z > maxNode.WorldPoint.z) && !(map.GetGridNodes()[x,y].value == 1))
                    Instantiate(this._floorTile, position + Vector3.up * this._tileYoffset, Quaternion.identity, transform);
                // if(map.grid[x,y].value == 1)
                //     Instantiate(this._wallTiles[Random.Range(0, this._wallTiles.Length)], map.grid[x,y].WorldPoint + new Vector3(Random.Range(-.5f, .5f), 0, Random.Range(-.5f, .5f)), Quaternion.identity, transform);
                if(map.GetGridNodes()[x,y].value == 1)
                    Instantiate(this._wallTiles[Random.Range(0, this._wallTiles.Length)], map.GetGridNodes()[x,y].WorldPoint, Quaternion.identity, transform);
            }
        }
    }

    void Update(){
        if(!Input.GetKeyDown(KeyCode.G)) return;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnDrawGizmos() {
        // if(map is null) return;

        // foreach (Node node in map.grid)
        // {
        //     Gizmos.color = node.value == 0 ? Color.white : Color.black;
        //     Gizmos.DrawCube(node.WorldPoint, Vector3.one);
        // }
        
    }

}
