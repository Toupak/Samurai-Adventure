using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactable : Trigger
{
    protected virtual void Update()
    {
        if (isWithinRange == true && PlayerInput.GetInteractInput())
        {
            OnTrigger.Invoke();
        }
    }
}
