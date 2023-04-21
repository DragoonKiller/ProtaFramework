using UnityEngine;

[ExecuteAlways]
public class BillboardObject : MonoBehaviour
{
    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if(mainCamera == null) mainCamera = Camera.main;
        if(mainCamera == null) return;
        transform.LookAt(
            transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up
        );
    }
}
