using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsControllerInput : MonoBehaviour
{
    public Vector2 GetMoveAxis()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
    }

    public Vector2 GetMouseAxis()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    public bool GetSprint()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }

    public bool Jump()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
}
