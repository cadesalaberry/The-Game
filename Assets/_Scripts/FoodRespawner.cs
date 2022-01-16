using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodRespawner : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        var gameObject = collision.gameObject;
        var dragger = gameObject.GetComponent<IDragger>();

        if (dragger == null) return;


        var spawnPosition = dragger.getSpawnPosition();
        var foodFactoryGO = GameObject.FindWithTag("FoodFactory");

        if (foodFactoryGO == null) return;

        var foodFactory = foodFactoryGO.GetComponent<FoodFactory>();

        Destroy(gameObject);
        foodFactory.SpawnRandomFoodAt(spawnPosition);
    }
}
