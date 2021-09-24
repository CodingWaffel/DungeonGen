using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Grid
{
    public Vector2 gridWorldSize;
    float _nodeRadius = 1f;
    public Node[,] grid;
    float _nodeDiameter;
    int _gridSizeX, _gridSizeY;
    Vector3 _worldBottomLeft;

    public Grid(int[,] baseGrid, float nodeRadius = 1f)
    {
        this.gridWorldSize = new Vector2(baseGrid.GetLength(0), baseGrid.GetLength(1));
        this._nodeRadius = nodeRadius;
        _nodeDiameter = nodeRadius * 2;
        _gridSizeX = Mathf.RoundToInt(this.gridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(this.gridWorldSize.y / _nodeDiameter); 
        // _gridSizeX = Mathf.RoundToInt(this.gridSize.x);
        // _gridSizeY = Mathf.RoundToInt(this.gridSize.y);  
        this._worldBottomLeft = Vector3.zero;// - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        
        grid = new Node[(int)this.gridWorldSize.x, (int)gridWorldSize.y];

        for(int x = 0; x < GetGridNodes().GetLength(0); x++)
        {
            for(int y = 0; y < GetGridNodes().GetLength(1); y++)
            {
                Vector3 worldPoint = _worldBottomLeft + Vector3.right * (x * _nodeDiameter + nodeRadius) + Vector3.forward * (y * _nodeDiameter + nodeRadius);
                
                GetGridNodes()[x, y] = new Node(x, y, baseGrid[x, y], worldPoint);
                
            }
        }
    }

    public Grid(){
        // int[,] randomGrid = new int[64,64];
        // for (int x = 0; x < randomGrid.GetLength(0); x++)
        //     for (int y = 0; y < randomGrid.GetLength(1); y++)
        //         randomGrid[x,y] = Random.Range(0f, 1f) > .5f ? 1 : 0;

        // this.gridWorldSize = new Vector2(randomGrid.GetLength(0), randomGrid.GetLength(1));
        // this._nodeRadius = 1;
        // _nodeDiameter = this._nodeRadius * 2;
        // _gridSizeX = Mathf.RoundToInt(this.gridWorldSize.x / _nodeDiameter);
        // _gridSizeY = Mathf.RoundToInt(this.gridWorldSize.y / _nodeDiameter); 
        // // _gridSizeX = Mathf.RoundToInt(this.gridSize.x);
        // // _gridSizeY = Mathf.RoundToInt(this.gridSize.y);  
        // this._worldBottomLeft = Vector3.zero;// - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        
        // grid = new Node[(int)this.gridWorldSize.x, (int)gridWorldSize.y];

        // for(int x = 0; x < GetGridNodes().GetLength(0); x++)
        // {
        //     for(int y = 0; y < GetGridNodes().GetLength(1); y++)
        //     {
        //         Vector3 worldPoint = _worldBottomLeft + Vector3.right * (x * _nodeDiameter + _nodeRadius) + Vector3.forward * (y * _nodeDiameter + _nodeRadius);
                
        //         GetGridNodes()[x, y] = new Node(x, y, randomGrid[x, y], worldPoint);
                
        //     }
        // }
    }

    public virtual Node[,] GetGridNodes() => this.grid;

    public virtual Node GetNearestNodeOnGrid(Vector3 position)
    {
        // position -= this._worldBottomLeft;
        // float percentX = Mathf.Clamp01((position.x + gridWorldSize.x / 2) / gridWorldSize.x);
        // float percentY = Mathf.Clamp01((position.z + gridWorldSize.y / 2) / gridWorldSize.y);
        float percentX = Mathf.Clamp01(position.x / gridWorldSize.x);
        float percentY = Mathf.Clamp01(position.z / gridWorldSize.y);
        int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);
        

        return GetGridNodes()[x, y];

    }


    public virtual List<Node> GetNeighbours(Node node, bool rect = false)
    {
        List<Node> neighbours = new List<Node>();

        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(!rect)
                    if (Mathf.Abs(x) == Mathf.Abs(y))  continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if(checkX >= 0 && checkX < this.GetGridNodes().GetLength(0) && checkY >= 0 && checkY < this.GetGridNodes().GetLength(1))
                {
                    neighbours.Add(GetGridNodes()[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

}
