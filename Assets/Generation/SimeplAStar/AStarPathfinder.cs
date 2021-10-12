using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinder
{
    public static float weightMultiplier = 0f;

    public static List<Node> FindPath(Grid grid, Node startNode, Node targetNode, System.Func<Node, bool> walkableRequirement, bool includeDiagonals = false)
    {

        //init open/closed Set
        Heap<Node> openSet = new Heap<Node>((int) (grid.gridSize.x * grid.gridSize.y));
        Heap<Node> closedSet = new Heap<Node>((int)(grid.gridSize.x * grid.gridSize.y));

        openSet.Add(startNode);


        while(openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirstItem();
            closedSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                return TraversePath(startNode, targetNode);
            }

            foreach(Node neighbour in grid.GetNeighbours(currentNode, includeDiagonals))
            {
                if (!walkableRequirement(neighbour) || closedSet.Contains(neighbour)) continue;

                float newMovementCostToNeighbour = (currentNode.gCost + neighbour.weight * weightMultiplier + GetDistance(currentNode, neighbour));
                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                    else
                    {
                        openSet.UpdateItem(neighbour);
                    }
                }
            }
        }

        // Debug.LogError("No Path, still need to implement No Path Handling!!");
        return null;
    }
  

    static List<Node> TraversePath(Node start, Node target)
    {
        List<Node> path = new List<Node>();
        Node currentNode = target;
        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Add(start);
        
        path.Reverse();
        return path;

    }

    static float GetDistance(Node a, Node b)
    {
        //return Vector3.Distance(a._worldPosition, b._worldPosition);
        int distX = Mathf.Abs(a.gridX - b.gridX);
        int distY = Mathf.Abs(a.gridY - b.gridY);

        if (distX > distY) return 14 * distY + 10 * (distX - distY);
        else return 14 * distX + 10 * (distY - distX);
    }

    public static Vector3[] PathSmoothChaikin(Vector3[] pts) {
        if(pts.Length <= 2) return pts;
         Vector3[] newPts = new Vector3[(pts.Length - 2) * 2 + 2];
         newPts[0] = pts[0];
         newPts[newPts.Length-1] = pts[pts.Length-1];
 
         int j = 1;
         for (int i = 0; i < pts.Length - 2; i++) {
             newPts[j] = pts[i] + (pts[i+1] - pts[i]) * 0.75f;
             newPts[j+1] = pts[i+1] + (pts[i+2] - pts[i+1]) * 0.25f;
             j += 2;
         }
         return  newPts;
     }
    public static Vector3[] PathSmoothChaikin(List<Node> path, int iterations = 1){
        if(path is null) return null;
        Vector3[] result = path.ConvertAll(n => n.WorldPoint).ToArray();
        for (int i = 0; i < iterations; i++)
        {
            result = PathSmoothChaikin(result);
        }
        return result;
     }



}
