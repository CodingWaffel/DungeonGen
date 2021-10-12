using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Grid
{
    public Node[,] grid;
    Vector3 _worldBottomLeft;

    public Vector2 gridSize;
    public Vector2 gridWorldSize;

    float _nodeRadius = 1f;
    float _nodeDiameter;
    

    public Grid(int[,] baseGrid, float nodeRadius = 1f)
    {
        this.gridSize = new Vector2(baseGrid.GetLength(0), baseGrid.GetLength(1));
        this._nodeRadius = nodeRadius;
        _nodeDiameter = nodeRadius * 2;
        this.gridWorldSize = new Vector3(gridSize.x * this._nodeDiameter, gridSize.y * this._nodeDiameter);
        
        grid = new Node[(int)this.gridSize.x, (int)gridSize.y];

        for(int x = 0; x < GetGridNodes().GetLength(0); x++)
        {
            for(int y = 0; y < GetGridNodes().GetLength(1); y++)
            {
                Vector3 worldPoint = _worldBottomLeft + Vector3.right * (x * _nodeDiameter + nodeRadius) + Vector3.forward * (y * _nodeDiameter + nodeRadius);
                
                this.grid[x, y] = new Node(x, y, baseGrid[x, y], worldPoint);
                
            }
        }
    }

    public virtual Node[,] GetGridNodes() => this.grid;
    public bool IsOnGrid(int xCoord, int yCoord) => xCoord >= 0 && yCoord >= 0 && xCoord < this.gridSize.x && yCoord < this.gridSize.y;
    public virtual Node GetNearestNodeOnGrid(Vector3 position)
    {
        
        float percentX = Mathf.Clamp01(position.x / gridWorldSize.x);
        float percentY = Mathf.Clamp01(position.z / gridWorldSize.y);
        int x = Mathf.RoundToInt((gridSize.x - 1) * percentX);
        int y = Mathf.RoundToInt((gridSize.y - 1) * percentY);
        

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
