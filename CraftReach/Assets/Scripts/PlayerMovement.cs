using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : IPlayerMovement
{
    private readonly Transform _playerTransform;
    private readonly CharacterController _controller;
    private readonly float _speed;
    private readonly float _rotationSpeed;
    private readonly float _hoverHeight; //altura constantexd
    private readonly float _heightAdjustSmoothness = 5f;

    public PlayerMovement(Transform playerTransform, CharacterController controller, float speed, float rotationSpeed, float hoverHeight)
    {
        _playerTransform = playerTransform;
        _controller = controller;
        _speed = speed;
        _rotationSpeed = rotationSpeed;
        _hoverHeight = hoverHeight;
    }

    public void Move(Vector2 inputDirection)
    {
        //Convierte el input 2D a direccion en 3D
        Vector3 moveDir = new Vector3(inputDirection.x, 0, inputDirection.y);
        moveDir = _playerTransform.TransformDirection(moveDir); // respeta rotacion

        // Aplica movimiento con velocidad
        Vector3 velocity = moveDir * _speed;

        // Mide el suelo con raycast
        if (Physics.Raycast(_playerTransform.position, Vector3.down, out RaycastHit hit, 10f))
        {
            float targetY = hit.point.y + _hoverHeight;
            float currentY = _playerTransform.position.y;
            float smoothY = Mathf.Lerp(currentY, targetY, Time.deltaTime * _heightAdjustSmoothness);

            // Corrige Y suavemente
            velocity.y = (smoothY - currentY) / Time.deltaTime;
        }

        // Mueve al personaje
        _controller.Move(velocity * Time.deltaTime);
    }

    public void Look(Vector2 lookDirection)
    {
        if (lookDirection.sqrMagnitude < 0.01f) return; // No hace nada si el input es casi cero

        // Obtiene la rotacion de la camera
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Aplana la rotacion al plano horizontal, quita la inclinacion vertical de la camara
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calcula la direccion deseada en relacion a la camara
        Vector3 desiredDirection = (cameraForward * lookDirection.y + cameraRight * lookDirection.x).normalized;

        // Aplica rotacion suave hacia esa direccion
        if (desiredDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
            _playerTransform.rotation = Quaternion.Slerp(
                _playerTransform.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime
            );
        }
    }


}