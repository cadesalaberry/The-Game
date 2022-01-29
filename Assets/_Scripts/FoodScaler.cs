using UnityEngine;

public class FoodScaler : MonoBehaviour
{
    [SerializeField] private float _maxScaleValue = 1.4f;
    [SerializeField] private float _minScaleValue = 0.6f;
    private Vector3 _spawnScale;

    // Start is called before the first frame update
    void Start()
    {
        _spawnScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        var zValue = transform.position.z;
        // Food evolves between 0 and 12
        // 0 should be 0.6
        // 6 should be 1
        // 12 should be max value (1.4?)
        // => (z / 12) * 0.8 + 0.6
        var span = _maxScaleValue - _minScaleValue;
        var zScaler = (zValue / 12) * span + _minScaleValue;
        var scaleValue = Mathf.Clamp(zScaler, _minScaleValue, _maxScaleValue);

        transform.localScale = scaleValue * _spawnScale;
    }
}
