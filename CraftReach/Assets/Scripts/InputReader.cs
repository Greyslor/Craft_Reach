using UnityEngine;
using UnityEngine.InputSystem;
using GameInput;
using System; //Esta usando el script de playerControls, referencia al mapa de controles generado automaticamente por unity

public class InputReader
{
    private readonly PlayerControls _controls;
    //public event Action PauseEvent;

    // vectores almacenan el movimiento y la mirada que el jugador hace
    public Vector2 MovementInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public event Action FireEvent;

    public InputReader()
    {
        _controls = new PlayerControls(); // Carga el mapa de controles del unity

        //Cuando el jugador se mueva con teclado o joystick izquierdo, se guarda el valor en MoveInput
        _controls.Player.Move.performed += ctx => MovementInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += ctx => MovementInput = Vector2.zero;

        //_controls.Player.Pause.performed += ctx => PauseEvent?.Invoke();

        //Cuando el jugador mire con mouse o joystick derecho, se guarda el valor en vector2 LookInput 
        _controls.Player.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        _controls.Player.Look.canceled += ctx => LookInput = Vector2.zero;

        _controls.Player.Fire.performed += ctx => FireEvent?.Invoke();

        _controls.Enable();
    }
}
