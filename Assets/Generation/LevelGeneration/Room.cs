using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    Grid _parentGrid;
    int _xPos, _yPos, _width, _height;
    Node[,] _subGrid;

    public int XPos => this._xPos;
    public int YPos => this._yPos;
    public int Width => this._width;
    public int Height => this._height;
    public Vector3 Position => new Vector3(XPos + Width/2, 0 , YPos + Height/2);
    public Room(Grid grid, int xPos, int yPos, int width, int height)
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

    public void Create(Transform parent, RoomSkin skin){

        for (int x = 0; x < this.Width; x++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                Node node = this._parentGrid.grid[this._xPos + x, this._yPos + y];
                if(node.value == 1){
                    GameObject.Instantiate(skin.wall, node.WorldPoint + Vector3.up * skin.wallYoffset, skin.wall.transform.rotation, parent);
                    node.instantiated = true;
                }
                if(node.value == 0){
                    GameObject.Instantiate(skin.floor, node.WorldPoint, skin.floor.transform.rotation, parent);
                    node.instantiated = true;
                }
            }
        }


        // foreach (Node node in this._subGrid)
        // {
        //     if(node.value == 1){
        //         GameObject.Instantiate(skin.wall, node.WorldPoint + Vector3.up * skin.wallYoffset, skin.wall.transform.rotation, parent);
        //     }
        //     if(node.value == 0){
        //         GameObject.Instantiate(skin.floor, node.WorldPoint, skin.floor.transform.rotation, parent);

        //     }
        // }
    }
}
