using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance = null;

    public Touch Touch;
    public int NbrTouches;
    public bool Jump;
    public bool BlockInput;


    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            instance = this;
        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a InputManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (BlockInput)
            return;
        Jump = (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButton(0);
        NbrTouches = Input.touchCount;
        if(NbrTouches > 0)
        {
            Touch = Input.GetTouch(0);
        }
    }
}
