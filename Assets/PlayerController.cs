using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float _speed = 10f;
    Rigidbody _rb;
    Camera _playerCam;

    private void Awake() {
        this._rb = GetComponent<Rigidbody>();
        
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // this._rb.MovePosition(this._rb.position + (new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * Time.fixedDeltaTime * this._speed));
        // this._rb.velocity = (new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * Time.fixedDeltaTime * this._speed);
        this._rb.velocity = (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")).normalized * Time.fixedDeltaTime * this._speed;
        this.transform.Rotate(0, Input.GetAxis("Mouse X"), 0);
        
    }
}
