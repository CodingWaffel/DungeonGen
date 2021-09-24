using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class Node : IHeapItem<Node>{

    public int gridX, gridY;

    public float gCost;
    public float hCost;
    public Node parent;
    public int value;
    public Vector3 WorldPoint{get;}

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
