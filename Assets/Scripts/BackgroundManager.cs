using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject[] Frontgrounds;
    public GameObject[] Backgrounds2;
    public GameObject[] Backgrounds3;
    public GameObject[] Backgrounds4;
    public GameObject[] Backgrounds5;

    public float HorizontalSpeedBackground;

    private float MaxRightX;
    private float camLeftBorder;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.UpdateScreenSize();
        MaxRightX = GameManager.instance.CameraMaxDimensions.x;
        camLeftBorder = GameManager.instance.CameraMinDimensions.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveBackgrounds(Frontgrounds, HorizontalSpeedBackground * 0.5f);
        MoveBackgrounds(Backgrounds2, HorizontalSpeedBackground * 0.4f);
        MoveBackgrounds(Backgrounds3, HorizontalSpeedBackground * 0.3f);
        MoveBackgrounds(Backgrounds4, HorizontalSpeedBackground * 0.2f);
        MoveBackgrounds(Backgrounds5, HorizontalSpeedBackground * 0.1f);
    }

    private void MoveBackgrounds(GameObject[] Backgrounds, float speed)
    {
        MaxRightX = GetMaxX(Backgrounds);
        for (var i = 0; i < Backgrounds.Length; i++)
        {
            var currentGround = Backgrounds[i];
            var currentSprite = Backgrounds[i].GetComponent<SpriteRenderer>();

            if (currentSprite.bounds.max.x < camLeftBorder)
            {
                var pos = new Vector3(MaxRightX, 0);
                pos.y = 0;
                currentGround.transform.position = pos;
            }
            currentGround.transform.position += new Vector3(speed, 0) * Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.left * camLeftBorder, 0.2f);
    }

    private float GetMaxX(GameObject[] gameObjects)
    {
        float max = 0;
        foreach (GameObject gm in gameObjects)
        {
            var spriteBounds = gm.GetComponent<SpriteRenderer>().bounds;
            if (spriteBounds.max.x + spriteBounds.extents.x > max)
                max = spriteBounds.max.x + spriteBounds.extents.x;
        }
        return max;
    }
}
