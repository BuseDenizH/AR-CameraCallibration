using UnityEngine;

public class ProjectionRenderer : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject teapot;

    void Update()
    {
        if (teapot != null)
        {
            Vector3 teapotScreenPosition = mainCamera.WorldToScreenPoint(teapot.transform.position);
            Debug.Log($"Teapot projected to screen at: {teapotScreenPosition}");
        }
    }
}
