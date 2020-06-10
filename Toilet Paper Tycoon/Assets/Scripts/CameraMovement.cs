using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    float speed = 3.35f; //feels most normal.

    float minSize = 3f;
    float maxSize = 10f;
    float zoomSensitivity = 150f;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update()
    {
        float xAxis = Input.GetAxisRaw("Horizontal") * (Time.deltaTime) * speed;
        float yAxis = Input.GetAxisRaw("Vertical") * (Time.deltaTime) * speed;

        Camera.main.transform.Translate(xAxis, 0, 0);
        Camera.main.transform.Translate(0, yAxis, 0);

        float fov = Camera.main.orthographicSize;
        fov -= Input.GetAxis("Mouse ScrollWheel") * (Time.deltaTime) * zoomSensitivity;
        fov = Mathf.Clamp(fov, minSize, maxSize);

        Camera.main.orthographicSize = fov;
    }
}
