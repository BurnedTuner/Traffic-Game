using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Camera gameCamera;
    public float cameraMovementSpeed = 5;

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        InputInitializer.Instance.MovementInput -= MoveCamera;
    }

    private void Start()
    {
        InputInitializer.Instance.MovementInput += MoveCamera;
        gameCamera = GetComponent<Camera>();
    }

    public void MoveCamera(Vector2 inputVector)
    {
        Vector3 movementVector = new Vector3(inputVector.x, 0, inputVector.y);
        gameCamera.transform.position += movementVector * Time.deltaTime * cameraMovementSpeed;
    }
}
