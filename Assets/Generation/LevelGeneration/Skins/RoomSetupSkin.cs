using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Room Setup")]
public class RoomSetupSkin : ScriptableObject
{
    public GridGeneratorFactory gridGeneratorFactory;
    public List<MapModifierFactory> modifierFactory;

}
