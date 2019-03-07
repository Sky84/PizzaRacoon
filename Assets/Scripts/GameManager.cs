using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public GameObject inputManager;
    public Vector3 CameraMinDimensions;
    public Vector3 CameraMaxDimensions;

    public Vector2 Resolution;

    public PlayerController Player;
    public BackgroundManager BackgroundManager;
    public FoodSpawner FoodManager;
    public GameObject Roulette;
    public CanvaManager CanvaManager;
    public VideoPlayer VideoPlayer;
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
        VideoPlayer.gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Init()
    {
        UpdateScreenSize();
        Instantiate(inputManager);

        VideoPlayer.loopPointReached += VideoPlayer_loopPointReached;
    }

    private void VideoPlayer_loopPointReached(VideoPlayer source)
    {
        source.Stop();
        VideoPlayer.gameObject.SetActive(false);
    }

    private void OnLevelLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Time.timeScale = 1;
        InputManager.instance.BlockInput = false;
        BackgroundManager = GameObject.FindObjectOfType<BackgroundManager>();
        FoodManager = GameObject.FindObjectOfType<FoodSpawner>();
        Roulette = GameObject.FindGameObjectWithTag("Roulette");
        Player = GameObject.FindObjectOfType<PlayerController>();
        CanvaManager = GameObject.FindObjectOfType<CanvaManager>();
        if(GameObject.FindObjectOfType<AudioSource>() != null && arg0.name.ToLower().Contains("level"))
        {
            GameObject.FindObjectOfType<AudioSource>().Stop();
            GameObject.FindObjectOfType<AudioSource>().Play();
        }
        else
        {
            GameObject.FindObjectOfType<AudioSource>().Stop();
        }
    }

    private void Update()
    {
        if (Resolution.x != Screen.width || Resolution.y != Screen.height)
            UpdateScreenSize();

        if(Player != null)
        {
            if (Player.CurrentWeight > Player.OverfatGap && !Roulette.activeSelf)
            {
                Roulette.SetActive(true);
            }
            else if (Player.CurrentWeight < Player.OverfatGap && Roulette.activeSelf)
            {
                Roulette.SetActive(false);
            }
        } 
    }

    public void Pause()
    {
        Time.timeScale = 0;
        InputManager.instance.BlockInput = true;
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void ReturnTo(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void Loose()
    {
        Time.timeScale = 0;
        InputManager.instance.BlockInput = true;
        CanvaManager.ShowLooseScreen();
    }

    public void UpdateScreenSize()
    {
        CameraMinDimensions = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.rect.xMin, 0, Camera.main.transform.position.z));
        CameraMaxDimensions = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.rect.xMax, 0, Camera.main.transform.position.z));
        Resolution.x = Screen.width;
        Resolution.y = Screen.height;
    }
}
