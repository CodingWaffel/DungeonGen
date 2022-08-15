using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Room Skin")]
public class RoomSkin : ScriptableObject
{
    public GameObject wall;
    public GameObject wallCorner;
    public GameObject floor;
    public GameObject floorCorner;
    public float floorYoffset = 0f;
    

}
