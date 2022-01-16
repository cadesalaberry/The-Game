using UnityEngine;

public class FoodFactory : MonoBehaviour
{
    [SerializeField] Transform prefab;
    public Sprite[] FoodAssets;

    // Start is called before the first frame update
    void Start()
    {
        FoodAssets = Resources.LoadAll<Sprite>("PixelFood");
    }

    public Transform SpawnRandomFoodAt(Vector3 spawnPosition)
    {
        int randomIndex = Random.Range(0, FoodAssets.Length);
        Sprite randomSprite = FoodAssets[randomIndex];
        var newFood = Instantiate(prefab, spawnPosition, Quaternion.identity);
        var spriteRenderer = newFood.GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = randomSprite;

        return newFood;
    }
}
