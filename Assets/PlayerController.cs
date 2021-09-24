using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float _speed = 10f;
    Rigidbody _rb;

    private void Awake() {
        this._rb = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this._rb.MovePosition(this._rb.position + (new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.fixedDeltaTime * this._speed));
    }
}
