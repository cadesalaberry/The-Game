using System.Collections.Generic;

[System.Serializable]
public class FoodItemCoumpounds
{
    public float water;
    public float fiber;
    public float fat;
}

[System.Serializable]
public class FoodItem
{
    public string id;
    public string name;
    public string sprite_name;
    public string scientific_name;
    public string description;
    public FoodItemCoumpounds compounds;
}

[System.Serializable]
public class FoodCollection
{
    public List<FoodItem> collection;
}