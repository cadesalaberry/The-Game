using System.Collections.Generic;
using UnityEngine;

public class DraggerWithHistory : MonoBehaviour, IDragger
{
    [SerializeField] private float _speedMultiplierOnRelease = 3;
    [SerializeField] private float _rememberXPreviousMovements = 5;
    [SerializeField] private float _followFingerSpeed = 100;
    private Vector3 _zVectorOffset = Vector3.forward * 6;
    private Vector3 _originalDragPosition;
    private Vector3 _spawnPosition;
    private Vector3 _dragOffset;
    private Camera _cam;
    private	Rigidbody _rb;
    private List<Vector3> _previousMovements = new List<Vector3>();

    public Vector3 getSpawnPosition() => _spawnPosition;

    void Start()
    {
        _cam = Camera.main;
		_rb = GetComponent<Rigidbody>();
        _spawnPosition = transform.position;
    }

    void OnMouseDown()
    {
        var movementSpeed = _followFingerSpeed * Time.deltaTime;

        _rb.isKinematic = true;
        _rb.Sleep();

        _originalDragPosition = transform.position;
        _dragOffset = _originalDragPosition - GetMousePos();

        var target = transform.position + _zVectorOffset;

        transform.position = Vector3.MoveTowards(_originalDragPosition, target, movementSpeed);

        _previousMovements.Clear();
    }

    void OnMouseDrag()
    {
        var finalPosition = GetMousePos() + _dragOffset + _zVectorOffset;
        var movementSpeed = _followFingerSpeed * Time.deltaTime;
        var movementVector = finalPosition - transform.position;

        transform.position = Vector3.MoveTowards(transform.position, finalPosition, movementSpeed);
        Debug.DrawLine(finalPosition, transform.position, Color.white, 3);

        _previousMovements.Add(movementVector);
        if (_previousMovements.Count >= _rememberXPreviousMovements)
        {
            _previousMovements.RemoveAt(0);
        }
    }

    void OnMouseUp()
    {
        var movementSpeed = _followFingerSpeed * Time.deltaTime;
        _rb.isKinematic = false;
        var finalPosition = GetMousePos() + _dragOffset + _zVectorOffset;

        transform.position = Vector3.MoveTowards(transform.position, finalPosition, movementSpeed);

        var averageVector = SumVector(_previousMovements);

        var newVelocity = _speedMultiplierOnRelease * averageVector;

        Debug.Log(newVelocity);

        _rb.velocity = newVelocity;

        Debug.DrawLine(finalPosition, transform.position, Color.white, 3);
    }

    public void Reset()
    {
        transform.position = getSpawnPosition();
        _rb.Sleep();
    }

    // Update is called once per frame
    Vector3 GetMousePos()
    {
        var mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    Vector3 SumVector(List<Vector3> vectors)
    {
        var total = vectors.Count;
        var sumVector = new Vector3(0, 0, 0);

        for (int i = 0; i < total; i++)
        {
            sumVector += vectors[i];
        }

        return sumVector;
    }
}
