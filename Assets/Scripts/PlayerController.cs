using DragonBones;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArmatureFactoryComponent
{
    public DragonBonesData Data_ske { get; set; }
    public UnityTextureAtlasData Data_tex { get; set; }
    public string ArmatureName { get; set; }
    public string AnimationNameDefault { get; set; }
}

public class PlayerController : MonoBehaviour
{
    public float JumpForce;
    public int CurrentWeight;

    [Header("Score Params")]
    public int GoodFoodScore;
    public int BadFoodScore;
    public int ToxicFoodScore;

    [Header("Food Weight To Add Params")]
    public int GoodFoodWeight;
    public int BadFoodWeight;
    public int ToxicFoodWeight;

    [Header("Food Gap")]
    public int FitGap;
    public int FatGap;
    public int FatGap2;
    public int OverfatGap;
    public int DyingGap;

    [Header("Speed By Weight")]
    public int SpeedFit;
    public int SpeedFat;
    public int SpeedFat2;
    public int SpeedOverfat;
    public int SpeedLoose;

    private Rigidbody2D rigid;
    private CapsuleCollider2D colliderCaps;
    [HideInInspector]
    public UnityArmatureComponent currentArmature;

    private bool isGrounded;
    private int score;
    private FoodSpawner foodManager;
    private Dictionary<int, ArmatureFactoryComponent> weightArmatures;
    private Dictionary<int, int> speedFoods;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        colliderCaps = GetComponent<CapsuleCollider2D>();
        foodManager = GameManager.instance.FoodManager;
        GameManager.instance.CanvaManager.SetScore(0);

        InitArmature();
        speedFoods = new Dictionary<int, int>
        {
            {FitGap,  SpeedFit},
            {FatGap,  SpeedFat},
            {FatGap2,  SpeedFat2},
            {OverfatGap,  SpeedOverfat},
            {DyingGap,  SpeedLoose},
        };

