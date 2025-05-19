using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMove : NetworkBehaviour
{
    public float f = 5f;
    public Rigidbody rb;
    public Vector3 move;
   
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    public override void OnStartClient()
    {
        name = $"Player[{netId}|{(isLocalPlayer ? "local" : "remote")}]";
        Debug.Log("OnStartServer: " + name);
    }

    public override void OnStartServer()
    {
        name = $"Player[{netId}|server]";
        Debug.Log("OnStartServer: " + name);
    }
    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        move = new Vector3(moveX, 0, moveZ).normalized;
        rb.MovePosition(rb.position + (move * f * Time.deltaTime));
    }

   /* private void FixedUpdate()
    {
        rb.MovePosition(rb.position + move * f * Time.deltaTime);
    }*/
}
