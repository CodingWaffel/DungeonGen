using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public Room Start{get;}
    public Room Destination{get;}
    public List<Node> PathNodes {get;}

    public Path(List<Node> path, Room start, Room destionation){
        this.PathNodes = path;
        this.Start = start;
        this.Destination = destionation;
    }
}
