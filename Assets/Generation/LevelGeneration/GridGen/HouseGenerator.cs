using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseGenerator : IRoomGenerator
{
    float _fillPercentage;
    public HouseGenerator(float fillpercentage){
        this._fillPercentage = Mathf.Clamp01(fillpercentage);
    }
    public Grid GenerateRoom(int width, int height, float nodeRadius)
    {
        // int[,] randomgrid = RandomFill(width, height,this._fillPercentage);
        int[,] startingGrid = RandomFill(width, height, -1f);

        SubGrid baseGrid = new SubGrid();
        baseGrid.origin = new Vector2Int(0,0);
        baseGrid.subgrid = startingGrid;

        Grid grid = new Grid(CombineHorizontal(Split(baseGrid, 16, true)).subgrid, nodeRadius);


        return grid;
    }

    int[,] RandomFill(int width, int height, float fillpercentage = .5f){
        int[,] grid = new int[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++){
                if(0 == x || x == width-1 || 0 == y || y == height){
                    grid[x,y] = 1;
                }else{
                    grid[x,y] = Random.Range(0f, 1f) <= fillpercentage ? 1 : 0 ;
                }
            }
        return grid;
    }

    (SubGrid, SubGrid) Split(SubGrid grid, int minimumRoomSize, bool horizontal){
        if(grid.subgrid.GetLength(0) * grid.subgrid.GetLength(1) / 2f < minimumRoomSize){
            SubGrid emptySubgrid = new SubGrid();
            emptySubgrid.origin = Vector2Int.zero;
            emptySubgrid.subgrid = new int[0,0];
            return (grid, emptySubgrid);
        }

        //TODO! Terminierungsargument!!!

        int height = grid.subgrid.GetLength(1);
        int width = grid.subgrid.GetLength(0);

        if(horizontal){
            
            SubGrid a = new SubGrid();
            a.subgrid = RandomFill(width, height/2, -1);
            a.origin = grid.origin;
            SubGrid b = new SubGrid();
            b.subgrid = RandomFill(width, height/2, -1);
            b.origin = new Vector2Int(grid.origin.x, grid.origin.y + height/2);

            return(CombineHorizontal(Split(a, minimumRoomSize, false)), CombineHorizontal(Split(b, minimumRoomSize, false)));
        }else{
            
            SubGrid a = new SubGrid();
            a.subgrid = RandomFill(width/2, height, -1);
            a.origin = grid.origin;
            SubGrid b = new SubGrid();
            b.subgrid = RandomFill(width/2, height, -1);
            b.origin = new Vector2Int(grid.origin.x + width/2, grid.origin.y);

            return(CombineVertical(Split(a, minimumRoomSize, true)), CombineVertical(Split(b, minimumRoomSize, true)));
        }
    }

    SubGrid CombineHorizontal(SubGrid a, SubGrid b){
        SubGrid newSubgrid = new SubGrid();
        if(a.origin.y < b.origin.y){
            newSubgrid.origin = a.origin;
            int[,] subgrid = new int[a.subgrid.GetLength(0), a.subgrid.GetLength(1) + b.subgrid.GetLength(1)];
            for (int x = 0; x < a.subgrid.GetLength(0); x++)
            {
                for (int y = 0; y < a.subgrid.GetLength(1); y++)
                {
                    subgrid[x,y] = a.subgrid[x,y];
                }
            }
            for (int x = 0; x < b.subgrid.GetLength(0); x++)
            {
                for (int y = 0; y < b.subgrid.GetLength(1); y++)
                {
                    subgrid[x,y+a.subgrid.GetLength(1)] = b.subgrid[x,y];
                }
            }
        }else{
            newSubgrid.origin = b.origin;
            int[,] subgrid = new int[a.subgrid.GetLength(0), a.subgrid.GetLength(1) + b.subgrid.GetLength(1)];
            for (int x = 0; x < a.subgrid.GetLength(0); x++)
            {
                for (int y = 0; y < b.subgrid.GetLength(1); y++)
                {
                    subgrid[x,y] = b.subgrid[x,y];
                }
            }
            for (int x = 0; x < a.subgrid.GetLength(0); x++)
            {
                for (int y = 0; y < a.subgrid.GetLength(1); y++)
                {
                    subgrid[x,y+b.subgrid.GetLength(1)] = a.subgrid[x,y];
                }
            }
        }
        return newSubgrid;
    }
    SubGrid CombineHorizontal((SubGrid, SubGrid) grids) => CombineHorizontal(grids.Item1, grids.Item2);
    SubGrid CombineVertical(SubGrid a, SubGrid b){
        SubGrid newSubgrid = new SubGrid();
        if(a.origin.y < b.origin.y){
            newSubgrid.origin = a.origin;
            int[,] subgrid = new int[a.subgrid.GetLength(0)+ b.subgrid.GetLength(0), a.subgrid.GetLength(1)];
            for (int x = 0; x < a.subgrid.GetLength(0); x++)
            {
                for (int y = 0; y < a.subgrid.GetLength(1); y++)
                {
                    subgrid[x,y] = a.subgrid[x,y];
                }
            }
            for (int x = 0; x < b.subgrid.GetLength(0); x++)
            {
                for (int y = 0; y < b.subgrid.GetLength(1); y++)
                {
                    subgrid[x+a.subgrid.GetLength(0),y] = b.subgrid[x,y];
                }
            }
        }else{
            newSubgrid.origin = b.origin;
            int[,] subgrid = new int[a.subgrid.GetLength(0) + b.subgrid.GetLength(0), a.subgrid.GetLength(1)];
            for (int x = 0; x < a.subgrid.GetLength(0); x++)
            {
                for (int y = 0; y < b.subgrid.GetLength(1); y++)
                {
                    subgrid[x,y] = b.subgrid[x,y];
                }
            }
            for (int x = 0; x < a.subgrid.GetLength(0); x++)
            {
                for (int y = 0; y < a.subgrid.GetLength(1); y++)
                {
                    subgrid[x+b.subgrid.GetLength(0),y] = a.subgrid[x,y];
                }
            }
        }
        return newSubgrid;
    }
    SubGrid CombineVertical((SubGrid, SubGrid) grids) => CombineVertical(grids.Item1, grids.Item2);


    struct SubGrid{
        public int[,] subgrid;
        public Vector2Int origin;
    }
}
