using System.Collections;
using Story;
using UnityEngine;

namespace PodshellLeviathan.Mono.Baby;

public class BabyGifting : MonoBehaviour, IScheduledUpdateBehaviour
{
    public Transform giftParent;
    public Collider[] colliders;
    public Animator animator;
    public WaterParkCreature waterPark;
    public float findGiftIntervalMin = 60 * 0.01f; // FIX LATER
    public float findGiftIntervalMax = 60 * 0.01f; // FIX LATER
    // 0: Must be directly behind the camera, 1: must be horizontal to the camera 
    public float offScreenTolerance = 0.7f;
    public float spawnPosterChance = 0.1f;
    public float spawnPosterAgainChance = 0.025f;
    
    private bool _loadingGift;
    private Pickupable _gift;

    private double _timeTryAgain;

    private readonly TechType[] _items =
    {
        TechType.NutrientBlock,
        TechType.FilteredWater,
        TechType.WaterFiltrationSuitWater,
        TechType.Snack1,
        TechType.CreepvineSeedCluster,
        TechType.Copper,
        TechType.Titanium,
        TechType.CreepvinePiece
    };

    private static readonly int Holding = Animator.StringToHash("holding");

    private bool _playerAlreadyGotPosterGoal;

    private void Start()
    {
        ResetTimer();
    }

    private void OnEnable()
    {
        UpdateSchedulerUtils.Register(this);
        SetHoldingState(_gift != null);
    }

    private void OnDisable()
    {
        UpdateSchedulerUtils.Deregister(this);
    }

    private void GrabRandomGift()
    {
        StartCoroutine(GrabGiftCoroutine(GetRandomItem()));
    }

    private TechType GetRandomItem()
    {
        if (Random.value < spawnPosterChance)
        {
            if (CanGiftPoster() || Random.value < spawnPosterAgainChance)
            {
                return Plugin.PodshellPosterInfo.TechType;
            }
        }

        return _items[Random.Range(0, _items.Length)];
    }

    private void ResetTimer()
    {
        _timeTryAgain = DayNightCycle.main.timePassed + Random.Range(findGiftIntervalMin, findGiftIntervalMax);
    }

    private bool CanGiftPoster()
    {
        if (_playerAlreadyGotPosterGoal)
            return false;
        
        var goalComplete = StoryGoalManager.main.IsGoalComplete(Plugin.PickUpPosterGoal.key);
        if (!goalComplete) return true;
        
        _playerAlreadyGotPosterGoal = true;
        return false;
    }

    private IEnumerator GrabGiftCoroutine(TechType giftType)
    {
        _loadingGift = true;
        var task = CraftData.GetPrefabForTechTypeAsync(giftType);
        yield return task;
        if (task.GetResult() == null)
        {
            Plugin.Logger.LogError($"Failed to load prefab by TechType {giftType}!");
            yield break;
        }
        _loadingGift = false;
        var item = Instantiate(task.GetResult());
        item.transform.localScale = GetGiftScale(giftType);
        item.transform.SetParent(giftParent, true);
        item.transform.localPosition = GetGiftOffset(giftType);
        item.transform.localEulerAngles = GetGiftRotation(giftType);

        foreach (var collider in item.GetComponentsInChildren<Collider>())
        {
            foreach (var myCollider in colliders)
            {
                Physics.IgnoreCollision(collider, myCollider);
            }
        }
        
        SetHoldingState(true);
        
        _gift = item.GetComponent<Pickupable>();

        // Ensure the gift is pickupable to prevent softlocking
        if (_gift == null)
        {
            _gift = item.AddComponent<Pickupable>();
        }

        if (!_gift.isPickupable)
        {
            _gift.isPickupable = true;
        }
        
        // Disable physics on gift
        var rb = item.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
    }

    private bool IsOffScreen()
    {
        return Vector3.Dot(MainCamera.camera.transform.forward,
            (transform.position - MainCamera.camera.transform.position).normalized) < -1 + offScreenTolerance;
    }

    public Pickupable GetGift() => _gift;
    public bool IsHoldingGift() => _gift != null;
    
    public string GetProfileTag()
    {
        return "Podshell:BabyGifting";
    }

    public void ScheduledUpdate()
    {
        if (_gift != null && _gift.transform.parent != giftParent)
        {
            // Reset the gift reference in case it was referring to one that is now in the player's inventory
            _gift = null;
            SetHoldingState(false);
        }

        if (waterPark.IsInsideWaterPark())
        {
            return;
        }
        
        if (DayNightCycle.main.timePassed < _timeTryAgain)
        {
            return;
        }
        
        // Grab a gift if possible
        if (!_loadingGift && _gift == null && IsOffScreen())
        {
            GrabRandomGift();
            ResetTimer();
        }
    }
    
    private Vector3 GetGiftOffset(TechType giftType)
    {
        if (giftType == Plugin.PodshellPosterInfo.TechType)
        {
            return new Vector3(0, 0.00012f, 0.00005f);
        }
        
        if (giftType == TechType.CreepvinePiece)
        {
            return new Vector3(0, 0, 0.00001f);
        }

        return Vector3.zero;
    }

    private Vector3 GetGiftRotation(TechType giftType)
    {
        if (giftType == Plugin.PodshellPosterInfo.TechType)
        {
            return new Vector3(340, 180, 180);
        }

        if (giftType == TechType.CreepvinePiece)
        {
            return new Vector3(77, 180, 180);
        }

        if (giftType == TechType.CreepvineSeedCluster)
        {
            return new Vector3(0, 0, 188.571f);
        }
        
        bool isWater = giftType == TechType.FilteredWater || giftType == TechType.WaterFiltrationSuitWater;
        if (isWater)
        {
            return new Vector3(0, 0, 90);
        }

        return new Vector3(-90, 0, 0);
    }
    
    private Vector3 GetGiftScale(TechType giftType)
    {
        if (giftType == TechType.Titanium || giftType == TechType.Copper)
        {
            return Vector3.one * 0.6f;
        }

        return Vector3.one;
    }

    private void SetHoldingState(bool holdingItem)
    {
        animator.SetBool(Holding, holdingItem);
    }

    public int scheduledUpdateIndex { get; set; }
}