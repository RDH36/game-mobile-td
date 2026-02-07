using UnityEngine;
using System.Collections;

/// <summary>
/// After each arrow is destroyed, surviving enemies counter-attack the bow.
/// Handles game over conditions: bow HP=0, or 0 arrows + enemies alive.
/// </summary>
public class EnemyCounterAttack : MonoBehaviour
{
    [SerializeField] private float counterAttackDelay = 0.3f;
    [SerializeField] private float delayBetweenHits = 0.25f;
    [SerializeField] private float projectileSpeed = 12f;

    private EnemySpawner _spawner;
    private ArrowManager _arrowManager;
    private BowHealth _bowHealth;
    private bool _counterAttacking;

    public bool IsCounterAttacking => _counterAttacking;

    void Start()
    {
        _spawner = GetComponent<EnemySpawner>();
        _arrowManager = FindFirstObjectByType<ArrowManager>();
        _bowHealth = FindFirstObjectByType<BowHealth>();

        if (_arrowManager != null)
            _arrowManager.OnArrowLanded += OnArrowLanded;

        if (_spawner != null)
            _spawner.OnAllEnemiesKilled += OnAllEnemiesKilled;
    }

    void OnDestroy()
    {
        if (_arrowManager != null)
            _arrowManager.OnArrowLanded -= OnArrowLanded;
        if (_spawner != null)
            _spawner.OnAllEnemiesKilled -= OnAllEnemiesKilled;
    }

    void OnArrowLanded()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            return;

        if (_spawner.AliveCount > 0)
        {
            StartCoroutine(CounterAttackRoutine());
        }
        else
        {
            _arrowManager.AllowNextShot();
        }
    }

    private IEnumerator CounterAttackRoutine()
    {
        _counterAttacking = true;

        yield return new WaitForSeconds(counterAttackDelay);

        // Snapshot alive enemies (list may change during iteration)
        var enemies = new System.Collections.Generic.List<Enemy>(_spawner.ActiveEnemies);
        enemies.RemoveAll(e => e == null || e.Health.IsDead);

        Vector3 bowPos = _bowHealth.transform.position;
        int totalDamage = 0;

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;

            int dmg = enemy.Data.damage;
            totalDamage += dmg;

            // Fire visual projectile from enemy to bow
            bool projectileHit = false;
            EnemyProjectile.Create(
                enemy.transform.position,
                bowPos,
                dmg,
                projectileSpeed,
                () =>
                {
                    projectileHit = true;
                    _bowHealth.TakeDamage(dmg);
                    DamagePopup.Create(_bowHealth.transform.position, dmg);

                    if (ScreenFlash.Instance != null)
                        ScreenFlash.Instance.FlashRed();
                    if (CameraShake.Instance != null)
                        CameraShake.Instance.Shake(0.25f + dmg * 0.1f, 0.3f);
                }
            );

            // Wait for projectile to arrive
            while (!projectileHit)
                yield return null;

            // Check if bow died
            if (_bowHealth.CurrentHP <= 0)
                break;

            yield return new WaitForSeconds(delayBetweenHits);
        }

        if (totalDamage > 0)
            Debug.Log($"Counter-attack: {enemies.Count} enemies dealt {totalDamage} total damage");

        _counterAttacking = false;

        // If bow survived, allow next shot
        if (_bowHealth.CurrentHP > 0)
        {
            _arrowManager.AllowNextShot();

            // Check: no arrows left + enemies alive = game over
            if (_arrowManager.ArrowsRemaining <= 0 && _spawner.AliveCount > 0)
            {
                GameManager.Instance?.SetState(GameState.GameOver);
                Debug.Log("Game Over: No arrows left and enemies still alive!");
            }
        }
    }

    void OnAllEnemiesKilled()
    {
        // WaveManager listens to this state change and handles next wave / victory
        GameManager.Instance?.SetState(GameState.WaveComplete);
        Debug.Log("Wave Complete! All enemies eliminated.");
    }
}
