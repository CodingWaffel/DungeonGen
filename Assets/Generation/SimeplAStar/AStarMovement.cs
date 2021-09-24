using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class AStarMovement : MonoBehaviour
{
    [SerializeField] float _speed = 5f;
    Rigidbody _rigidbody;
    Transform _transform;
    Vector3[] _path;
    int _iterator = 0;
    System.Action _onMovement, _onIdle, _callbackOnTargetReached;

    void Awake() {
        this._transform = transform;
        this._rigidbody = GetComponent<Rigidbody>();
        this._callbackOnTargetReached = this.Nothing;
    }
    void FixedUpdate(){
        if(!(this._path is null)){
            this.Move(Time.fixedDeltaTime);  
        }else{
            this._onIdle();
        }
    }

    public AStarMovement InitAnimation(System.Action onMovement, System.Action onIdle){
        this._onMovement = onMovement is null ? this.Nothing : onMovement;
        this._onIdle = onIdle is null? this.Nothing : onIdle;
        return this;
    }

    void Move(float timeStep){
        if(this._path.Length <= this._iterator+1){
            this._path = null;
            this._callbackOnTargetReached();
            return;
        }
        this._rigidbody.MovePosition(this._rigidbody.position + (this._path[this._iterator+1] - this._rigidbody.position).normalized * timeStep * this._speed);
        transform.LookAt(this._path[this._iterator+1]);

        if(Vector3.Distance(this._rigidbody.position, this._path[this._iterator + 1]) <= .5f){
            this._iterator++;  
        }
    }

    void Nothing(){}


}
