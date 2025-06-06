using Mirror.Examples.Benchmark;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Header("Configuracion de movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float hoverHeight = 1.5f;

    private IPlayerMovement _movement;
    private InputReader _inputReader;
    private CharacterController _controller;

    [Header("Camara")]
    [SerializeField] private GameObject virtualCamera; // Cinemachine

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _inputReader = new InputReader();
        _movement = new PlayerMovement(transform, _controller, moveSpeed, rotateSpeed, hoverHeight);
    }

    private void Start()
    {
        // Solo activa camara si es el jugador local
        if (isLocalPlayer)
        {
            virtualCamera.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            virtualCamera.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        _movement.Move(_inputReader.MovementInput);
        _movement.Look(_inputReader.LookInput);
    }
}