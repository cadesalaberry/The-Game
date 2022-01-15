using UnityEngine;

public class FoodReset : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        var dragger = collision.gameObject.GetComponent<IDragger>();

        dragger?.Reset();
    }
}
