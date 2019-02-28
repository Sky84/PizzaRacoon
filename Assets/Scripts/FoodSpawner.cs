using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum FoodType
{
    GoodFood,
    BadFood,
    ToxicFood
}

public class FoodSpawner : MonoBehaviour
{
    public int TimeStartWait;
    public int SpeedFood;

    [Header("Spawn Settings")]
    public bool SpawnFood;
    public int TimeGapSpawner;

    public GameObject[] GoodFoods;
    public GameObject[] BadFoods;
    public GameObject[] ToxicFoods;
    [HideInInspector]
    public List<GameObject> FoodsSpawned;

    private List<GameObject> SpawnPoints;


    private Dictionary<FoodType, GameObject[]> FoodTypeDictionnary;

    private void Awake()
    {
        FoodsSpawned = new List<GameObject>();

        var sps = transform.Find("SpawnPoints");
        SpawnPoints = new List<GameObject>();
        foreach (Transform sp in sps)
        {
            SpawnPoints.Add(sp.gameObject);
        }

        FoodTypeDictionnary = new Dictionary<FoodType, GameObject[]>();
        FoodTypeDictionnary.Add(FoodType.GoodFood, GoodFoods);
        FoodTypeDictionnary.Add(FoodType.BadFood, BadFoods);
        FoodTypeDictionnary.Add(FoodType.ToxicFood, ToxicFoods);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.FoodManager = this;
        if (SpawnFood)
            StartCoroutine("SpawnFood");
        else
        {
            foreach (Transform food in transform)
            {
                if (food.gameObject.layer == LayerMask.NameToLayer("Food"))
                    FoodsSpawned.Add(food.gameObject);
            }
        }
    }

    private void Update()
    {
        foreach (GameObject food in FoodsSpawned.ToList())
        {
            food.transform.localPosition += Vector3.left * SpeedFood * Time.deltaTime;
            var fRder = food.GetComponent<SpriteRenderer>();
            if (fRder.bounds.max.x + fRder.bounds.extents.x < GameManager.instance.CameraMinDimensions.x)
            {
                Destroy(food);
                FoodsSpawned.Remove(food);
            }
        }
    }

    private IEnumerator SpawnGameobjectFood()
    {
        yield return new WaitForSeconds(TimeStartWait);
        while (true)
        {
            FoodType foodType = (FoodType)UnityEngine.Random.Range(0, 3);
            var foods = FoodTypeDictionnary[foodType];
            var randomNbr = UnityEngine.Random.Range(0, foods.Length);
            var foodGm = Instantiate(foods[randomNbr], transform);
            var pos = SpawnPoints[UnityEngine.Random.Range(0, SpawnPoints.Count)].transform.position;
            foodGm.transform.position = pos;
            FoodsSpawned.Add(foodGm);
            yield return new WaitForSeconds(TimeGapSpawner);
        }
    }
}
