using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerMovement
{
    void Move(Vector2 direction); // el metodo se mueve segun el input desde unity
    void Look(Vector2 lookDirection);
}
