using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public float playerSpeed = 5;
    public Camera playerCamera;
    public float JumpForce = 10;
    private CharacterController _player;

    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
    }
    
    void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);
        Vector3 velocity = direction * playerSpeed;
        _player.Move(velocity * Time.deltaTime);
    }
    
    // method to jump
    void Jump()
    {
        Console.WriteLine("Jump method called");
        // jump if player is on the ground
        if (_player.isGrounded || !_player.detectCollisions)
        {
            Console.WriteLine("Player is on the ground");
            if (!Input.GetKeyDown(KeyCode.Space)) return;
            Console.WriteLine("Player is jumping");
            Vector3 jumpVector = new Vector3(0, JumpForce, 0);
            _player.Move(jumpVector);
        }
        else
        {
            Console.WriteLine("Player is not on the ground");
            _player.Move(Physics.gravity * Time.deltaTime);
        }
    }
}
