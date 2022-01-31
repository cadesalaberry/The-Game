using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpecs : MonoBehaviour
{
    [SerializeField]
    private FoodItem _specs;

    public FoodItem getFoodItem()
    {
        return _specs;
    }

    public void setFoodItem(FoodItem foodItem)
    {
        _specs = foodItem;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
