using DragonBones;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArmatureFactoryComponent
{
    public DragonBonesData Data_ske { get; set; }
    public UnityTextureAtlasData Data_tex { get; set; }
    public string ArmatureName { get; set; }
}

public class PlayerController : MonoBehaviour
{
    public float JumpForce;

    [Header("Score Params")]
    public TextMeshProUGUI ScoreText;
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

    private Rigidbody2D rigid;
    private CapsuleCollider2D colliderCaps;
    private UnityArmatureComponent currentArmature;

    private bool isGrounded;
    private int score;
    private int weight;
    private Dictionary<int, ArmatureFactoryComponent> weightArmatures;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        colliderCaps = GetComponent<CapsuleCollider2D>();


        InitArmature();

        SetCurrentArmature(weightArmatures[FitGap].ArmatureName);
        currentArmature.animation.FadeIn("Run1", 0.25f);
        ScoreText.text = "x 0";
        weight = 10;
    }

    private void InitArmature()
    {
        var goodArmature = new ArmatureFactoryComponent
        {
            Data_ske = UnityFactory.factory.LoadDragonBonesData("Animations/Runfit/Runfit_ske"),
            Data_tex = UnityFactory.factory.LoadTextureAtlasData("Animations/Runfit/Runfit_tex"),
            ArmatureName = "RunfitArmature"
        };
        var fatArmature = new ArmatureFactoryComponent
        {
            Data_ske = UnityFactory.factory.LoadDragonBonesData("Animations/runfat1/runfat1_ske"),
            Data_tex = UnityFactory.factory.LoadTextureAtlasData("Animations/runfat1/runfat1_tex"),
            ArmatureName = "Runfat1Armature"
        };
        var fatArmature2 = new ArmatureFactoryComponent
        {
            Data_ske = UnityFactory.factory.LoadDragonBonesData("Animations/runfat2/runfat2_ske"),
            Data_tex = UnityFactory.factory.LoadTextureAtlasData("Animations/runfat2/runfat2_tex"),
            ArmatureName = "Runfat2Armature"
        };
        var overfatArmature = new ArmatureFactoryComponent
        {
            Data_ske = UnityFactory.factory.LoadDragonBonesData("Animations/overfat/overfat_ske"),
            Data_tex = UnityFactory.factory.LoadTextureAtlasData("Animations/overfat/overfat_tex"),
            ArmatureName = "OverfatArmature"
        };


        weightArmatures = new Dictionary<int, ArmatureFactoryComponent>
        {
            {FitGap, goodArmature },
            {FatGap, fatArmature },
            {FatGap2, fatArmature2 },
            {OverfatGap, overfatArmature }
        };
    }

    private void SetCurrentArmature(string armatureName)
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
        currentArmature.animation.FadeIn("Run1", 0.25f);
    }

    private void FixedUpdate()
    {
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
            default:
                break;
        }
        ScoreText.text = "x " + score.ToString();
        if (collision.gameObject.layer == LayerMask.NameToLayer("Food"))
        {
            Destroy(collision.gameObject);
            GameManager.instance.FoodManager.FoodsSpawned.Remove(collision.gameObject);



        }
    }

    private void AddWeight(int weightToAdd)
    {
        weight += weightToAdd;

        int armatureIndex = 0;
        if (weight <= FitGap)
            armatureIndex = FitGap;
        else if (weight <= FatGap)
            armatureIndex = FatGap;
        else if (weight <= FatGap2)
            armatureIndex = FatGap2;
        else
            armatureIndex = OverfatGap;

        SetCurrentArmature(weightArmatures[armatureIndex].ArmatureName);
    }

    private bool IsGrounded()
    {
        var origin = new Vector2(colliderCaps.bounds.center.x, colliderCaps.bounds.min.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, -Vector2.up, 0.1f);

        return hit.collider != null && hit.collider.gameObject.CompareTag("Ground");
    }
}
