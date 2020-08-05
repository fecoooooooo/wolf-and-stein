using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviourSingleton<Character>
{
    new Rigidbody rigidbody;
    float TurnSpeed = 3f;
    const float MoveSpeed = 5f;
    float moveVector = 0;
    float turnVector = 0;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        moveVector = Input.GetAxisRaw("Vertical");
        turnVector = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = transform.forward * moveVector * MoveSpeed;
        rigidbody.angularVelocity = transform.up * turnVector * TurnSpeed;
    }
}
