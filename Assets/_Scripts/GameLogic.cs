using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    [SerializeField] Transform prefab;
    [SerializeField] int MaxScore = 5;
    public Dictionary<string, Sprite> FoodAssets = new Dictionary<string, Sprite>();
    public FoodCollection FoodDefinitions;
    public Text ScoreText;
    public Button StartButton;
    public List<FoodSpecs> EatenFood;

    // Start is called before the first frame update
    void Start()
    {
        var jsonDefinitions = Resources.Load<TextAsset>("FoodDefinitions");

        FoodDefinitions = JsonUtility.FromJson<FoodCollection>(jsonDefinitions.text);

        foreach (var food in FoodDefinitions.collection)
        {
            var foodSprite = Resources.Load<Sprite>("PixelFood/" + food.sprite_name);
            FoodAssets.Add(food.id, foodSprite);
        }

        // StartButton.gameObject.SetActive(false);

        StartButton.onClick.AddListener(delegate { Retry(); });
        Retry();
    }

    public string setScore(string newScore)
    {
        ScoreText.text = newScore;

        return newScore;
    }

    public void EatFood(GameObject gameObject, Vector3 spawnPosition)
    {
        var foodSpecs = gameObject.GetComponent<FoodSpecs>();

        if (foodSpecs == null)
        {
            throw new Exception("This cannot be eaten!");
        }

        EatenFood.Add(foodSpecs);
        Destroy(gameObject);
        var totalFoodEaten = EatenFood.Count;
        var scoreAsString = GetScoreAsString(EatenFood);

        SpawnRandomFoodAtPosition(spawnPosition);

        if (totalFoodEaten < MaxScore)
        {
            setScore(scoreAsString);
            return;
        }

        if (StartButton.gameObject.activeSelf)
        {
            // Do nothing if the retry button is already shown
            return;
        }

        setScore(scoreAsString);
        Debug.Log("Done eating food, showing retry button");
        StartButton.gameObject.SetActive(true);
    }

    private string GetScoreAsString(List<FoodSpecs> eatenFood)
    {
        var totalFat = 0.0;
        var totalWater = 0.0;
        var totalFiber = 0.0;

        eatenFood.ForEach((eatenFood) =>
        {
            totalFat += eatenFood.getFoodItem().compounds.fat;
            totalWater += eatenFood.getFoodItem().compounds.water;
            totalFiber += eatenFood.getFoodItem().compounds.fiber;
        });

        var fat = Math.Round(totalFat, 5);
        var fib = Math.Round(totalFiber, 5);
        var wat = Math.Round(totalWater, 5);

        return $"fat: {fat}\nfib: {fib}\nwat: {wat}";
    }

    public void Retry()
    {
        EatenFood.Clear();
        ScoreText.text = GetScoreAsString(EatenFood);
        StartButton.gameObject.SetActive(false);
        StartButton.GetComponentInChildren<Text>().text = "Retry";
        CleanAllRemainingFood();
        SpawnInitialFood();
    }

    public void CleanAllRemainingFood()
    {
        var foodGO = GameObject.FindGameObjectsWithTag("Food");

        foreach (var food in foodGO)
        {
            Destroy(food);
        }
    }

    public void SpawnInitialFood()
    {
        SpawnFoodFromIdAtPosition("asparagus", new Vector3(1.5f, -4, 6));
        SpawnFoodFromIdAtPosition("apple", new Vector3(0, -4, 6));
        SpawnFoodFromIdAtPosition("banana", new Vector3(-1.5f, -4, 6));
    }

    public Transform SpawnRandomFoodAtPosition(Vector3 spawnPosition)
    {
        var randomIndex = UnityEngine.Random.Range(0, FoodDefinitions.collection.Count);
        var randomFood = FoodDefinitions.collection[randomIndex];

        return SpawnFoodAtPosition(randomFood, spawnPosition);
    }

    public Transform SpawnFoodFromIdAtPosition(string foodId, Vector3 spawnPosition)
    {
        var foodItem = FoodDefinitions.collection.Find((food) => food.id == foodId);

        if (foodItem == null) { throw new Exception("No food by the name: " + foodId); }

        return SpawnFoodAtPosition(foodItem, spawnPosition);
    }

    public Transform SpawnFoodAtPosition(FoodItem foodItem, Vector3 spawnPosition)
    {
        Sprite foodSprite = FoodAssets[foodItem.id];
        var newFood = Instantiate(prefab, spawnPosition, Quaternion.identity);
        var spriteRenderer = newFood.GetComponent<SpriteRenderer>();
        var foodSpecs = newFood.GetComponent<FoodSpecs>();

        spriteRenderer.sprite = foodSprite;
        foodSpecs.setFoodItem(foodItem);
        newFood.tag = "Food";
        newFood.name = "food-" + foodItem.id;

        return newFood;
    }
}
