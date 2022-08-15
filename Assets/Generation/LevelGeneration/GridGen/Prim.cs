using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VonPrim : IRoomGenerator
{
    float _pointDenseness = .85f;
    float _pointFrequency = 1f;
    int _width = 128, _height = 128;
    bool _boradenPaths = true;

    int[,] Fill(int width, int height){
        int[,] grid = new int[width, height];
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                grid[i,j] = 1;
        return grid;
    }

    public VonPrim(int width, int height, float pointDenseness, float pointFrequency, bool broadenPaths = true){
        this._width = width;
        this._height = height;
        this._pointDenseness = pointDenseness;
        this._boradenPaths = broadenPaths;
        this._pointFrequency = pointFrequency;
    }
    
	static int GetMinimumIndex(int[] distances, bool[] includedNodes)
	{
		int minimumDistance = int.MaxValue;
		int minimumIndex = -1;
		for (int i = 0; i < distances.Length; i++)
		{
			if (!includedNodes[i] && distances[i] < minimumDistance)
			{
				minimumDistance = distances[i];
				minimumIndex = i;
			}
		}
		return minimumIndex;
	}
	
    
	public static void RunPrim(int[,] distanceMatrix, int numberOfNodes, out int[] parent, out int[] distances)
	{
		parent = new int[numberOfNodes];
		distances = new int[numberOfNodes];
		bool[] includedNodes = new bool[numberOfNodes];
		for (int i = 0; i < numberOfNodes; i++)
		{
			distances[i] = int.MaxValue;
			includedNodes[i] = false;
		}
		distances[0] = 0;
		parent[0] = -1;
		for (int i = 0; i < numberOfNodes - 1; i++)
		{
			int minimumIndex = GetMinimumIndex(distances, includedNodes);
			includedNodes[minimumIndex] = true;
			for (int j = 0; j < numberOfNodes; j++)
			{
				if (distanceMatrix[minimumIndex, j] != 0 && !includedNodes[j] && distanceMatrix[minimumIndex, j] < distances[j])
				{
					parent[j] = minimumIndex;
					distances[j] = distanceMatrix[minimumIndex, j];
				}
			}
		}
	}
	
    
	public Grid GenerateRoom(int width, int height, float nodeRadius)
	{
        Grid grid = new Grid(Fill(width, height), nodeRadius);
        // Grid grid = new Grid(PerlinFilledMatrix(this._width, this._height, 0, 0, 5), nodeRadius);
        Node[] waypoints = GetRandomPoints(grid, (int)this._pointFrequency);
        // Node[] waypoints = PerlinCalculatedWaypoints(grid, this._width, this._height, this._pointFrequency);
        foreach (Node node in waypoints)
            node.value = 0;
            
        
        // for (int i = 0; i < waypoints.GetLength(0)-2; i++){
        //     waypoints[i] = grid.grid[Random.Range(5, this._width-5), Random.Range(5, this._height-5)];
        //     waypoints[i].value = 0;
        // }
        waypoints[waypoints.GetLength(0)-2] = grid.GetGridNodes()[width-1, Random.Range(0, height)];
        waypoints[waypoints.GetLength(0)-1] = grid.GetGridNodes()[0, Random.Range(0, height)];
            
		// int[, ] distanceMatrix = RandomDistanceMatrix(this._amountOfPoints);
        int[, ] distanceMatrix = RealDistanceMatrix(grid, waypoints);
        int[] parent;
		int[] distances;
		RunPrim(distanceMatrix, waypoints.GetLength(0), out parent, out distances);
        Debug.Log(waypoints.GetLength(0));
		for (int i = 1; i < waypoints.GetLength(0); i++)
		{
            foreach (Node node in AStarPathfinder.FindPath(grid, waypoints[parent[i]], waypoints[i], (Node node) => true, false))
            {
                if(this._boradenPaths)
                    foreach (Node neighbour in grid.GetNeighbours(node, true))
                        neighbour.value = 0;
                node.value = 0;    
            }
            
		}
        return grid;	
	}

    Node[] GetRandomPoints(Grid grid, int amount){
        Node[] result = new Node[amount];
        for (int i = 0; i < amount; i++)
        {
            result[i] = grid.GetGridNodes()[Random.Range(1, grid.GetGridNodes().GetLength(0)-2), Random.Range(1, grid.GetGridNodes().GetLength(1)-2)];
        }
        return result;
    }

    int[,] RandomDistanceMatrix(int size){
        int[,] distanceMatrix = new int[size, size];
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                distanceMatrix[x, y] = x == y ? 0 : Random.Range(1, 50);
        return distanceMatrix;
    }
    int[,] RealDistanceMatrix(Grid grid, Node[] waypoints){
        int[,] distanceMatrix = new int[waypoints.Length, waypoints.Length];
        for (int x = 0; x < distanceMatrix.GetLength(0); x++)
        {
            for (int y = 0; y < distanceMatrix.GetLength(1); y++)
            {
                if(x == y){
                    distanceMatrix[x, y] = 0;
                    continue;
                } 
                distanceMatrix[x, y] = (int)Vector3.Distance(grid.GetGridNodes()[waypoints[x].gridX, waypoints[x].gridY].WorldPoint, grid.GetGridNodes()[waypoints[y].gridX, waypoints[y].gridY].WorldPoint);

            }
        }
        return distanceMatrix;
    }

    int[,] PerlinFilledMatrix(int widthIn, int heightIn, float xOrg, float yOrg, float scale){
        // For each pixel in the texture...
        int[,] result = new int[widthIn,heightIn];
        float width = (float)widthIn;
        float height = (float)heightIn;
        float y = 0.0F;

        while (y < height)
        {
            float x = 0.0F;
            while (x < width)
            {
                float xCoord = xOrg + x / width * scale;
                float yCoord = yOrg + y / height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                result[(int)x, (int)y] = Mathf.PerlinNoise(xCoord, yCoord) > this._pointDenseness ? 1 : 0;
                x++;
            }
            y++;
        }
        return result;
    }

    Node[] PerlinCalculatedWaypoints(Grid grid, int widthIn, int heightIn, float scale){
        List<Node> result = new List<Node>();
        float width = (float)widthIn;
        float height = (float)heightIn;
        float y = 0.0F;
        Debug.Log(this._pointDenseness);
        while (y < height)
        {
            float x = 0.0F;
            while (x < width)
            {
                // float xCoord = xOrg + x / width * scale;
                // float yCoord = yOrg + y / height * scale;
                float xCoord = Random.Range(0f, 1000f) + x / width * scale;
                float yCoord = Random.Range(0f, 1000f) + y / height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                if(Mathf.PerlinNoise(xCoord, yCoord) > this._pointDenseness){
                    result.Add(grid.GetGridNodes()[(int)x, (int)y]);
                }
                
                x++;
            }
            y++;
        }
        Debug.Log(result.Count);
        return result.ToArray();
    }

}