using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputInitializer : MonoBehaviour
{
    public static InputInitializer Instance { get; private set; }

    private Controls _controls;

    public event Action<Vector2> ClickInput;
    public event Action<Vector2> HoldStartedInput;
    public event Action<Vector2> HoldCancelledInput;

    public bool IsHolding { get; private set; }

    private void OnEnable()
    {
        CheckInstance();
        _controls = new Controls();
        _controls.Player.Enable();
        _controls.Player.Click.performed += OnClick;
        _controls.Player.Hold.started += OnHoldStarted;
        _controls.Player.Hold.canceled += OnHoldCancelled;
    }

    private void OnDisable()
    {
        _controls.Player.Click.performed -= OnClick;
        _controls.Player.Hold.started -= OnHoldStarted;
        _controls.Player.Hold.canceled -= OnHoldCancelled;
        _controls.Player.Disable();
    }

    private void CheckInstance()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one " + this + " detected!");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnClick(InputAction.CallbackContext obj) => ClickInput?.Invoke(MousePosition());

    public Vector2 MousePosition() => _controls.Player.MousePosition.ReadValue<Vector2>();

    private void OnHoldCancelled(InputAction.CallbackContext obj) 
    {
        HoldCancelledInput?.Invoke(MousePosition());
        IsHolding = false; 
    }
    private void OnHoldStarted(InputAction.CallbackContext obj)
    {
        HoldStartedInput?.Invoke(MousePosition());
        IsHolding = true;
    }
}
