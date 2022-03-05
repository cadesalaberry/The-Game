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
    public GameObject completeLevelUI;
    public Text completeLevelTitle;
    public Text completeLevelType;

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
        CompleteLevel();
    }

    private void CompleteLevel()
    {
        Debug.Log("Done eating food, showing complete level screen");
        var totalFood = GetFoodTotal(EatenFood);
        var bristolScore = GetBristolScore(totalFood);
        var title = "FAILURE";
        var type = "Type " + bristolScore;

        if (bristolScore == 5)
        {
           title = "SUCCESS";
        }

        completeLevelTitle.text = title;
        completeLevelType.text = type;
        StartButton.gameObject.SetActive(true);
        completeLevelUI.SetActive(true);

    }

    private string GetScoreAsString(List<FoodSpecs> eatenFood)
    {
        var totalFood = GetFoodTotal(eatenFood);

        var fat = Math.Round(totalFood.fat, 5);
        var fib = Math.Round(totalFood.fiber, 5);
        var wat = Math.Round(totalFood.water, 5);

        return $"fat: {fat}\nfib: {fib}\nwat: {wat}";
    }


    private FoodItemCoumpounds GetFoodTotal(List<FoodSpecs> eatenFood)
    {
        var totalFat = 0.0f;
        var totalFiber = 0.0f;
        var totalWater = 0.0f;

        eatenFood.ForEach((eatenFood) =>
        {
            totalFat += eatenFood.getFoodItem().compounds.fat;
            totalFiber += eatenFood.getFoodItem().compounds.fiber;
            totalWater += eatenFood.getFoodItem().compounds.water;
        });

        return new FoodItemCoumpounds {
            fat = totalFat,
            fiber = totalFiber,
            water = totalWater,
        };
    }

    /**
     * Very simple algorithm.
     * If we have to much water returns type 1.
     * If we have not enough water returns type 7.
     */
    private int GetBristolScore(float fat, float fiber, float water)
    {
        var array = new float[] { fat, fiber, water };

        if (Mathf.Min(array) == water) return 1;

        if (Mathf.Max(array) == water) return 7;

        return 5;
    }
    private int GetBristolScore(FoodItemCoumpounds compounds)
    {
        return GetBristolScore(compounds.fat, compounds.fiber, compounds.water);
    }

    public void Retry()
    {
        EatenFood.Clear();
        ScoreText.text = GetScoreAsString(EatenFood);
        StartButton.gameObject.SetActive(false);
        completeLevelUI.SetActive(false);
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
