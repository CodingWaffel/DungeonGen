using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WallColliderRemover : MonoBehaviour
{
    [SerializeField] bool _destroyObject = false;
    void Start()
    {
        if(!Dungeon.DungeonMap.GetNeighbours(Dungeon.DungeonMap.GetNearestNodeOnGrid(transform.position), true).Any(n => n.value == 0)){
            if(this._destroyObject){
                Destroy(gameObject);
            }else{
                GetComponent<Collider>().enabled = false;
            }
            
        }
    }

}
