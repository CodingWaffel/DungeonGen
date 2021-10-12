using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDungeonGenerator : DungeonGenerator
{
    [SerializeField] int _blurFactor;
    [SerializeField] int _polygonReduction;
    [SerializeField] float _wallHeight;
    [SerializeField] Material _dungeonMat;
    public override void GenerateDungeon(Grid dungeonMap)
    {
        MapGen.MapGenerator mapGen = new MapGen.MapGenerator();
        int width = dungeonMap.grid.GetLength(0);
        int height = dungeonMap.grid.GetLength(1);
        float[,] heightMap = new float[width, height];
        for (int x = 0; x < width; x++) 
            for (int y = 0; y < height; y++)
                if(dungeonMap.grid[x,y]._tileType == Node.TileType.Floor){
                    heightMap[x,y] = 0;
                }else{
                    heightMap[x,y] = 1;
                }
                // heightMap[x,y] = Mathf.Abs(dungeonMap.grid[x,y].value);
            
        
        heightMap = Scale(heightMap, (int)base.NodeRadius);
        heightMap = Blur(heightMap, this._blurFactor);

        
        mapGen.GenerateMap(heightMap, this._polygonReduction, this._dungeonMat);
    }

    float[,] Blur(float[,] input){
        int width = input.GetLength(0);
        int height = input.GetLength(1);
        float[,] smoothed = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float val = 0f;
                
                if(x-1 >= 0 && x+1 < width && y-1 >= 0 && y+1 < height){

                    for(int i = x-1; i <= x+1; i++){
                        for (int j = y-1; j <= y+1; j++)
                        {
                            val += input[i, j];
                        }
                    }
                    smoothed[x,y] = val/9f;
                }
            }
        }
        return smoothed;
    }
    float[,] Blur(float[,] input, int strength){
        for (int i = 0; i < strength; i++)
        {
            input = Blur(input);
        }
        return input;
    }
    float[,] Scale(float[,] input, int scalefactor){
        if(scalefactor < 1)
            scalefactor = 1;
        int width = input.GetLength(0);
        int height = input.GetLength(1);
        float[,] scaled = new float[width * scalefactor, height * scalefactor];
        for (int i = 0; i < width * scalefactor; i+=scalefactor)
        {
            for (int j = 0; j < height * scalefactor; j+=scalefactor)
            {
                float val = (input[i/scalefactor,j/scalefactor]) * this._wallHeight;
                for (int x = 0; x < scalefactor; x++)
                {
                    for (int y = 0; y < scalefactor; y++)
                    {
                        scaled[i+x,j+y] = val;
                    }
                } 
            }
        }
        return scaled;
    }
}
