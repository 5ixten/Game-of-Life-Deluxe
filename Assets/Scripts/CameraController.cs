using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Camera camera;
    public float dragSpeed = 5f;

    bool isDragging = false;
    Vector3 lastMousePos;

    float scrollSpeed = 15f;
    float minScroll = 2f;
    float maxScroll = 100f;

    void Start() {
        camera = GetComponent<Camera>();
    }

    void Update() {
        MoveCamera();
        ScrollCamera();
    }

    void MoveCamera() {
        if (Input.GetMouseButtonDown(0)) {
            isDragging = true;
        }
        if (Input.GetMouseButtonUp(0)) {
            isDragging = false;
        }

        if (!isDragging) {
            lastMousePos = Input.mousePosition;
            return; 
        }

        // Movement on screen
        Vector3 screenDelta = Input.mousePosition - lastMousePos;
        // Movement in world
        Vector3 worldDelta = camera.ScreenToWorldPoint(new Vector3(screenDelta.x, screenDelta.y, -camera.transform.position.z)) -
                             camera.ScreenToWorldPoint(new Vector3(0, 0, -camera.transform.position.z));

        transform.position -= worldDelta;
        lastMousePos = Input.mousePosition;
    }

    void ScrollCamera() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float scrollValue = scroll * scrollSpeed;

        float zPos = Mathf.Clamp(-(transform.position.z + scrollValue), minScroll, maxScroll);
   
        transform.position = new Vector3(transform.position.x, transform.position.y, -zPos);
        //camera.orthographicSize = Mathf.Clamp(camera.orthographicSize + scrollValue, minScroll, maxScroll);
    }
}
