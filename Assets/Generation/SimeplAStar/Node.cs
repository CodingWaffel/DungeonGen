using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class Node : IHeapItem<Node>{

    //---basic values----------
    public int gridX, gridY;
    public Vector3 WorldPoint{get;}


    //----A* parameter-----------
    public float gCost;
    public float hCost;
    public Node parent;
    public float weight;

    //----inherent values-----
    public enum NodeType{Room, Path, Door, None}
    public enum TileType{Wall, Floor, None}
    public NodeType _nodeType = NodeType.None;
    public TileType _tileType = TileType.None;
    public int value;
    public bool instantiated = false;

    //---------------------
    
    public Node(int x, int y, int value, Vector3 worldPoint){
        this.gridX = x;
        this.gridY = y;
        this.value = value;
        this.WorldPoint = worldPoint;
    }
    int heapIndex;

    public float fCost {
        get {
            return gCost + hCost;
        }
    }

    public int HeapIndex {
        get {
            return heapIndex;
        }

        set {
            heapIndex = value;
        }
    }

    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }

        return -compare;
    }


}
