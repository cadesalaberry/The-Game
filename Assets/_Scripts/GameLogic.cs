using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    [SerializeField] Transform prefab;
    [SerializeField] int MaxScore = 5;
    public Sprite[] FoodAssets;
    public Text ScoreText;
    public Button RetryButton;
    public List<GameObject> EatenFood;

    // Start is called before the first frame update
    void Start()
    {
        FoodAssets = Resources.LoadAll<Sprite>("PixelFood");
        RetryButton.gameObject.SetActive(false);
        RetryButton.onClick.AddListener(delegate { Retry(); });
    }

    public int setScore(int newScore)
    {
        ScoreText.text = newScore.ToString();

        return newScore;
    }

    public int getScore()
    {
        return int.Parse(ScoreText.text);
    }

    public void EatFood(GameObject gameObject, Vector3 spawnPosition)
    {
        EatenFood.Add(gameObject);
        Destroy(gameObject);
        var newScore = getScore() + 1;

        SpawnRandomFoodAtPosition(spawnPosition);

        if (newScore < MaxScore)
        {
            setScore(newScore);
            return;
        }

        if (RetryButton.gameObject.activeSelf)
        {
            // Do nothing if the retry button is already shown
            return;
        }

        setScore(newScore);
        Debug.Log("Done eating food, showing retry button");
        RetryButton.gameObject.SetActive(true);
    }

    public void Retry()
    {
        EatenFood.Clear();
        ScoreText.text = 0.ToString();
        RetryButton.gameObject.SetActive(false);
    }

    public Transform SpawnRandomFoodAtPosition(Vector3 spawnPosition)
    {
        int randomIndex = Random.Range(0, FoodAssets.Length);
        Sprite randomSprite = FoodAssets[randomIndex];
        var newFood = Instantiate(prefab, spawnPosition, Quaternion.identity);
        var spriteRenderer = newFood.GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = randomSprite;

        return newFood;

    }
}
