using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public static class PlayerInput
{
    private static float bufferDuration = 0.2f;
    private static float attackTimeStamp = -1.0f;
    private static float dashTimeStamp = -1.0f;
    private static float shieldTimeStamp = -1.0f;
    private static float fireballTimeStamp = -1.0f;

    public static bool GetAttackInput(bool useBuffer = true)
    {
        if (useBuffer && Time.time <= attackTimeStamp)
        {
            attackTimeStamp = -1.0f;
            return true;
        }
        if (Gamepad.current != null)
            return Gamepad.current.xButton.wasPressedThisFrame;
        else
            return Mouse.current.leftButton.wasPressedThisFrame;
    }

    public static bool GetDashInput(bool useBuffer = true)
    {
        if (useBuffer && Time.time <= dashTimeStamp)
        {
            dashTimeStamp = -1.0f;
            return true;
        }
        if (Gamepad.current != null)
            return Gamepad.current.bButton.wasPressedThisFrame;
        else
            return Keyboard.current.spaceKey.wasPressedThisFrame;
    }

    public static bool GetShieldInput(bool useBuffer = true)
    {
        if (useBuffer && Time.time <= shieldTimeStamp)
        {
            shieldTimeStamp = -1.0f;
            return true;
        }
        if (Gamepad.current != null)
            return Gamepad.current.yButton.wasPressedThisFrame;
        else
            return Mouse.current.rightButton.wasPressedThisFrame;
    }

    public static bool GetRemoveShieldInput()
    {
        if (Gamepad.current != null)
            return Gamepad.current.yButton.isPressed;
        else
            return Mouse.current.rightButton.isPressed;
    }

    public static bool GetFireballInput(bool useBuffer = true)
    {
        if (useBuffer && Time.time <= fireballTimeStamp)
        {
            fireballTimeStamp = -1.0f;
            return true;
        }
        if (Gamepad.current != null)
            return Gamepad.current.rightTrigger.wasPressedThisFrame;
        else
            return Keyboard.current.fKey.wasPressedThisFrame;
    }
    public static bool GetReleaseFireballInput()
    {
        if (Gamepad.current != null)
            return Gamepad.current.rightTrigger.wasReleasedThisFrame;
        else
            return Keyboard.current.fKey.wasReleasedThisFrame;
    }

    public static bool GetInteractInput()
    {
        if (Gamepad.current != null)
            return Gamepad.current.aButton.wasPressedThisFrame;
        else
            return Keyboard.current.eKey.wasPressedThisFrame;
    }

    public static bool GetLockInput()
    {
        if (Gamepad.current != null)
            return Gamepad.current.leftTrigger.isPressed;
        else
            return Keyboard.current.aKey.wasPressedThisFrame;
    }

    public static void UpdateInputBuffer()
    {
        if (GetAttackInput(false))
            attackTimeStamp = Time.time + bufferDuration;

        if (GetDashInput(false))
            dashTimeStamp = Time.time + bufferDuration;

        if (GetShieldInput(false))
            shieldTimeStamp = Time.time + bufferDuration;

        if (GetFireballInput(false))
            fireballTimeStamp = Time.time + bufferDuration;
    }

    public static Vector2 ComputeMoveDirection()
    {
        if (Gamepad.current != null)
        {
            Vector2 joystickInput = Gamepad.current.leftStick.value;

            if (joystickInput.magnitude < 0.1f)
                joystickInput = Vector2.zero;

            return joystickInput;
        }

        Vector2 inputDirection = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            inputDirection.y = 1.0f;

        if (Keyboard.current.sKey.isPressed)
            inputDirection.y = -1.0f;

        if (Keyboard.current.dKey.isPressed)
            inputDirection.x = 1.0f;

        if (Keyboard.current.aKey.isPressed)
            inputDirection.x = -1.0f;

        return inputDirection.normalized;
    }

    public static Vector2 ComputeLookDirection()
    {
        if (Gamepad.current != null)
        {
            Vector2 joystickInput = Gamepad.current.rightStick.value;

            //joystick deadzone
            if (joystickInput.magnitude < 0.1f)
                joystickInput = Vector2.zero;

            return joystickInput;
        }

        return Vector2.zero;
    }
}
