using UnityEngine;
using System.Collections.Generic;

public class ArrowManager : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject shootFXPrefab;
    [SerializeField] private GameObject deathFXPrefab;
    [SerializeField] private int arrowsPerWave = 3;

    private int _arrowsRemaining;
    private bool _arrowInFlight;
    private BowController _bow;
    private List<GameObject> _activeArrows = new List<GameObject>();

    // Upgrade bonuses (accumulate during run, reset between runs)
    private int _bonusDurability;
    private int _bonusDamage;
    private int _bonusArrowCount;

    public int ArrowsRemaining => _arrowsRemaining;
    public int ArrowsPerWave => arrowsPerWave + _bonusArrowCount;
    public bool CanShoot => _arrowsRemaining > 0 && !_arrowInFlight;

    public int BonusDurability => _bonusDurability;
    public int BonusDamage => _bonusDamage;
    public int BonusArrowCount => _bonusArrowCount;

    public event System.Action<int> OnArrowCountChanged;
    public event System.Action OnAllArrowsUsed;
    public event System.Action OnArrowLanded;

    void Start()
    {
        _bow = FindFirstObjectByType<BowController>();
        if (_bow != null)
            _bow.OnShoot += HandleShoot;
        ResetArrows();
    }

    void OnDestroy()
    {
        if (_bow != null)
            _bow.OnShoot -= HandleShoot;
    }

    void HandleShoot(Vector2 direction, float speed)
    {
        if (!CanShoot) return;
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing) return;

        _arrowsRemaining--;
        _arrowInFlight = true;
        OnArrowCountChanged?.Invoke(_arrowsRemaining);

        // Spawn arrow at bow position, offset upward
        Vector3 spawnPos = _bow.transform.position + (Vector3)(direction.normalized * 0.6f);
        GameObject arrowGO = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
        arrowGO.layer = LayerMask.NameToLayer("Arrow");
        _activeArrows.Add(arrowGO);

        ArrowController arrow = arrowGO.GetComponent<ArrowController>();
        arrow.Launch(direction, speed);

        // Cannon shoot animation
        PlayCannonShootAnimation();

        // Spawn muzzle flash FX
        if (shootFXPrefab != null)
        {
            Vector3 fxPos = _bow.transform.position + (Vector3)(direction.normalized * 0.8f);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Instantiate(shootFXPrefab, fxPos, Quaternion.Euler(0, 0, angle));
        }

        // Apply upgrade bonuses to this arrow
        ArrowDurability durability = arrowGO.GetComponent<ArrowDurability>();
        if (durability != null && _bonusDurability > 0)
            durability.SetMaxHP(4 + _bonusDurability); // base 4 + bonus

        ArrowCollisionHandler collision = arrowGO.GetComponent<ArrowCollisionHandler>();
        if (collision != null && _bonusDamage > 0)
            collision.SetDamage(1 + _bonusDamage); // base 1 + bonus

        // Visual upgrade effects via AllIn1SpriteShader
        ArrowVisualUpgrade.Apply(arrowGO, _bonusDamage, _bonusDurability);

        // Listen for arrow destruction
        if (durability != null)
            durability.OnArrowDestroyed += () => OnArrowFinished();

        ArrowLifetime lifetime = arrowGO.GetComponent<ArrowLifetime>();
        if (lifetime != null)
        {
            // Also handle timeout destruction
            arrowGO.GetComponent<ArrowLifetime>().gameObject.AddComponent<ArrowDestroyCallback>().Init(() => OnArrowFinished());
        }
    }

    void OnArrowFinished()
    {
        if (OnArrowLanded != null)
        {
            OnArrowLanded.Invoke();
        }
        else
        {
            AllowNextShot();
        }
    }

    public void AllowNextShot()
    {
        _arrowInFlight = false;

        if (_arrowsRemaining <= 0)
        {
            OnAllArrowsUsed?.Invoke();
        }
    }

    public void DestroyAllArrows()
    {
        foreach (var go in _activeArrows)
        {
            if (go == null) continue;
            // Neutralize callbacks before destroying so they don't trigger counter-attack
            var cb = go.GetComponent<ArrowDestroyCallback>();
            if (cb != null) cb.Cancel();
            Destroy(go);
        }
        _activeArrows.Clear();
    }

    public void ResetArrows()
    {
        DestroyAllArrows();
        _arrowsRemaining = arrowsPerWave + _bonusArrowCount;
        _arrowInFlight = false;
        OnArrowCountChanged?.Invoke(_arrowsRemaining);
    }

    public void AddBonusDurability(int amount) { _bonusDurability += amount; }
    public void AddBonusDamage(int amount) { _bonusDamage += amount; }
    public void AddBonusArrowCount(int amount) { _bonusArrowCount += amount; }

    public void ResetUpgrades()
    {
        _bonusDurability = 0;
        _bonusDamage = 0;
        _bonusArrowCount = 0;
    }

    public GameObject DeathFXPrefab => deathFXPrefab;

    void PlayCannonShootAnimation()
    {
        if (_bow == null) return;
        var animator = _bow.GetComponent<Animator>();
        if (animator != null)
            animator.SetTrigger("Shoot");
    }
}

// Helper to detect when arrow is destroyed by lifetime timeout
public class ArrowDestroyCallback : MonoBehaviour
{
    private System.Action _callback;
    private bool _called;

    public void Init(System.Action callback)
    {
        _callback = callback;
    }

    public void Cancel()
    {
        _called = true;
        _callback = null;
    }

    void OnDestroy()
    {
        if (!_called)
        {
            _called = true;
            _callback?.Invoke();
        }
    }
}
