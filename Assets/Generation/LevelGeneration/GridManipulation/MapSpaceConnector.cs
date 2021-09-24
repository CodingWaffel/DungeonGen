using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapSpaceConnector : IMapModifier
{
    bool _broadPaths = true;

    public MapSpaceConnector(bool broadPaths = true){
        this._broadPaths = broadPaths;
    }

    public Grid Modify(Grid grid)
    {
        Grid map = grid;
        List<MapShape> shapes = ShapeDetector.DetectShapes(map);
        List<MapShape> nonWallShapes = shapes.Where(shape => !shape.isWall).ToList();
        int biggest = nonWallShapes.Max(shape => shape.Pixels.Count);
        MapShape biggestShape = nonWallShapes.Single(s => s.Pixels.Count == biggest);
        nonWallShapes.Remove(biggestShape);

        Node randomPoint = biggestShape.Pixels[Random.Range(0, biggestShape.Pixels.Count)];

        if(nonWallShapes.Count > 0)
            foreach (MapShape shape in nonWallShapes)
            {
                Node randomPointTarget = shape.Pixels[Random.Range(0, shape.Pixels.Count)];
                foreach (Node node in AStarPathfinder.FindPath(map, randomPoint, randomPointTarget, node => true))
                {
                    if(this._broadPaths){
                        foreach (Node pNode in grid.GetNeighbours(node))
                            pNode.value = 0;
                    }else{
                        node.value = 0;
                    }
                    
                }
                
            }
        return map;
    }
}
