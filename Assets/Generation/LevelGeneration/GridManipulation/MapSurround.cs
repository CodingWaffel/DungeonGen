using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSurround : IMapModifier
{
    public Grid Modify(Grid grid)
    {
        for (int x = 0; x < grid.GetGridNodes().GetLength(0); x++){
            grid.GetGridNodes()[x,0].value = 1;
            grid.GetGridNodes()[x,grid.GetGridNodes().GetLength(1)-1].value = 1;
        }
        for (int y = 0; y < grid.GetGridNodes().GetLength(1); y++){
            grid.GetGridNodes()[0,y].value = 1;
            grid.GetGridNodes()[grid.GetGridNodes().GetLength(0)-1, y].value = 1;
        }   
                
                
            
        return grid;
    }
}
