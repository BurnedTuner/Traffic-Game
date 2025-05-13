using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputInitializer : MonoBehaviour
{
    public static InputInitializer Instance { get; private set; }

    private Controls _controls;

    public event Action<Vector2> PrimaryClickInput;
    public event Action<Vector2> AltClickInput;
    public event Action<Vector2> OnRemoveInput;
    public event Action<Vector2> PrimaryHoldStartedInput;
    public event Action<Vector2> AltHoldStartedInput;
    public event Action<Vector2> PrimaryHoldCancelledInput;
    public event Action<Vector2> AltHoldCancelledInput;

    public bool IsHoldingPrimary { get; private set; }
    public bool IsHoldingAlt { get; private set; }

    private void OnEnable()
    {
        CheckInstance();
        _controls = new Controls();
        _controls.Player.Enable();
        _controls.Player.PrimaryClick.performed += OnPrimaryClick;
        _controls.Player.PrimaryHold.started += OnPrimaryHoldStarted;
        _controls.Player.PrimaryHold.canceled += OnPrimaryHoldCancelled;
        _controls.Player.AltClick.performed += OnAltClick;
        _controls.Player.Remove.performed += OnRemove;
        _controls.Player.AltHold.started += OnAltHoldStarted;
        _controls.Player.AltHold.canceled += OnAltHoldCancelled;
    }

    private void OnDisable()
    {
        _controls.Player.PrimaryClick.performed -= OnPrimaryClick;
        _controls.Player.PrimaryHold.started -= OnPrimaryHoldStarted;
        _controls.Player.PrimaryHold.canceled -= OnPrimaryHoldCancelled;
        _controls.Player.AltClick.performed -= OnAltClick;
        _controls.Player.Remove.performed -= OnRemove;
        _controls.Player.AltHold.started -= OnAltHoldStarted;
        _controls.Player.AltHold.canceled -= OnAltHoldCancelled;
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

    public Vector2 MousePosition() => _controls.Player.MousePosition.ReadValue<Vector2>();

    private void OnPrimaryClick(InputAction.CallbackContext obj) => PrimaryClickInput?.Invoke(MousePosition());
    private void OnAltClick(InputAction.CallbackContext obj) => AltClickInput?.Invoke(MousePosition());
    private void OnRemove(InputAction.CallbackContext obj) => OnRemoveInput?.Invoke(MousePosition());

    private void OnPrimaryHoldStarted(InputAction.CallbackContext obj)
    {
        PrimaryHoldStartedInput?.Invoke(MousePosition());
        IsHoldingPrimary = true;
    }

    private void OnPrimaryHoldCancelled(InputAction.CallbackContext obj) 
    {
        PrimaryHoldCancelledInput?.Invoke(MousePosition());
        IsHoldingPrimary = false; 
    }

    private void OnAltHoldStarted(InputAction.CallbackContext obj)
    {
        AltHoldStartedInput?.Invoke(MousePosition());
        IsHoldingAlt = true;
    }

    private void OnAltHoldCancelled(InputAction.CallbackContext obj) 
    {
        AltHoldCancelledInput?.Invoke(MousePosition());
        IsHoldingAlt = false; 
    }
}
