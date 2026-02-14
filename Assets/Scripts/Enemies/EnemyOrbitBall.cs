using UnityEngine;
using System.Collections;

public class EnemyOrbitBall : MonoBehaviour
{
    private const float OrbitRadius = 0.8f;
    private const float OrbitSpeed = 150f;
    private const float BallScale = 0.4f;
    private const float LaunchSpeed = 10f;
    private const float MinOrbitTime = 2f;
    private const float MaxOrbitTime = 5f;

    private GameObject _ball;
    private float _orbitAngle;
    private bool _launched;
    private Sprite[] _bulletSprites;

    void Start()
    {
        _bulletSprites = Resources.LoadAll<Sprite>("Sprites/Bullets");
        SpawnBall();
        StartCoroutine(AutoLaunchRoutine());
    }

    void Update()
    {
        if (_launched || _ball == null) return;

        _orbitAngle += OrbitSpeed * Time.deltaTime;
        float rad = _orbitAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad) * OrbitRadius, Mathf.Sin(rad) * OrbitRadius, 0f);
        _ball.transform.position = transform.position + offset;
        _ball.transform.Rotate(0, 0, 200f * Time.deltaTime);
    }

    void OnDestroy()
    {
        if (_ball != null) Destroy(_ball);
    }

    void SpawnBall()
    {
        _ball = new GameObject("OrbitBall");
        _ball.layer = LayerMask.NameToLayer("Wall");
        _ball.transform.position = transform.position + Vector3.right * OrbitRadius;
        _ball.transform.localScale = Vector3.one * BallScale;

        CircleCollider2D col = _ball.AddComponent<CircleCollider2D>();
        var mat = new PhysicsMaterial2D("OrbitBounce");
        mat.bounciness = 1f;
        mat.friction = 0f;
        col.sharedMaterial = mat;

        SpriteRenderer sr = _ball.AddComponent<SpriteRenderer>();
        if (_bulletSprites != null && _bulletSprites.Length > 0)
            sr.sprite = _bulletSprites[Random.Range(0, _bulletSprites.Length)];
        sr.sortingOrder = 5;
    }

    IEnumerator AutoLaunchRoutine()
    {
        // Orbit for a random time, then auto-launch at the bow
        yield return new WaitForSeconds(Random.Range(MinOrbitTime, MaxOrbitTime));

        BowHealth bow = FindFirstObjectByType<BowHealth>();
        if (bow != null && bow.CurrentHP > 0 && !_launched)
            LaunchAtTarget(bow.transform.position);
    }

    public void LaunchAtTarget(Vector3 target)
    {
        if (_launched || _ball == null) return;
        _launched = true;

        var col = _ball.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        StartCoroutine(MoveBallToTarget(target));
    }

    IEnumerator MoveBallToTarget(Vector3 target)
    {
        while (_ball != null && Vector3.Distance(_ball.transform.position, target) > 0.3f)
        {
            _ball.transform.position = Vector3.MoveTowards(_ball.transform.position, target, LaunchSpeed * Time.deltaTime);
            _ball.transform.Rotate(0, 0, 400f * Time.deltaTime);
            yield return null;
        }

        if (_ball != null)
        {
            BowHealth bow = FindFirstObjectByType<BowHealth>();
            if (bow != null && bow.CurrentHP > 0)
            {
                bow.TakeDamage(1);
                DamagePopup.Create(bow.transform.position, 1);
                if (CameraShake.Instance != null)
                    CameraShake.Instance.Shake(0.1f, 0.1f);
            }
            Destroy(_ball);
        }

        Destroy(this);
    }
}
