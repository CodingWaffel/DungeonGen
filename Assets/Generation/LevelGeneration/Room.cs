using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : Grid
{
    Grid _parentGrid;
    int _xPos, _yPos, _width, _height;
    Node[,] _subGrid;
    public Room(Grid grid, int xPos, int yPos, int width, int height) : base()
    {
        this._parentGrid = grid;
        this._xPos = xPos;
        this._yPos = yPos;
        this._width = width;
        this._height = height;

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

    public override Node[,] GetGridNodes() => this._subGrid;

    public override Node GetNearestNodeOnGrid(Vector3 position)
    {
        float percentX = Mathf.Clamp01(position.x / this._width);
        float percentY = Mathf.Clamp01(position.z / this._height);
        int x = Mathf.RoundToInt((this._width - 1) * percentX);
        int y = Mathf.RoundToInt((this._height - 1) * percentY);
        

        return GetGridNodes()[x, y];

    }
}
