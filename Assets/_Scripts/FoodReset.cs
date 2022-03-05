using UnityEngine;

public class FoodReset : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        var gameObject = collision.gameObject;
        var dragger = gameObject.GetComponent<IDragger>();

        if (dragger == null) return;


        var spawnPosition = dragger.getSpawnPosition();
        var foodFactoryGO = GameObject.FindWithTag("FoodFactory");

        if (foodFactoryGO == null) return;

        var gameLogic = foodFactoryGO.GetComponent<GameLogic>();

        gameLogic.SpawnRandomFoodAtPosition(spawnPosition);
    }
}
