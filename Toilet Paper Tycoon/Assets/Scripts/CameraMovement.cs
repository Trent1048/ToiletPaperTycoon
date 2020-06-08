using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    float speed = 0.12f; //feels most normal.

    float minSize = 3f;
    float maxSize = 10f;
    float zoomSensitivity = 5.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float xAxis = Input.GetAxisRaw("Horizontal") * speed;
        float yAxis = Input.GetAxisRaw("Vertical") * speed;
        
        Camera.main.transform.Translate(xAxis, 0, 0);
        Camera.main.transform.Translate(0, yAxis, 0);


        float fov = Camera.main.orthographicSize;
        fov += Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        fov = Mathf.Clamp(fov, minSize, maxSize);

        Camera.main.orthographicSize = fov;

    }
}
