using System.Collections.Generic;
using UnityEngine;

public class DraggerWithHistory : MonoBehaviour, IDragger
{
    [SerializeField] private float _transferSpeedFactor = 100;
    [SerializeField] private float _recentMovementsImportance = 100;
    [SerializeField] private float _previousMovementsMax = 5;
    [SerializeField] private float _speed = 100;
    private Vector3 _zVectorOffset = Vector3.forward * 6;
    private Vector3 _originalDragPosition;
    private Vector3 _initialPosition;
    private Vector3 _dragOffset;
    private Camera _cam;
    private	Rigidbody _rb;
    private List<Vector3> _previousMovements = new List<Vector3>();



    void Awake()
    {
        _cam = Camera.main;
		    _rb = GetComponent<Rigidbody>();
        _initialPosition = transform.position;
    }

    void OnMouseDown()
    {
        var movementSpeed = _speed * Time.deltaTime;

        _rb.isKinematic = true;
        _rb.Sleep();

        _originalDragPosition = transform.position;
        _dragOffset = _originalDragPosition - GetMousePos();

        var target = transform.position + _zVectorOffset;

        transform.position = Vector3.MoveTowards(_originalDragPosition, target, movementSpeed);

        _previousMovements.Clear();
    }

    void OnMouseUp()
    {
        var movementSpeed = _speed * Time.deltaTime;
        _rb.isKinematic = false;
        var finalPosition = GetMousePos() + _dragOffset + _zVectorOffset;

        transform.position = Vector3.MoveTowards(transform.position, finalPosition, movementSpeed);

        var averageVector = GetWeightedAverageVector(_previousMovements);

        var newVelocity = _transferSpeedFactor * averageVector;

        Debug.Log(newVelocity);

        _rb.velocity = newVelocity;

        Debug.DrawLine(finalPosition, transform.position, Color.white, 3);
    }

    void OnMouseDrag()
    {
        var finalPosition = GetMousePos() + _dragOffset + _zVectorOffset;
        var movementSpeed = _speed * Time.deltaTime;
        var movementVector = finalPosition - transform.position;

        transform.position = Vector3.MoveTowards(transform.position, finalPosition, movementSpeed);
        Debug.DrawLine(finalPosition, transform.position, Color.white, 3);

        _previousMovements.Add(movementVector);
        if (_previousMovements.Count >= _previousMovementsMax)
        {
            _previousMovements.RemoveAt(0);
        }
    }

    public void Reset()
    {
        transform.position = _initialPosition;
        _rb.Sleep();
    }

    // Update is called once per frame
    Vector3 GetMousePos()
    {
        var mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    Vector3 GetWeightedAverageVector(List<Vector3> vectors)
    {
      var total = vectors.Count;
      var sumVector = new Vector3(0, 0, 0);

      for (int i = 0; i < total; i++)
      {
          var priority = total - i;
          var weight = priority * _recentMovementsImportance;

          sumVector += vectors[i] * weight;
      }

      var averageVector = sumVector / total;

      return averageVector;
    }

}
