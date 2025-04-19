using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GroundPointer : MonoBehaviour
{
    public event Action<Vector3> GroundClicked;

    private void Update()
    {
        if (InputInitializer.Instance.IsHoldingPrimary)
            OnClickInput(InputInitializer.Instance.MousePosition());
    }

    private void OnClickInput(Vector2 screenPos)
    {

        if (RaycastGround(screenPos) != null)
        {
            Vector3 position = (Vector3)RaycastGround(screenPos);
            //Debug.Log(position);
            GroundClicked?.Invoke(position);
        }
    }

    private Vector3? RaycastGround(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        foreach (RaycastHit hit in Physics.RaycastAll(ray, Mathf.Infinity))
            if (!EventSystem.current.IsPointerOverGameObject())
                if (hit.collider.TryGetComponent(out Ground ground))
                {
                    Vector3 position = hit.point;
                    return position;
                }

        return null;
    }
}
