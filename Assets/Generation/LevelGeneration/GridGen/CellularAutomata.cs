using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CellularAutomata : IGridGenerator, IMapModifier
{
    string _seed;
    bool _useRandomSeed;
    [Range(0,100)] int _fillPercentage = 50;
    int _smoothingIteration = 4, _smoothingStrength = 4;
    int _width = 128, _height = 128;
    int[,] _grid;//0 == empty, 1 == filled

    public CellularAutomata(int width, int height, int fillpercentage = 45, int smoothingStrength = 4, int smoothingiterations = 4){
        this._width = width;
        this._height = height;
        this._fillPercentage = fillpercentage;
        this._smoothingStrength = smoothingStrength;
        this._smoothingIteration = smoothingiterations;
        this._useRandomSeed = true;
    }
    public CellularAutomata(int width, int height, string seed, int fillpercentage = 45, int smoothingStrength = 4, int smoothingiterations = 4){
        this._width = width;
        this._height = height;
        this._fillPercentage = fillpercentage;
        this._smoothingStrength = smoothingStrength;
        this._smoothingIteration = smoothingiterations;
        this._seed = seed;
        this._useRandomSeed = false;
    }

    public Grid GenerateMap(int width, int height, float nodeRadius){
        this._grid = this.RandomFill(width, height, this._fillPercentage);
        for (int i = 0; i < this._smoothingIteration; i++)
        {
            this._grid = this.SmoothMap(this._grid);
        }
        return new Grid(this._grid, nodeRadius);
    }

    int[,] RandomFill(int width, int height, int fillPercentage){
        if(this._useRandomSeed)
            this._seed = UnityEngine.Random.Range(0f, 5000000f).ToString();// Time.timeAsDouble.ToString();
        int[,] randomFill = new int[width, height];
        System.Random prng = new System.Random(this._seed.GetHashCode());
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(x == 0 || x == width-1 || y == 0 || y == height - 1){
                    randomFill[x,y] = 1;
                }else{
                    randomFill[x,y] = prng.Next(0, 100) < fillPercentage ? 1 : 0;
                }
            }
        }
        return randomFill;
    }

    int[,] SmoothMap(int[,] input){
        int[,] map = new int[input.GetLength(0), input.GetLength(1)];
        int width = input.GetLength(0);
        int height = input.GetLength(1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(this.GetNeighbourFillCount(input, x, y) > this._smoothingStrength){
                    map[x,y] = 1;
                }else{
                    map[x, y] = 0;
                }
            }
        }

        return map;
    }

    int GetNeighbourFillCount(int[,] input, int inX, int inY){
        int count = 0;
        for (int x = inX - 1; x <= inX + 1; x++)
        {
            for (int y = inY - 1; y <= inY + 1; y++)
            {
                if(x < 0 || x >= input.GetLength(0) || y < 0 || y >= input.GetLength(1)){
                    count++;
                }else if(x != inX || y != inY){
                    count += input[x,y];
                }
            }
        }

        return count;
    }

    void OnDrawGizmos() {
        if(this._grid is null) return;
        Gizmos.color = Color.black;
        for (int x = 0; x < this._width; x++)
        {
            for (int y = 0; y < this._height; y++)
            {
                if(this._grid[x,y] == 0) continue;
                Vector3 pos = new Vector3(-this._width/2 + x + .5f, 0, -_height/2 +y + .5f);
                Gizmos.DrawCube(pos, Vector3.one);
            }
        }
    }

    public Grid Modify(Grid grid)
    {
        for (int i = 0; i < this._smoothingIteration; i++)
        {
            grid.grid = this.SmoothMap(grid.GetGridNodes());
        }
        return grid;
    }
    Node[,] SmoothMap(Node[,] input){
        Node[,] map = new Node[input.GetLength(0), input.GetLength(1)];
        
        int width = input.GetLength(0);
        int height = input.GetLength(1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x,y] = input[x,y];
                if(this.GetNeighbourFillCount(input, x, y) > this._smoothingStrength){
                    map[x,y].value = 1;
                }else{
                    map[x, y].value = 0;
                }
            }
        }

        return map;
    }
    int GetNeighbourFillCount(Node[,] input, int inX, int inY){
        int count = 0;
        for (int x = inX - 1; x <= inX + 1; x++)
        {
            for (int y = inY - 1; y <= inY + 1; y++)
            {
                if(x < 0 || x >= input.GetLength(0) || y < 0 || y >= input.GetLength(1)){
                    count++;
                }else if(x != inX || y != inY){
                    count += input[x,y].value;
                }
            }
        }

        return count;
    }
}
