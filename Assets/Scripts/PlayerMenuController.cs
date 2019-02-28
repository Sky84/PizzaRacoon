using DragonBones;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenuController : MonoBehaviour
{
    public int Speed;
    public LevelSelectionManager levelManager;

    private Vector2 PosistionToGo;
    private UnityArmatureComponent armature;

    private void Start()
    {
        armature = GetComponentInChildren<UnityArmatureComponent>();
        GoToLevelButton(1);
    }

    private void GoToLevelButton(int index)
    {
        PosistionToGo = levelManager.KeyPositions[index];
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.instance.NbrTouches > 0 && InputManager.instance.Touch.phase == TouchPhase.Began)
        {
            Vector3 raycast = Camera.main.ScreenToWorldPoint(InputManager.instance.Touch.position);
            RaycastHit2D hit = Physics2D.Raycast(raycast, Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("LevelButton"))
            {
                PosistionToGo = hit.collider.transform.position;
                if (transform.position.Equals(PosistionToGo))
                {
                    levelManager.GoToSuggestedLevel();
                }
                SetAnimation("Run1");
                var rotationY = (transform.position.x > PosistionToGo.x) ? 180 : 0;
                transform.rotation = new Quaternion(transform.rotation.x, rotationY, transform.rotation.z, transform.rotation.w);
            }
        }else if(transform.position.Equals(PosistionToGo))
        {
            SetAnimation("Idle");
            levelManager.SuggestLevelByPosition(PosistionToGo);
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, PosistionToGo, Speed * Time.deltaTime);
    }

    private void SetAnimation(string animName)
    {
        if(armature.animation.lastAnimationName != animName)
        {
            armature.animation.FadeIn(animName, 0.25f, 0);
        }
    }
}
