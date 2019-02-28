using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public GameObject inputManager;
    public Vector3 CameraMinDimensions;
    public Vector3 CameraMaxDimensions;

    public Vector2 Resolution;

    public FoodSpawner FoodManager;
    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            instance = this;
        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        Init();
    }

    // Start is called before the first frame update
    void Init()
    {
        UpdateScreenSize();
        Instantiate(inputManager);
    }

    private void Update()
    {
        if (Resolution.x != Screen.width || Resolution.y != Screen.height)
            UpdateScreenSize();
    }

    public void UpdateScreenSize()
    {
        CameraMinDimensions = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.rect.xMin, 0, Camera.main.transform.position.z));
        CameraMaxDimensions = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.rect.xMax, 0, Camera.main.transform.position.z));
        Resolution.x = Screen.width;
        Resolution.y = Screen.height;
    }
}
