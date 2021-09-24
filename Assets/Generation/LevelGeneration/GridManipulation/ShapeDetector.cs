using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class ShapeDetector
{
    public static List<MapShape> DetectShapes(Grid grid){
        int width = (int)grid.GetGridNodes().GetLength(0);
        int height = (int)grid.GetGridNodes().GetLength(1);
        List<Node> openSet = new List<Node>(width);
        List<Node> closedSet = new List<Node>(height);
        List<MapShape> detected = new List<MapShape>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                openSet.Add(grid.GetGridNodes()[x, y]);
            }
        }

        while(openSet.Count > 0){
            Node current = openSet[0];
            
                List<Node> shape = FloodFill(ref closedSet, grid, grid.GetGridNodes()[current.gridX, current.gridY].value, current);
                detected.Add(new MapShape(shape, grid.GetGridNodes()[current.gridX, current.gridY].value == 1));
                // closedSet.AddRange(shape);
                foreach (Node p in shape)
                {
                    // openSet.RemoveAll(point => point.gridX == p.gridX && point.gridY == p.gridY);
                    openSet.Remove(p);
                }

            
        }


        // Debug.Log(detected.Count);
        return detected;
    }
    public static List<Node> FloodFill(ref List<Node> closedSet, Grid grid, int sourceColor, Node point) {
        int width = (int)grid.GetGridNodes().GetLength(0);
        int height = (int)grid.GetGridNodes().GetLength(1);
		var q = new System.Collections.Generic.Queue<Node> (width * height);
		q.Enqueue (point);
        List<Node> result = new List<Node>();

		while (q.Count > 0) {
			var p = q.Dequeue();
            result.Add(p);

			var x1 = p.gridX;
			var y1 = p.gridY;
			if (q.Count > width * height) {
				throw new System.Exception ("The algorithm is probably looping. Queue size: " + q.Count);
			}

            Node newPoint;
            if(x1 + 1 > 0 && x1 + 1 < width && y1 > 0 && y1 < height){
                newPoint = grid.GetGridNodes()[x1 + 1, y1];
                if (CheckValidity(closedSet, grid, width, height, newPoint, sourceColor)){
                    q.Enqueue(newPoint);
                    closedSet.Add(newPoint);
                }
            }
			
				
            if(x1 - 1 > 0 && x1 - 1 < width && y1 > 0 && y1 < height){
                newPoint = grid.GetGridNodes()[x1 - 1, y1];
                if (CheckValidity(closedSet, grid, width, height, newPoint, sourceColor)){
                    q.Enqueue(newPoint);
                    closedSet.Add(newPoint);
                }
            }

            if(x1 > 0 && x1 < width && y1 + 1> 0 && y1 + 1 < height){
                newPoint = grid.GetGridNodes()[x1, y1 + 1];
                if (CheckValidity(closedSet, grid, width, height, newPoint, sourceColor)){
                    q.Enqueue(newPoint);
                    closedSet.Add(newPoint);
                }
            }

            if(x1 > 0 && x1 < width && y1 - 1 > 0 && y1 - 1 < height){
                newPoint = grid.GetGridNodes()[x1, y1 - 1];
                if (CheckValidity(closedSet, grid, width, height, newPoint, sourceColor)){
                    q.Enqueue(newPoint);
                    closedSet.Add(newPoint);
                }
            }
            
		}
        return result;
	}
    static bool CheckValidity(List<Node> closedSet, Grid grid, int width, int height, Node p, int sourceColor) {
		if (p.gridX < 0 || p.gridX >= width) {
			return false;
		}
		if (p.gridY < 0 || p.gridY >= height) {
			return false;
		}

		var color = grid.GetGridNodes()[p.gridX, p.gridY].value;

        return !closedSet.Any(point => p.gridX == point.gridX && p.gridY == point.gridY) && sourceColor == color;
	}
}
public struct Point {

    public int x;
    public int y;

    public Point(int x, int y) {
        this.x = x;
        this.y = y;
    }
}
public class MapShape
{
    public List<Node> Pixels{get; private set;}
    public bool isWall;

    public MapShape(List<Node> pixels, bool isWall){
        this.Pixels = pixels;
        this.isWall = isWall;
    }


}
