using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragger : MonoBehaviour
{
    private Vector3 _originalPosition;
    private Vector3 _dragOffset;
    private Camera _cam;
    [SerializeField] private float _speed = 10;

    void Awake()
    {
        _cam = Camera.main;
    }

    void OnMouseDown()
    {
        _originalPosition = transform.position;
        _dragOffset = _originalPosition - GetMousePos();
    }

    void OnMouseUp()
    {
        var movementSpeed = _speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _originalPosition, movementSpeed);
    }

    // Start is called before the first frame update
    void OnMouseDrag()
    {
        
        var finalPosition = GetMousePos() + _dragOffset;
        var movementSpeed = _speed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, finalPosition, movementSpeed);
    }

    // Update is called once per frame
    Vector3 GetMousePos()
    {
        var mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }
}