        AddWeight(FatGap2);
        currentArmature.animation.FadeIn("Run1", 0.25f);
    }

    private void SetWeightVisualyByGap(int gap)
    {
        var speed = (speedFoods[gap] * Time.deltaTime);
        foodManager.SpeedFood = speed;
        GameManager.instance.BackgroundManager.HorizontalSpeedBackground = speed * -1;
        SetCurrentArmature(weightArmatures[gap].ArmatureName, weightArmatures[gap].AnimationNameDefault);
    }

    private void InitArmature()
    {
        var goodArmature = new ArmatureFactoryComponent
        {
            Data_ske = UnityFactory.factory.LoadDragonBonesData("Animations/Runfit/Runfit_ske"),
            Data_tex = UnityFactory.factory.LoadTextureAtlasData("Animations/Runfit/Runfit_tex"),
            ArmatureName = "RunfitArmature",
            AnimationNameDefault = "Run1"
        };
        var fatArmature = new ArmatureFactoryComponent
        {
            Data_ske = UnityFactory.factory.LoadDragonBonesData("Animations/runfat1/runfat1_ske"),
            Data_tex = UnityFactory.factory.LoadTextureAtlasData("Animations/runfat1/runfat1_tex"),
            ArmatureName = "Runfat1Armature",
            AnimationNameDefault = "Run1"
        };
        var fatArmature2 = new ArmatureFactoryComponent
        {
            Data_ske = UnityFactory.factory.LoadDragonBonesData("Animations/runfat2/runfat2_ske"),
            Data_tex = UnityFactory.factory.LoadTextureAtlasData("Animations/runfat2/runfat2_tex"),
            ArmatureName = "Runfat2Armature",
            AnimationNameDefault = "Run1"
        };
        var overfatArmature = new ArmatureFactoryComponent
        {
            Data_ske = UnityFactory.factory.LoadDragonBonesData("Animations/overfat/overfat_ske"),
            Data_tex = UnityFactory.factory.LoadTextureAtlasData("Animations/overfat/overfat_tex"),
            ArmatureName = "OverfatArmature",
            AnimationNameDefault = "Run1"
        };
        var looseArmature = new ArmatureFactoryComponent
        {
            Data_ske = UnityFactory.factory.LoadDragonBonesData("Animations/boom/boom_ske"),
            Data_tex = UnityFactory.factory.LoadTextureAtlasData("Animations/boom/boom_tex"),
            ArmatureName = "BoomArmature",
            AnimationNameDefault = "explode"
        };


        weightArmatures = new Dictionary<int, ArmatureFactoryComponent>
        {
            {FitGap, goodArmature },
            {FatGap, fatArmature },
            {FatGap2, fatArmature2 },
            {OverfatGap, overfatArmature },
            {DyingGap, looseArmature }
        };
    }

    private void SetCurrentArmature(string armatureName, string defaultAnimation)
    {
        if (currentArmature != null)
        {
            if (currentArmature.name == armatureName)
                return;
            Destroy(currentArmature.gameObject);
        }
        currentArmature = UnityFactory.factory.BuildArmatureComponent(armatureName);
        currentArmature.transform.parent = transform;
        currentArmature.transform.localPosition = Vector3.zero;
        currentArmature.transform.localScale = new Vector3(0.6f, 0.6f);
        currentArmature.animation.FadeIn(defaultAnimation, 0.25f);
    }

    private void FixedUpdate()
    {
        if (InputManager.instance.BlockInput)
            return;
        var isGrounded = IsGrounded();
        if (InputManager.instance.Jump && isGrounded)
        {
            currentArmature.animation.FadeIn("Jump", 0.15f, 1);
            rigid.AddForce(new Vector2(0, JumpForce * Time.deltaTime), ForceMode2D.Impulse);
            isGrounded = false;
        }

        if (isGrounded && currentArmature.animation.lastAnimationName == "Jump")
            currentArmature.animation.FadeIn("Run1", 0.10f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var tag = collision.gameObject.tag;
        switch (tag)
        {
            case "GoodFood":
                score += GoodFoodScore;
                AddWeight(GoodFoodWeight);
                break;
            case "BadFood":
                score += BadFoodScore;
                AddWeight(BadFoodWeight);
                break;
            case "ToxicFood":
                score += ToxicFoodScore;
                AddWeight(ToxicFoodWeight);
                break;
            case "Roulette":
                StartCoroutine("Loose");
                break;
            default:
                break;
        }
        GameManager.instance.CanvaManager.SetScore(score);
        if (collision.gameObject.layer == LayerMask.NameToLayer("Food"))
        {
            Destroy(collision.gameObject);
            GameManager.instance.FoodManager.FoodsSpawned.Remove(collision.gameObject);
        }
    }

    private IEnumerator Loose()
    {
        SetWeightVisualyByGap(DyingGap);
        currentArmature.animation.FadeIn("explode", 0.25f, 1);
        yield return new WaitForSeconds(1);
        GameManager.instance.Loose();
    }

    private void AddWeight(int weightToAdd)
    {
        CurrentWeight += CurrentWeight < 0 ? 0 : weightToAdd;

        int gap = 0;
        if (CurrentWeight <= FitGap)
            gap = FitGap;
        else if (CurrentWeight <= FatGap)
            gap = FatGap;
        else if (CurrentWeight <= FatGap2)
            gap = FatGap2;
        else if (CurrentWeight <= OverfatGap || CurrentWeight < DyingGap)
            gap = OverfatGap;
        else
        {
            StartCoroutine("Loose");
            return;
        }

        SetWeightVisualyByGap(gap);
    }

    private bool IsGrounded()
    {
        var origin = new Vector2(colliderCaps.bounds.center.x, colliderCaps.bounds.min.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, -Vector2.up, 0.1f);

        return hit.collider != null && hit.collider.gameObject.CompareTag("Ground");
    }
}
